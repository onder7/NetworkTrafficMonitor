using System;
using System.Diagnostics;
using System.Linq;

namespace NetworkTrafficMonitor.Services
{
    public class FirewallService
    {
        public bool BlockRemoteIP(string remoteIp, string protocol = "TCP", string? ruleName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(ruleName))
                {
                    ruleName = $"NetworkMonitor_Block_{remoteIp}_{protocol}";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall add rule name=\"{ruleName}\" dir=out action=block remoteip={remoteIp} protocol={protocol}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using var process = Process.Start(psi);
                if (process == null) return false;

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public bool AllowRemoteIP(string remoteIp, string protocol = "TCP", string? ruleName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(ruleName))
                {
                    ruleName = $"NetworkMonitor_Allow_{remoteIp}_{protocol}";
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall add rule name=\"{ruleName}\" dir=out action=allow remoteip={remoteIp} protocol={protocol}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using var process = Process.Start(psi);
                if (process == null) return false;

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveFirewallRule(string ruleName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall delete rule name=\"{ruleName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using var process = Process.Start(psi);
                if (process == null) return false;

                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public bool IsConnectionBlocked(string remoteIp, int remotePort, string protocol)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall show rule name=all",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return false;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output.Contains("Block") && output.Contains(remoteIp);
            }
            catch
            {
                return false;
            }
        }

        public string GetFirewallProfile()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "advfirewall show currentprofile",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return "Unknown";

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (output.Contains("Domain")) return "Domain";
                if (output.Contains("Private")) return "Private";
                if (output.Contains("Public")) return "Public";

                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
