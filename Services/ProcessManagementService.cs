using System;
using System.Diagnostics;
using System.Linq;

namespace NetworkTrafficMonitor.Services
{
    public class ProcessManagementService
    {
        public ProcessInfo GetProcessInfo(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return new ProcessInfo
                {
                    ProcessId = processId,
                    ProcessName = process.ProcessName,
                    MainWindowTitle = process.MainWindowTitle,
                    StartTime = process.StartTime,
                    WorkingSet = process.WorkingSet64,
                    IsResponding = process.Responding,
                    HasExited = process.HasExited
                };
            }
            catch (Exception ex)
            {
                return new ProcessInfo
                {
                    ProcessId = processId,
                    ProcessName = "Unknown",
                    ErrorMessage = ex.Message
                };
            }
        }

        public bool KillProcess(int processId, bool force = false)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                
                if (force)
                {
                    // Önce normal kill dene
                    try
                    {
                        process.Kill(entireProcessTree: true);
                        process.WaitForExit(2000);
                        
                        if (!process.HasExited)
                        {
                            // Hala çalışıyorsa taskkill kullan
                            return KillProcessWithTaskkill(processId);
                        }
                    }
                    catch
                    {
                        // Normal kill başarısız, taskkill dene
                        return KillProcessWithTaskkill(processId);
                    }
                }
                else
                {
                    process.CloseMainWindow();
                    
                    // 5 saniye bekle, kapanmazsa zorla kapat
                    if (!process.WaitForExit(5000))
                    {
                        process.Kill(entireProcessTree: true);
                        process.WaitForExit(2000);
                        
                        if (!process.HasExited)
                        {
                            return KillProcessWithTaskkill(processId);
                        }
                    }
                }
                
                return true;
            }
            catch
            {
                // Son çare: taskkill
                return KillProcessWithTaskkill(processId);
            }
        }

        private bool KillProcessWithTaskkill(int processId)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/F /PID {processId} /T",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(psi);
                if (process == null) return false;

                process.WaitForExit(5000);
                
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                
                // Taskkill completed

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public bool KillProcessByName(string processName, bool force = false)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                
                if (processes.Length == 0)
                    return false;

                foreach (var process in processes)
                {
                    try
                    {
                        if (force)
                        {
                            process.Kill(entireProcessTree: true);
                        }
                        else
                        {
                            process.CloseMainWindow();
                            if (!process.WaitForExit(5000))
                            {
                                process.Kill(entireProcessTree: true);
                            }
                        }
                    }
                    catch
                    {
                        // Devam et, diğer process'leri dene
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsSystemProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                
                // Sistem process'leri
                string[] systemProcesses = 
                {
                    "System", "svchost", "csrss", "smss", "wininit", "services",
                    "lsass", "winlogon", "explorer", "dwm", "RuntimeBroker"
                };

                return systemProcesses.Any(sp => 
                    process.ProcessName.Equals(sp, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return true; // Hata varsa güvenli tarafta kal
            }
        }

        public string GetProcessPath(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return process.MainModule?.FileName ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }

    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string MainWindowTitle { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public long WorkingSet { get; set; }
        public bool IsResponding { get; set; }
        public bool HasExited { get; set; }
        public string? ErrorMessage { get; set; }

        public string WorkingSetFormatted
        {
            get
            {
                if (WorkingSet < 1024) return $"{WorkingSet} B";
                if (WorkingSet < 1024 * 1024) return $"{WorkingSet / 1024.0:F2} KB";
                if (WorkingSet < 1024 * 1024 * 1024) return $"{WorkingSet / (1024.0 * 1024):F2} MB";
                return $"{WorkingSet / (1024.0 * 1024 * 1024):F2} GB";
            }
        }
    }
}
