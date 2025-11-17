using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NetworkTrafficMonitor.Models;
using NetworkTrafficMonitor.Services;

namespace NetworkTrafficMonitor.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private DateTime _startDate = DateTime.Now.AddHours(-1);
        private DateTime _endDate = DateTime.Now;
        private string _searchText = string.Empty;
        private DatabaseStats? _stats;

        public ObservableCollection<ConnectionInfo> HistoryConnections { get; }
        public Dictionary<string, int> TopProcesses { get; private set; } = new();
        public Dictionary<string, int> TopRemoteIPs { get; private set; } = new();

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public DatabaseStats? Stats
        {
            get => _stats;
            set => SetProperty(ref _stats, value);
        }

        public ICommand LoadHistoryCommand { get; }
        public ICommand CleanOldRecordsCommand { get; }
        public ICommand RefreshStatsCommand { get; }
        public ICommand SetLast1HourCommand { get; }
        public ICommand SetLast24HoursCommand { get; }
        public ICommand SetLast7DaysCommand { get; }

        public HistoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            HistoryConnections = new ObservableCollection<ConnectionInfo>();

            LoadHistoryCommand = new RelayCommand(LoadHistory);
            CleanOldRecordsCommand = new RelayCommand(CleanOldRecords);
            RefreshStatsCommand = new RelayCommand(RefreshStats);
            SetLast1HourCommand = new RelayCommand(() => SetTimeRange(1, "hours"));
            SetLast24HoursCommand = new RelayCommand(() => SetTimeRange(24, "hours"));
            SetLast7DaysCommand = new RelayCommand(() => SetTimeRange(7, "days"));

            RefreshStats();
        }

        private void LoadHistory()
        {
            try
            {
                var connections = _databaseService.GetConnectionHistory(StartDate, EndDate, 5000);

                // Arama filtresi uygula
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var search = SearchText.ToLower();
                    connections = connections.Where(c =>
                        c.ProcessName.ToLower().Contains(search) ||
                        c.LocalAddress.Contains(search) ||
                        c.RemoteAddress.Contains(search) ||
                        c.Domain.ToLower().Contains(search) ||
                        c.Description.ToLower().Contains(search)
                    ).ToList();
                }

                HistoryConnections.Clear();
                foreach (var conn in connections)
                {
                    HistoryConnections.Add(conn);
                }

                // Top processes ve IPs
                TopProcesses = _databaseService.GetTopProcesses(10, StartDate, EndDate);
                TopRemoteIPs = _databaseService.GetTopRemoteIPs(10, StartDate, EndDate);
                OnPropertyChanged(nameof(TopProcesses));
                OnPropertyChanged(nameof(TopRemoteIPs));

                System.Windows.MessageBox.Show(
                    $"Loaded {HistoryConnections.Count} records\n" +
                    $"From: {StartDate:yyyy-MM-dd HH:mm}\n" +
                    $"To: {EndDate:yyyy-MM-dd HH:mm}",
                    "History Loaded",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error loading history: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void CleanOldRecords()
        {
            var result = System.Windows.MessageBox.Show(
                "This will delete all records older than 7 days.\n\nAre you sure?",
                "Clean Old Records",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    _databaseService.CleanOldRecords(7);
                    RefreshStats();
                    System.Windows.MessageBox.Show(
                        "Old records cleaned successfully!",
                        "Success",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Error cleaning records: {ex.Message}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void RefreshStats()
        {
            try
            {
                Stats = _databaseService.GetDatabaseStats();
            }
            catch
            {
                // Sessizce devam et
            }
        }

        private void SetTimeRange(int value, string unit)
        {
            EndDate = DateTime.Now;
            StartDate = unit switch
            {
                "hours" => DateTime.Now.AddHours(-value),
                "days" => DateTime.Now.AddDays(-value),
                _ => DateTime.Now.AddHours(-1)
            };
        }
    }
}
