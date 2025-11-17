using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace NetworkTrafficMonitor.Services
{
    public class PerformanceMonitorService
    {
        private readonly Dictionary<string, NetworkInterfaceStats> _interfaceStats = new();
        private readonly Dictionary<int, ProcessStats> _processStats = new();

        public class NetworkInterfaceStats
        {
            public string Name { get; set; } = string.Empty;
            public long BytesSent { get; set; }
            public long BytesReceived { get; set; }
            public long PacketsSent { get; set; }
            public long PacketsReceived { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        public class ProcessStats
        {
            public int ProcessId { get; set; }
            public string ProcessName { get; set; } = string.Empty;
            public long BytesSent { get; set; }
            public long BytesReceived { get; set; }
            public int ConnectionCount { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        public List<NetworkInterfaceStats> GetNetworkInterfaceStats()
        {
            var stats = new List<NetworkInterfaceStats>();

            try
            {
                var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PerfFormattedData_Tcpip_NetworkInterface");

                foreach (ManagementObject obj in searcher.Get())
                {
                    var name = obj["Name"]?.ToString() ?? "";
                    
                    // Loopback ve sanal adaptörleri atla
                    if (name.Contains("Loopback") || name.Contains("isatap"))
                        continue;

                    var stat = new NetworkInterfaceStats
                    {
                        Name = name,
                        BytesSent = Convert.ToInt64(obj["BytesSentPerSec"] ?? 0),
                        BytesReceived = Convert.ToInt64(obj["BytesReceivedPerSec"] ?? 0),
                        PacketsSent = Convert.ToInt64(obj["PacketsSentPerSec"] ?? 0),
                        PacketsReceived = Convert.ToInt64(obj["PacketsReceivedPerSec"] ?? 0),
                        LastUpdate = DateTime.Now
                    };

                    stats.Add(stat);
                    
                    // Cache'e kaydet
                    if (_interfaceStats.ContainsKey(name))
                    {
                        var prev = _interfaceStats[name];
                        stat.BytesSent += prev.BytesSent;
                        stat.BytesReceived += prev.BytesReceived;
                    }
                    
                    _interfaceStats[name] = stat;
                }
            }
            catch
            {
                // Stats not available
            }

            return stats;
        }

        public (long totalSent, long totalReceived) GetTotalNetworkStats()
        {
            var stats = GetNetworkInterfaceStats();
            var totalSent = stats.Sum(s => s.BytesSent);
            var totalReceived = stats.Sum(s => s.BytesReceived);
            return (totalSent, totalReceived);
        }

        public ProcessStats GetProcessStats(int processId, string processName, int connectionCount)
        {
            // Process bazlı network istatistikleri için tahmin
            // Gerçek değerler için ETW (Event Tracing for Windows) gerekli
            
            if (_processStats.TryGetValue(processId, out var prevStats))
            {
                var timeDiff = (DateTime.Now - prevStats.LastUpdate).TotalSeconds;
                
                if (timeDiff > 0)
                {
                    // Bağlantı sayısına göre tahmin
                    // Her bağlantı için ortalama 1KB/s varsayalım
                    var estimatedBytes = (long)(connectionCount * 1024 * timeDiff);
                    
                    prevStats.BytesSent += estimatedBytes / 2;
                    prevStats.BytesReceived += estimatedBytes / 2;
                    prevStats.ConnectionCount = connectionCount;
                    prevStats.LastUpdate = DateTime.Now;
                    
                    return prevStats;
                }
                
                return prevStats;
            }
            else
            {
                var newStats = new ProcessStats
                {
                    ProcessId = processId,
                    ProcessName = processName,
                    BytesSent = 0,
                    BytesReceived = 0,
                    ConnectionCount = connectionCount,
                    LastUpdate = DateTime.Now
                };
                
                _processStats[processId] = newStats;
                return newStats;
            }
        }

        public void ResetStats()
        {
            _interfaceStats.Clear();
            _processStats.Clear();
        }
    }
}
