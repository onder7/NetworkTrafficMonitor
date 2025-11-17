using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetworkTrafficMonitor.Models;

namespace NetworkTrafficMonitor.Services
{
    public class PacketCaptureService
    {
        private readonly TrafficService _trafficService;
        private readonly ProtocolExplainService _protocolService;
        private readonly FirewallService _firewallService;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isRunning;

        public event EventHandler<List<ConnectionInfo>>? ConnectionsUpdated;

        public PacketCaptureService()
        {
            _trafficService = new TrafficService();
            _protocolService = new ProtocolExplainService();
            _firewallService = new FirewallService();
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => CaptureLoop(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
        }

        private async Task CaptureLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var connections = new List<ConnectionInfo>();
                    
                    // TCP bağlantılarını al
                    var tcpConnections = _trafficService.GetTcpConnections();
                    
                    foreach (var conn in tcpConnections)
                    {
                        conn.Description = _protocolService.GetPortDescription(conn.RemotePort);
                        
                        // Domain çözümlemesi (sadece outbound için) - ama çok yavaşlatmasın diye skip
                        // if (conn.Direction == "Outbound" && conn.RemoteAddress != "0.0.0.0")
                        // {
                        //     conn.Domain = await _trafficService.ResolveDomainAsync(conn.RemoteAddress);
                        // }
                        conn.Domain = conn.RemoteAddress; // Şimdilik IP'yi göster
                    }
                    connections.AddRange(tcpConnections);

                    // UDP bağlantılarını al
                    var udpConnections = _trafficService.GetUdpConnections();
                    
                    foreach (var conn in udpConnections)
                    {
                        conn.Description = _protocolService.GetPortDescription(conn.LocalPort);
                    }
                    connections.AddRange(udpConnections);
                    
                    // Event'i tetikle
                    ConnectionsUpdated?.Invoke(this, connections);

                    await Task.Delay(1000, cancellationToken); // 1 saniye bekle
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }
    }
}
