namespace NetworkTrafficMonitor.Models
{
    public class ProcessTraffic
    {
        public string ProcessName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
        public int ConnectionCount { get; set; }

        public string BytesSentFormatted => FormatBytes(BytesSent);
        public string BytesReceivedFormatted => FormatBytes(BytesReceived);

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
