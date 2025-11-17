using System;

namespace NetworkTrafficMonitor.Models
{
    public class PacketDetail
    {
        public DateTime Timestamp { get; set; }
        public string SourceIP { get; set; } = string.Empty;
        public int SourcePort { get; set; }
        public string DestinationIP { get; set; } = string.Empty;
        public int DestinationPort { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public int PacketSize { get; set; }
        
        // TCP Flags
        public bool FIN { get; set; }  // Connection termination
        public bool SYN { get; set; }  // Synchronize sequence numbers
        public bool RST { get; set; }  // Reset connection
        public bool PSH { get; set; }  // Push data
        public bool ACK { get; set; }  // Acknowledgment
        public bool URG { get; set; }  // Urgent pointer
        public bool ECE { get; set; }  // ECN-Echo
        public bool CWR { get; set; }  // Congestion Window Reduced

        public string TCPFlags => GetTCPFlagsString();
        public string FlagDescription => GetFlagDescription();

        private string GetTCPFlagsString()
        {
            if (Protocol != "TCP") return "-";

            var flags = "";
            if (FIN) flags += "FIN ";
            if (SYN) flags += "SYN ";
            if (RST) flags += "RST ";
            if (PSH) flags += "PSH ";
            if (ACK) flags += "ACK ";
            if (URG) flags += "URG ";
            if (ECE) flags += "ECE ";
            if (CWR) flags += "CWR ";

            return string.IsNullOrEmpty(flags) ? "NONE" : flags.Trim();
        }

        private string GetFlagDescription()
        {
            if (Protocol != "TCP") return "UDP Packet";

            if (SYN && !ACK) return "Connection Request (SYN)";
            if (SYN && ACK) return "Connection Acknowledgment (SYN-ACK)";
            if (FIN && ACK) return "Connection Closing (FIN-ACK)";
            if (RST) return "Connection Reset (RST)";
            if (ACK && !PSH && !FIN) return "Acknowledgment (ACK)";
            if (PSH && ACK) return "Data Push (PSH-ACK)";
            if (FIN) return "Connection Termination (FIN)";

            return "TCP Packet";
        }

        public string PacketSizeFormatted => $"{PacketSize} bytes";
    }
}
