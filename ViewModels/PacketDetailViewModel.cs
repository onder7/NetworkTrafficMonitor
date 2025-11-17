using System;
using System.Collections.ObjectModel;
using NetworkTrafficMonitor.Models;

namespace NetworkTrafficMonitor.ViewModels
{
    public class PacketDetailViewModel : BaseViewModel
    {
        private readonly int _maxPackets = 1000;
        private string _filterText = string.Empty;
        private readonly ObservableCollection<PacketDetail> _allPackets = new();

        public ObservableCollection<PacketDetail> Packets { get; }
        public ObservableCollection<string> FlagFilters { get; }

        private string _selectedFlagFilter = "All";
        public string SelectedFlagFilter
        {
            get => _selectedFlagFilter;
            set
            {
                if (SetProperty(ref _selectedFlagFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public PacketDetailViewModel()
        {
            Packets = new ObservableCollection<PacketDetail>();
            FlagFilters = new ObservableCollection<string> 
            { 
                "All", "SYN", "ACK", "FIN", "RST", "PSH", "SYN-ACK", "FIN-ACK" 
            };
        }

        public void AddPacket(PacketDetail packet)
        {
            _allPackets.Add(packet);

            // Maksimum paket sayısını aşarsa eski paketleri sil
            if (_allPackets.Count > _maxPackets)
            {
                _allPackets.RemoveAt(0);
            }

            ApplyFilters();
        }

        public void AddPacketFromConnection(ConnectionInfo connection)
        {
            // ConnectionInfo'dan PacketDetail oluştur
            var packet = new PacketDetail
            {
                Timestamp = DateTime.Now,
                SourceIP = connection.LocalAddress,
                SourcePort = connection.LocalPort,
                DestinationIP = connection.RemoteAddress,
                DestinationPort = connection.RemotePort,
                Protocol = connection.Protocol,
                PacketSize = 0, // Gerçek paket boyutu için ETW gerekli
                
                // TCP state'e göre flag tahmini
                SYN = connection.State == "SYN_SENT" || connection.State == "SYN_RCVD",
                ACK = connection.State == "Established" || connection.State.Contains("ACK"),
                FIN = connection.State.Contains("FIN") || connection.State == "CLOSE_WAIT",
                RST = connection.State == "Closed",
                PSH = connection.State == "Established"
            };

            AddPacket(packet);
        }

        private void ApplyFilters()
        {
            Packets.Clear();

            foreach (var packet in _allPackets)
            {
                // Flag filtresi
                bool flagMatch = SelectedFlagFilter == "All" || MatchesFlag(packet, SelectedFlagFilter);

                // Text filtresi
                bool textMatch = string.IsNullOrWhiteSpace(FilterText) ||
                    packet.SourceIP.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                    packet.DestinationIP.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                    packet.Protocol.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                    packet.TCPFlags.Contains(FilterText, StringComparison.OrdinalIgnoreCase);

                if (flagMatch && textMatch)
                {
                    Packets.Add(packet);
                }
            }
        }

        private bool MatchesFlag(PacketDetail packet, string flag)
        {
            return flag switch
            {
                "SYN" => packet.SYN && !packet.ACK,
                "ACK" => packet.ACK && !packet.SYN && !packet.FIN,
                "FIN" => packet.FIN && !packet.ACK,
                "RST" => packet.RST,
                "PSH" => packet.PSH,
                "SYN-ACK" => packet.SYN && packet.ACK,
                "FIN-ACK" => packet.FIN && packet.ACK,
                _ => true
            };
        }

        public void Clear()
        {
            _allPackets.Clear();
            Packets.Clear();
        }
    }
}
