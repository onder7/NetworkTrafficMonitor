using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using NetworkTrafficMonitor.Models;

namespace NetworkTrafficMonitor.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string dbPath = "traffic.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Connections (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LocalAddress TEXT,
                    LocalPort INTEGER,
                    RemoteAddress TEXT,
                    RemotePort INTEGER,
                    Protocol TEXT,
                    ProcessName TEXT,
                    ProcessId INTEGER,
                    Direction TEXT,
                    State TEXT,
                    Domain TEXT,
                    Description TEXT,
                    IsBlocked INTEGER,
                    Timestamp TEXT
                )";
            command.ExecuteNonQuery();
        }

        public void LogConnection(ConnectionInfo connection)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = @"
                INSERT INTO Connections 
                (LocalAddress, LocalPort, RemoteAddress, RemotePort, Protocol, ProcessName, 
                 ProcessId, Direction, State, Domain, Description, IsBlocked, Timestamp)
                VALUES 
                (@LocalAddress, @LocalPort, @RemoteAddress, @RemotePort, @Protocol, @ProcessName,
                 @ProcessId, @Direction, @State, @Domain, @Description, @IsBlocked, @Timestamp)";

            command.Parameters.AddWithValue("@LocalAddress", connection.LocalAddress);
            command.Parameters.AddWithValue("@LocalPort", connection.LocalPort);
            command.Parameters.AddWithValue("@RemoteAddress", connection.RemoteAddress);
            command.Parameters.AddWithValue("@RemotePort", connection.RemotePort);
            command.Parameters.AddWithValue("@Protocol", connection.Protocol);
            command.Parameters.AddWithValue("@ProcessName", connection.ProcessName);
            command.Parameters.AddWithValue("@ProcessId", connection.ProcessId);
            command.Parameters.AddWithValue("@Direction", connection.Direction);
            command.Parameters.AddWithValue("@State", connection.State);
            command.Parameters.AddWithValue("@Domain", connection.Domain);
            command.Parameters.AddWithValue("@Description", connection.Description);
            command.Parameters.AddWithValue("@IsBlocked", connection.IsBlocked ? 1 : 0);
            command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("o"));

            command.ExecuteNonQuery();
        }

        public List<ConnectionInfo> GetConnectionHistory(DateTime? startDate = null, DateTime? endDate = null, int limit = 1000)
        {
            var connections = new List<ConnectionInfo>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM Connections WHERE 1=1";

            if (startDate.HasValue)
            {
                command.CommandText += " AND Timestamp >= @StartDate";
                command.Parameters.AddWithValue("@StartDate", startDate.Value.ToString("o"));
            }

            if (endDate.HasValue)
            {
                command.CommandText += " AND Timestamp <= @EndDate";
                command.Parameters.AddWithValue("@EndDate", endDate.Value.ToString("o"));
            }

            command.CommandText += $" ORDER BY Timestamp DESC LIMIT {limit}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                connections.Add(new ConnectionInfo
                {
                    LocalAddress = reader.GetString(1),
                    LocalPort = reader.GetInt32(2),
                    RemoteAddress = reader.GetString(3),
                    RemotePort = reader.GetInt32(4),
                    Protocol = reader.GetString(5),
                    ProcessName = reader.GetString(6),
                    ProcessId = reader.GetInt32(7),
                    Direction = reader.GetString(8),
                    State = reader.GetString(9),
                    Domain = reader.GetString(10),
                    Description = reader.GetString(11),
                    IsBlocked = reader.GetInt32(12) == 1,
                    FirstSeen = DateTime.Parse(reader.GetString(13))
                });
            }

            return connections;
        }

        public void CleanOldRecords(int daysToKeep = 7)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = @"
                DELETE FROM Connections 
                WHERE Timestamp < @CutoffDate";

            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            command.Parameters.AddWithValue("@CutoffDate", cutoffDate.ToString("o"));

            command.ExecuteNonQuery();
        }

        public DatabaseStats GetDatabaseStats()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var stats = new DatabaseStats();

            // Toplam kayıt sayısı
            var command = conn.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Connections";
            stats.TotalRecords = Convert.ToInt32(command.ExecuteScalar());

            // En eski kayıt
            command.CommandText = "SELECT MIN(Timestamp) FROM Connections";
            var oldestStr = command.ExecuteScalar()?.ToString();
            if (!string.IsNullOrEmpty(oldestStr))
            {
                stats.OldestRecord = DateTime.Parse(oldestStr);
            }

            // En yeni kayıt
            command.CommandText = "SELECT MAX(Timestamp) FROM Connections";
            var newestStr = command.ExecuteScalar()?.ToString();
            if (!string.IsNullOrEmpty(newestStr))
            {
                stats.NewestRecord = DateTime.Parse(newestStr);
            }

            // Veritabanı boyutu
            command.CommandText = "SELECT page_count * page_size as size FROM pragma_page_count(), pragma_page_size()";
            stats.DatabaseSizeBytes = Convert.ToInt64(command.ExecuteScalar());

            return stats;
        }

        public Dictionary<string, int> GetTopProcesses(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = new Dictionary<string, int>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT ProcessName, COUNT(*) as Count 
                FROM Connections 
                WHERE 1=1";

            if (startDate.HasValue)
            {
                command.CommandText += " AND Timestamp >= @StartDate";
                command.Parameters.AddWithValue("@StartDate", startDate.Value.ToString("o"));
            }

            if (endDate.HasValue)
            {
                command.CommandText += " AND Timestamp <= @EndDate";
                command.Parameters.AddWithValue("@EndDate", endDate.Value.ToString("o"));
            }

            command.CommandText += $" GROUP BY ProcessName ORDER BY Count DESC LIMIT {topN}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }

            return result;
        }

        public Dictionary<string, int> GetTopRemoteIPs(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = new Dictionary<string, int>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT RemoteAddress, COUNT(*) as Count 
                FROM Connections 
                WHERE RemoteAddress != '0.0.0.0' AND RemoteAddress != '127.0.0.1'";

            if (startDate.HasValue)
            {
                command.CommandText += " AND Timestamp >= @StartDate";
                command.Parameters.AddWithValue("@StartDate", startDate.Value.ToString("o"));
            }

            if (endDate.HasValue)
            {
                command.CommandText += " AND Timestamp <= @EndDate";
                command.Parameters.AddWithValue("@EndDate", endDate.Value.ToString("o"));
            }

            command.CommandText += $" GROUP BY RemoteAddress ORDER BY Count DESC LIMIT {topN}";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result[reader.GetString(0)] = reader.GetInt32(1);
            }

            return result;
        }
    }

    public class DatabaseStats
    {
        public int TotalRecords { get; set; }
        public DateTime? OldestRecord { get; set; }
        public DateTime? NewestRecord { get; set; }
        public long DatabaseSizeBytes { get; set; }

        public string DatabaseSizeFormatted
        {
            get
            {
                if (DatabaseSizeBytes < 1024) return $"{DatabaseSizeBytes} B";
                if (DatabaseSizeBytes < 1024 * 1024) return $"{DatabaseSizeBytes / 1024.0:F2} KB";
                if (DatabaseSizeBytes < 1024 * 1024 * 1024) return $"{DatabaseSizeBytes / (1024.0 * 1024):F2} MB";
                return $"{DatabaseSizeBytes / (1024.0 * 1024 * 1024):F2} GB";
            }
        }

        public TimeSpan? DataSpan => OldestRecord.HasValue && NewestRecord.HasValue 
            ? NewestRecord.Value - OldestRecord.Value 
            : null;
    }
}
