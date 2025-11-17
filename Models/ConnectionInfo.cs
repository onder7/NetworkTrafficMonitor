using System;

namespace NetworkTrafficMonitor.Models
{
    public class ConnectionInfo
    {
        public string LocalAddress { get; set; } = string.Empty;
        public int LocalPort { get; set; }
        public string RemoteAddress { get; set; } = string.Empty;
        public int RemotePort { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public string Direction { get; set; } = string.Empty;
        public DateTime FirstSeen { get; set; }
        public string State { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public long PacketCount { get; set; }
    }
}
