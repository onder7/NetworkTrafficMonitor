using System.Collections.Generic;

namespace NetworkTrafficMonitor.Services
{
    public class ProtocolExplainService
    {
        private readonly Dictionary<int, string> _portDescriptions = new()
        {
            { 20, "FTP Data Transfer" },
            { 21, "FTP Control" },
            { 22, "SSH - Secure Shell" },
            { 23, "Telnet" },
            { 25, "SMTP - Email" },
            { 53, "DNS - Domain Name System" },
            { 80, "HTTP - Web Traffic" },
            { 110, "POP3 - Email" },
            { 143, "IMAP - Email" },
            { 443, "HTTPS - Secure Web" },
            { 445, "SMB - File Sharing" },
            { 3389, "RDP - Remote Desktop" },
            { 5353, "mDNS - Network Discovery" },
            { 8080, "HTTP Alternate" },
            { 3306, "MySQL Database" },
            { 5432, "PostgreSQL Database" },
            { 27017, "MongoDB Database" },
            { 6379, "Redis Database" }
        };

        public string GetPortDescription(int port)
        {
            return _portDescriptions.TryGetValue(port, out var description) 
                ? description 
                : $"Port {port}";
        }
    }
}
