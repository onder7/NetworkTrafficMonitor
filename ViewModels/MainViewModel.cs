using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NetworkTrafficMonitor.Models;
using NetworkTrafficMonitor.Services;

namespace NetworkTrafficMonitor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly PacketCaptureService _captureService;
        private readonly DatabaseService _databaseService;
        private readonly PerformanceMonitorService _performanceMonitor;
        private readonly ProcessManagementService _processManagement;
        private readonly FirewallService _firewallService;
        private string _statusText = "Stopped";
        private bool _isMonitoring;
        private ProcessTraffic? _selectedProcess;
        private ConnectionInfo? _selectedConnection;
        private string _searchText = string.Empty;
        private string _selectedProtocol = "All";
        private readonly List<ConnectionInfo> _allInboundConnections = new();
        private readonly List<ConnectionInfo> _allOutboundConnections = new();
        private long _totalBytesSent;
        private long _totalBytesReceived;

        public long TotalBytesSent
        {
            get => _totalBytesSent;
            set => SetProperty(ref _totalBytesSent, value);
        }

        public long TotalBytesReceived
        {
            get => _totalBytesReceived;
            set => SetProperty(ref _totalBytesReceived, value);
        }

        public ObservableCollection<ConnectionInfo> InboundConnections { get; }
        public ObservableCollection<ConnectionInfo> OutboundConnections { get; }
        public ObservableCollection<ProcessTraffic> ProcessTraffics { get; }
        public ObservableCollection<string> ProtocolFilters { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SelectedProtocol
        {
            get => _selectedProtocol;
            set
            {
                if (SetProperty(ref _selectedProtocol, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand KillProcessCommand { get; }
        public ICommand KillProcessForceCommand { get; }
        public ICommand SuperKillCommand { get; }
        public ICommand BlockIPCommand { get; }
        public ICommand AllowIPCommand { get; }

        public ProcessTraffic? SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (SetProperty(ref _selectedProcess, value))
                {
                    // Command'ların CanExecute durumunu güncelle
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ConnectionInfo? SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (SetProperty(ref _selectedConnection, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ChartViewModel ChartViewModel { get; }
        public PacketDetailViewModel PacketDetailViewModel { get; }
        public HistoryViewModel HistoryViewModel { get; }

        public MainViewModel()
        {
            InboundConnections = new ObservableCollection<ConnectionInfo>();
            OutboundConnections = new ObservableCollection<ConnectionInfo>();
            ProcessTraffics = new ObservableCollection<ProcessTraffic>();
            ProtocolFilters = new ObservableCollection<string> { "All", "TCP", "UDP" };
            
            _captureService = new PacketCaptureService();
            _databaseService = new DatabaseService();
            _performanceMonitor = new PerformanceMonitorService();
            _processManagement = new ProcessManagementService();
            _firewallService = new FirewallService();
            
            ChartViewModel = new ChartViewModel();
            PacketDetailViewModel = new PacketDetailViewModel();
            HistoryViewModel = new HistoryViewModel(_databaseService);

            _captureService.ConnectionsUpdated += OnConnectionsUpdated;

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            ClearCommand = new RelayCommand(Clear);
            KillProcessCommand = new RelayCommand(KillProcess, CanKillProcess);
            KillProcessForceCommand = new RelayCommand(KillProcessForce, CanKillProcess);
            SuperKillCommand = new RelayCommand(SuperKill, CanKillProcess);
            BlockIPCommand = new RelayCommand(BlockIP, CanManageFirewall);
            AllowIPCommand = new RelayCommand(AllowIP, CanManageFirewall);

            // Başlangıçta eski kayıtları temizle (7 günden eski)
            try
            {
                _databaseService.CleanOldRecords(7);
            }
            catch
            {
                // Sessizce devam et
            }
        }

        private void Start()
        {
            try
            {
                _captureService.Start();
                IsMonitoring = true;
                StatusText = "Monitoring...";
                System.Windows.MessageBox.Show("Monitoring started! Data should appear in 1-2 seconds.", "Info", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error starting monitor: {ex.Message}\n\n{ex.StackTrace}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void Stop()
        {
            _captureService.Stop();
            IsMonitoring = false;
            StatusText = "Stopped";
        }

        private void Clear()
        {
            InboundConnections.Clear();
            OutboundConnections.Clear();
            ProcessTraffics.Clear();
            _allInboundConnections.Clear();
            _allOutboundConnections.Clear();
            ChartViewModel.Clear();
            PacketDetailViewModel.Clear();
        }

        private bool CanKillProcess()
        {
            // Process seçilmemiş
            if (SelectedProcess == null)
                return false;

            // Sistem process'lerini koruma
            if (_processManagement.IsSystemProcess(SelectedProcess.ProcessId))
                return false;

            // Monitoring durumu önemli değil - her zaman kill edilebilir
            return true;
        }

        private void KillProcess()
        {
            if (SelectedProcess == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to close this process?\n\n" +
                $"Process: {SelectedProcess.ProcessName}\n" +
                $"PID: {SelectedProcess.ProcessId}\n" +
                $"Connections: {SelectedProcess.ConnectionCount}\n\n" +
                $"This will attempt to close the process gracefully.",
                "Close Process",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _processManagement.KillProcess(SelectedProcess.ProcessId, force: false);
                    
                    if (success)
                    {
                        System.Windows.MessageBox.Show(
                            $"Process '{SelectedProcess.ProcessName}' closed successfully!",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(
                            $"Failed to close process '{SelectedProcess.ProcessName}'.\n\n" +
                            $"The process may have already exited or you may not have permission.",
                            "Error",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Error closing process: {ex.Message}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void KillProcessForce()
        {
            if (SelectedProcess == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"⚠️ WARNING: Force Kill Process ⚠️\n\n" +
                $"Process: {SelectedProcess.ProcessName}\n" +
                $"PID: {SelectedProcess.ProcessId}\n" +
                $"Connections: {SelectedProcess.ConnectionCount}\n\n" +
                $"This will FORCEFULLY terminate the process using:\n" +
                $"1. Process.Kill()\n" +
                $"2. taskkill /F /T (if needed)\n\n" +
                $"Unsaved data will be LOST!\n\n" +
                $"Are you absolutely sure?",
                "Force Kill Process",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    System.Windows.MessageBox.Show(
                        $"Attempting to kill process {SelectedProcess.ProcessId}...\n\n" +
                        $"This may take a few seconds.",
                        "Please Wait",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);

                    bool success = _processManagement.KillProcess(SelectedProcess.ProcessId, force: true);
                    
                    if (success)
                    {
                        System.Windows.MessageBox.Show(
                            $"✅ Process '{SelectedProcess.ProcessName}' (PID: {SelectedProcess.ProcessId}) terminated!\n\n" +
                            $"The process has been forcefully killed using taskkill.",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        var manualCommand = $"taskkill /F /PID {SelectedProcess.ProcessId} /T";
                        System.Windows.MessageBox.Show(
                            $"❌ Failed to kill process '{SelectedProcess.ProcessName}' (PID: {SelectedProcess.ProcessId})\n\n" +
                            $"Possible reasons:\n" +
                            $"• Process has special protection\n" +
                            $"• Insufficient permissions\n" +
                            $"• Process already exited\n\n" +
                            $"Try manually in Administrator Command Prompt:\n" +
                            $"{manualCommand}",
                            "Error",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"❌ Exception while killing process:\n\n" +
                        $"{ex.Message}\n\n" +
                        $"Stack Trace:\n{ex.StackTrace}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void OnConnectionsUpdated(object? sender, System.Collections.Generic.List<ConnectionInfo> connections)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // Tüm bağlantıları sakla
                    _allInboundConnections.Clear();
                    _allOutboundConnections.Clear();

                    foreach (var conn in connections)
                    {
                        if (conn.Direction == "Inbound")
                            _allInboundConnections.Add(conn);
                        else
                            _allOutboundConnections.Add(conn);

                        // Paket detayı ekle (sadece TCP için)
                        if (conn.Protocol == "TCP")
                        {
                            PacketDetailViewModel.AddPacketFromConnection(conn);
                        }

                        // Log to database
                        try
                        {
                            _databaseService.LogConnection(conn);
                        }
                        catch { }
                    }

                    // Filtreleri uygula
                    ApplyFilters();

                    UpdateProcessTraffic(connections);
                    
                    // İlk veri geldiğinde status güncelle
                    if (connections.Count > 0 && StatusText == "Monitoring...")
                    {
                        StatusText = $"Monitoring... ({connections.Count} connections)";
                    }
                });
            }
            catch
            {
                // Sessizce devam et
            }
        }

        private void ApplyFilters()
        {
            // Inbound filtreleme
            var filteredInbound = _allInboundConnections.AsEnumerable();
            
            // Protokol filtresi
            if (SelectedProtocol != "All")
            {
                filteredInbound = filteredInbound.Where(c => c.Protocol == SelectedProtocol);
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filteredInbound = filteredInbound.Where(c =>
                    c.ProcessName.ToLower().Contains(search) ||
                    c.LocalAddress.Contains(search) ||
                    c.RemoteAddress.Contains(search) ||
                    c.LocalPort.ToString().Contains(search) ||
                    c.RemotePort.ToString().Contains(search) ||
                    c.Description.ToLower().Contains(search) ||
                    c.State.ToLower().Contains(search)
                );
            }

            InboundConnections.Clear();
            foreach (var conn in filteredInbound)
            {
                InboundConnections.Add(conn);
            }

            // Outbound filtreleme
            var filteredOutbound = _allOutboundConnections.AsEnumerable();
            
            if (SelectedProtocol != "All")
            {
                filteredOutbound = filteredOutbound.Where(c => c.Protocol == SelectedProtocol);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filteredOutbound = filteredOutbound.Where(c =>
                    c.ProcessName.ToLower().Contains(search) ||
                    c.LocalAddress.Contains(search) ||
                    c.RemoteAddress.Contains(search) ||
                    c.LocalPort.ToString().Contains(search) ||
                    c.RemotePort.ToString().Contains(search) ||
                    c.Description.ToLower().Contains(search) ||
                    c.Domain.ToLower().Contains(search) ||
                    c.State.ToLower().Contains(search)
                );
            }

            OutboundConnections.Clear();
            foreach (var conn in filteredOutbound)
            {
                OutboundConnections.Add(conn);
            }
        }

        private void UpdateProcessTraffic(System.Collections.Generic.List<ConnectionInfo> connections)
        {
            // Global network stats güncelle
            var (totalSent, totalReceived) = _performanceMonitor.GetTotalNetworkStats();
            TotalBytesSent = totalSent;
            TotalBytesReceived = totalReceived;

            // Chart'a veri ekle
            ChartViewModel.AddDataPoint(totalSent, totalReceived);

            var processGroups = connections
                .GroupBy(c => new { c.ProcessName, c.ProcessId })
                .Select(g =>
                {
                    var connectionCount = g.Count();
                    var stats = _performanceMonitor.GetProcessStats(g.Key.ProcessId, g.Key.ProcessName, connectionCount);
                    
                    return new ProcessTraffic
                    {
                        ProcessName = g.Key.ProcessName,
                        ProcessId = g.Key.ProcessId,
                        ConnectionCount = connectionCount,
                        BytesSent = stats.BytesSent,
                        BytesReceived = stats.BytesReceived
                    };
                })
                .OrderByDescending(p => p.ConnectionCount)
                .Take(20);

            ProcessTraffics.Clear();
            foreach (var pt in processGroups)
            {
                ProcessTraffics.Add(pt);
            }

            // Command'ların durumunu güncelle
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private void SuperKill()
        {
            if (SelectedProcess == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"🔥 SUPER KILL MODE 🔥\n\n" +
                $"Process: {SelectedProcess.ProcessName}\n" +
                $"PID: {SelectedProcess.ProcessId}\n\n" +
                $"This will use MULTIPLE methods:\n" +
                $"1. Process.Kill()\n" +
                $"2. taskkill /F /T\n" +
                $"3. wmic process delete\n\n" +
                $"⚠️ Use only if Force Kill failed!\n\n" +
                $"Continue?",
                "Super Kill",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Stop);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var pid = SelectedProcess.ProcessId;
                    var name = SelectedProcess.ProcessName;

                    // Method 1: taskkill
                    var psi1 = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = $"/F /PID {pid} /T",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(psi1)?.WaitForExit(3000);

                    System.Threading.Thread.Sleep(1000);

                    // Method 2: WMIC
                    var psi2 = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "wmic",
                        Arguments = $"process where processid={pid} delete",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(psi2)?.WaitForExit(3000);

                    System.Threading.Thread.Sleep(1000);

                    // Kontrol
                    try
                    {
                        var check = System.Diagnostics.Process.GetProcessById(pid);
                        System.Windows.MessageBox.Show(
                            $"⚠️ Process still running!\n\nTry restarting Windows.",
                            "Failed",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Warning);
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show(
                            $"✅ Process terminated!",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanManageFirewall()
        {
            return SelectedConnection != null && !string.IsNullOrEmpty(SelectedConnection.RemoteAddress);
        }

        private void BlockIP()
        {
            if (SelectedConnection == null)
                return;

            var remoteIp = SelectedConnection.RemoteAddress;
            var protocol = SelectedConnection.Protocol;

            var result = System.Windows.MessageBox.Show(
                $"🚫 Block IP Address\n\n" +
                $"Remote IP: {remoteIp}\n" +
                $"Protocol: {protocol}\n" +
                $"Process: {SelectedConnection.ProcessName}\n\n" +
                $"This will add a Windows Firewall rule to BLOCK all outbound connections to this IP.\n\n" +
                $"⚠️ Requires Administrator privileges!\n\n" +
                $"Continue?",
                "Block IP",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _firewallService.BlockRemoteIP(remoteIp, protocol);
                    
                    if (success)
                    {
                        System.Windows.MessageBox.Show(
                            $"✅ Firewall rule created!\n\n" +
                            $"IP {remoteIp} is now BLOCKED for {protocol} protocol.\n\n" +
                            $"Rule name: NetworkMonitor_Block_{remoteIp}_{protocol}",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(
                            $"❌ Failed to create firewall rule!\n\n" +
                            $"Possible reasons:\n" +
                            $"• Application not running as Administrator\n" +
                            $"• Windows Firewall is disabled\n" +
                            $"• Rule already exists\n\n" +
                            $"Try running the application as Administrator.",
                            "Error",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"❌ Error creating firewall rule:\n\n{ex.Message}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void AllowIP()
        {
            if (SelectedConnection == null)
                return;

            var remoteIp = SelectedConnection.RemoteAddress;
            var protocol = SelectedConnection.Protocol;

            var result = System.Windows.MessageBox.Show(
                $"✅ Allow IP Address\n\n" +
                $"Remote IP: {remoteIp}\n" +
                $"Protocol: {protocol}\n" +
                $"Process: {SelectedConnection.ProcessName}\n\n" +
                $"This will add a Windows Firewall rule to ALLOW all outbound connections to this IP.\n\n" +
                $"⚠️ Requires Administrator privileges!\n\n" +
                $"Continue?",
                "Allow IP",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _firewallService.AllowRemoteIP(remoteIp, protocol);
                    
                    if (success)
                    {
                        System.Windows.MessageBox.Show(
                            $"✅ Firewall rule created!\n\n" +
                            $"IP {remoteIp} is now ALLOWED for {protocol} protocol.\n\n" +
                            $"Rule name: NetworkMonitor_Allow_{remoteIp}_{protocol}",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(
                            $"❌ Failed to create firewall rule!\n\n" +
                            $"Possible reasons:\n" +
                            $"• Application not running as Administrator\n" +
                            $"• Windows Firewall is disabled\n" +
                            $"• Rule already exists\n\n" +
                            $"Try running the application as Administrator.",
                            "Error",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"❌ Error creating firewall rule:\n\n{ex.Message}",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
    }
}
