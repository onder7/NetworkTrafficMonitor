using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using NetworkTrafficMonitor.Models;

namespace NetworkTrafficMonitor.Services
{
    public class TrafficService
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, int reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedUdpTable(IntPtr pUdpTable, ref int dwOutBufLen, bool sort, int ipVersion, UDP_TABLE_CLASS tblClass, int reserved);

        private enum TCP_TABLE_CLASS
        {
            TCP_TABLE_OWNER_PID_ALL = 5
        }

        private enum UDP_TABLE_CLASS
        {
            UDP_TABLE_OWNER_PID = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public uint remoteAddr;
            public byte remotePort1;
            public byte remotePort2;
            public byte remotePort3;
            public byte remotePort4;
            public int owningPid;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_UDPROW_OWNER_PID
        {
            public uint localAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public int owningPid;
        }

        public List<ConnectionInfo> GetTcpConnections()
        {
            var connections = new List<ConnectionInfo>();
            int bufferSize = 0;
            
            // İlk çağrı - buffer size'ı öğren
            uint result1 = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, 2, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            
            if (bufferSize == 0)
                return connections;
            
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                uint result = GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, 2, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                
                if (result != 0)
                    return connections;

                int rowCount = Marshal.ReadInt32(tcpTablePtr);
                IntPtr rowPtr = (IntPtr)((long)tcpTablePtr + 4);

                for (int i = 0; i < rowCount; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);
                    
                    var localAddr = new IPAddress(row.localAddr);
                    var remoteAddr = new IPAddress(row.remoteAddr);
                    int localPort = (row.localPort1 << 8) + row.localPort2;
                    int remotePort = (row.remotePort1 << 8) + row.remotePort2;

                    string processName = GetProcessName(row.owningPid);
                    string state = GetTcpState(row.state);

                    connections.Add(new ConnectionInfo
                    {
                        LocalAddress = localAddr.ToString(),
                        LocalPort = localPort,
                        RemoteAddress = remoteAddr.ToString(),
                        RemotePort = remotePort,
                        Protocol = "TCP",
                        ProcessName = processName,
                        ProcessId = row.owningPid,
                        State = state,
                        Direction = DetermineDirection(localAddr, remoteAddr),
                        FirstSeen = DateTime.Now
                    });

                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf<MIB_TCPROW_OWNER_PID>());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return connections;
        }

        public List<ConnectionInfo> GetUdpConnections()
        {
            var connections = new List<ConnectionInfo>();
            int bufferSize = 0;
            
            // İlk çağrı - buffer size'ı öğren
            uint result1 = GetExtendedUdpTable(IntPtr.Zero, ref bufferSize, true, 2, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            
            if (bufferSize == 0)
                return connections;
            
            IntPtr udpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                uint result = GetExtendedUdpTable(udpTablePtr, ref bufferSize, true, 2, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                
                if (result != 0)
                    return connections;

                int rowCount = Marshal.ReadInt32(udpTablePtr);
                IntPtr rowPtr = (IntPtr)((long)udpTablePtr + 4);

                for (int i = 0; i < rowCount; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_UDPROW_OWNER_PID>(rowPtr);
                    
                    var localAddr = new IPAddress(row.localAddr);
                    int localPort = (row.localPort1 << 8) + row.localPort2;

                    string processName = GetProcessName(row.owningPid);

                    connections.Add(new ConnectionInfo
                    {
                        LocalAddress = localAddr.ToString(),
                        LocalPort = localPort,
                        RemoteAddress = "0.0.0.0",
                        RemotePort = 0,
                        Protocol = "UDP",
                        ProcessName = processName,
                        ProcessId = row.owningPid,
                        State = "Listening",
                        Direction = "Inbound",
                        FirstSeen = DateTime.Now
                    });

                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf<MIB_UDPROW_OWNER_PID>());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }

            return connections;
        }

        private string GetProcessName(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                return process.ProcessName;
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetTcpState(uint state)
        {
            return state switch
            {
                1 => "Closed",
                2 => "Listen",
                3 => "SYN_SENT",
                4 => "SYN_RCVD",
                5 => "Established",
                6 => "FIN_WAIT1",
                7 => "FIN_WAIT2",
                8 => "CLOSE_WAIT",
                9 => "CLOSING",
                10 => "LAST_ACK",
                11 => "TIME_WAIT",
                12 => "DELETE_TCB",
                _ => "Unknown"
            };
        }

        private string DetermineDirection(IPAddress local, IPAddress remote)
        {
            if (remote.ToString() == "0.0.0.0" || remote.ToString() == "127.0.0.1")
                return "Inbound";
            return "Outbound";
        }

        public async System.Threading.Tasks.Task<string> ResolveDomainAsync(string ipAddress)
        {
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                return hostEntry.HostName;
            }
            catch
            {
                return ipAddress;
            }
        }
    }
}
