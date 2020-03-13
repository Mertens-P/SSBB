using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ShootyShootyBangBangEngine.Helpers.Threading;
using ShootyShootyBangBangEngine.Helpers;

namespace ShootyShootyBangBangEngine.Network
{
    public enum NetWrapConnectionStatusEnum
    {
        None,
        Connected,
        Disconnected,
        TimedOut
    }

    public struct NetWrapConnectionStatus
    {
        public NetWrapConnectionStatusEnum Status;
        public IPEndPoint EndPoint;
        public long ConnectionId;
    }

    public enum NetWrapOrdering
    {
        Unknown = 0,
        Unreliable = 1,
        UnreliableSequenced = 2,
        ReliableUnordered = 34,
        ReliableSequenced = 35,
        ReliableOrdered = 67,
    }

    public struct NetWrapOutgoingMessage
    {
        public NetWrapOutgoingMessage(NetWrapOutgoingMessage msg)
        {
            Message = msg.Message;
            MessageOffset = msg.MessageOffset;
            MessageCount = msg.MessageCount;
            ConnectionId = msg.ConnectionId;
            Flags = msg.Flags;
        }

        public byte[] Message;
        public int MessageOffset;
        public int MessageCount;
        public long ConnectionId;
        public ENet.PacketFlags Flags;
    }

    public struct NetWrapIncomingMessage
    {
        public byte[] Message;
        public int MessageOffset;
        public int MessageCount;
        public long SenderConnectionId;
    }

    public class NetClientServer
    {
        private struct NetWrapConnection
        {
            public long ConnectionId;
            public ENet.Peer Peer;
        }

        ~NetClientServer()
        {
            Shutdown("Goodbye.");
        }


        public int NumConnections = 0;
        protected ENet.Host host;
        protected readonly LockObject hostLock = new LockObject();
        protected ENet.Peer peer;
        protected int listenPort;
        protected int timeout = 0;

        protected bool m_valid = true;

        public RPCDispatcher RpcDispatcher = new RPCDispatcher();

        public event EventHandler OnUpdate = delegate { };
        public event EventHandler<string> OnMessage = delegate { };
        public event EventHandler<NetWrapConnectionStatus> OnStatusChanged = delegate { };
        public event EventHandler<NetWrapIncomingMessage> OnData = delegate { };
        public event EventHandler<NetWrapIncomingMessage> OnUnconnectedData = delegate { };
        public event EventHandler<NetWrapIncomingMessage> OnPacket = delegate { };

        List<ENet.Event>[] m_enetEventLists = new List<ENet.Event>[2] { new List<ENet.Event>(), new List<ENet.Event>() };
        readonly LockObject[] m_enetEventListLocks = new LockObject[2] { new LockObject(), new LockObject() };
        List<NetWrapOutgoingMessage>[] m_outgoingPacketLists = new List<NetWrapOutgoingMessage>[2] { new List<NetWrapOutgoingMessage>(), new List<NetWrapOutgoingMessage>() };
        readonly LockObject[] m_outgoingPacketListLocks = new LockObject[2] { new LockObject(), new LockObject() };
        List<long> m_disconnectionList = new List<long>();
        readonly LockObject m_disconnectionListLock = new LockObject();
        int m_currentENetEventListIndex = 0;
        int m_currentOutgoingPacketListIndex = 0;
        Task m_ENetTask = null;
        ManualResetEvent m_threadStoppedEvent = new ManualResetEvent(false);
        bool m_stopThread = false;

        ConcurrentDictionary<long, NetWrapConnection> m_connections = new ConcurrentDictionary<long, NetWrapConnection>();

        static byte[] RPCHeader = new byte[] { 0, (byte)'R', (byte)'p', (byte)'C' };

        public NetWrapOutgoingMessage CreateRPCPacket(params object[] objs)
        {
            NetWrapOutgoingMessage msg = new NetWrapOutgoingMessage();
            List<byte> packet = new List<byte>();
            packet.AddRange(RPCHeader);
            packet.Add(255);

            int count = 0;
            foreach (var obj in objs)
            {
                var stream = new MemoryStream(2048);
                RPCStream.AddCall(stream, obj);
                var array = stream.ToArray();
                if (array.Length > 512)
                {
                    packet.AddRange(BitConverter.GetBytes(-array.Length));
                    packet.AddRange(NetCompressByteArray.Wrap(array));

                }
                else
                {
                    packet.AddRange(BitConverter.GetBytes(array.Length));
                    packet.AddRange(array);
                }
                count++;
            }

            // set count to correct count (255 -> count)
            packet[4] = (byte)count;

            msg.Message = packet.ToArray();
            msg.MessageCount = packet.Count;
            msg.MessageOffset = 0;
            return msg;
        }
        
        public void Flush()
        {
            using (MonitorLock.CreateLock(hostLock))
            {
                host.Flush();
            }
        }

        long connectionIDCounter = 1;
        public bool Update()
        {
            if (m_valid == false)
                return false;

            i_checkStartENetThread();

            using (MonitorLock.CreateLock(m_outgoingPacketListLocks[m_currentOutgoingPacketListIndex]))
            {
                using (MonitorLock.CreateLock(m_outgoingPacketListLocks[(m_currentOutgoingPacketListIndex + 1) % 2]))
                {
                    if (m_outgoingPacketLists[(m_currentOutgoingPacketListIndex + 1) % 2].Count > 0 && m_outgoingPacketLists[m_currentOutgoingPacketListIndex].Count == 0)
                        m_currentOutgoingPacketListIndex = (m_currentOutgoingPacketListIndex + 1) % 2;
                }
            }

            using (MonitorLock.CreateLock(m_enetEventListLocks[m_currentENetEventListIndex]))
            {
                if (m_enetEventLists[m_currentENetEventListIndex].Count > 0)
                {
                    m_currentENetEventListIndex = (m_currentENetEventListIndex + 1) % 2;
                    m_enetEventLists[m_currentENetEventListIndex].Clear();
                }
            }

            using (MonitorLock.CreateLock(m_enetEventListLocks[m_currentENetEventListIndex]))
            {
                foreach (var enetEvent in m_enetEventLists[(m_currentENetEventListIndex + 1) % 2])
                {
                    if (enetEvent.Type == ENet.EventType.None)
                        continue;

                    NetWrapConnection connection;
                    if (enetEvent.Peer.UserData == IntPtr.Zero)
                    {
                        var peer = enetEvent.Peer;
                        connection = new NetWrapConnection
                        {
                            ConnectionId = connectionIDCounter++,
                            Peer = enetEvent.Peer
                        };
                        m_connections.TryAdd(connection.ConnectionId, connection);
                        peer.UserData = GCHandle.ToIntPtr(GCHandle.Alloc(connection));
                    }
                    else
                    {
                        var gcHandle = GCHandle.FromIntPtr(enetEvent.Peer.UserData);
                        connection = (NetWrapConnection)gcHandle.Target;
                        m_connections.GetOrAdd(connection.ConnectionId, connection);
                    }

                    switch (enetEvent.Type)
                    {
                        case ENet.EventType.Connect:
                            {
                                // set timeouts
                                enetEvent.Peer.SetTimeouts(5, 30000, 90000);

                                NumConnections++;

                                var wrappedStatus = new NetWrapConnectionStatus()
                                {
                                    Status = NetWrapConnectionStatusEnum.Connected,
                                    EndPoint = enetEvent.Peer.GetRemoteAddress(),
                                    ConnectionId = connection.ConnectionId
                                };
                                OnStatusChanged(this, wrappedStatus);
                            }
                            break;
                        case ENet.EventType.Disconnect:
                            {
                                NumConnections--;

                                var wrappedStatus = new NetWrapConnectionStatus()
                                {
                                    Status = NetWrapConnectionStatusEnum.Disconnected,
                                    EndPoint = enetEvent.Peer.GetRemoteAddress(),
                                    ConnectionId = connection.ConnectionId
                                };
                                OnStatusChanged(this, wrappedStatus);
                            }
                            break;
                        case ENet.EventType.Receive:
                            byte[] data = enetEvent.Packet.GetBytes();
                            if (data.Length >= 5 && data[0] == RPCHeader[0] && data[1] == RPCHeader[1] && data[2] == RPCHeader[2] && data[3] == RPCHeader[3])
                            {
#if RELEASE // Needs to be #if-ed out in debug to let errors fall into the debugger
                            try
                            {
#endif
                                var rpcDataTemplate = new RPCData();
                                var msg = new NetWrapIncomingMessage();
                                msg.SenderConnectionId = connection.ConnectionId;
                                rpcDataTemplate.OriginalMessage = msg;

                                int offset = 5;
                                for (int i = 0; i < data[4]; i++)
                                {
                                    int blockbytes = BitConverter.ToInt32(data, offset);
                                    offset += 4;
                                    if (blockbytes < 0)
                                    {

                                        // compressed RPC call
                                        blockbytes = -blockbytes;
                                        var buffer = NetCompressByteArray.Unwrap(data, offset);
                                        var stream = new MemoryStream(buffer);
                                        RPCStream.Execute(stream, RpcDispatcher, rpcDataTemplate);

                                    }
                                    else
                                    {
                                        // uncompressed RPC call
                                        var stream = new MemoryStream(data, offset, blockbytes);
                                        RPCStream.Execute(stream, RpcDispatcher, rpcDataTemplate);
                                    }
                                    offset += blockbytes;
                                }
#if RELEASE
                            }
                            catch(Exception ex)
                            {
                                try
                                {
                                    using (var stream = System.IO.File.AppendText("ignored-net-errors.txt"))
                                        stream.WriteLine(DateTime.Now.ToString() + " : " + ex.ToString());
                                } catch {}

                                Console.WriteLine(ex.ToString());
                                break;
                            }
#endif
                            }
                            else
                            {
                                NetWrapIncomingMessage msg = new NetWrapIncomingMessage()
                                {
                                    Message = data,
                                    MessageCount = data.Length,
                                    MessageOffset = 0,
                                    SenderConnectionId = connection.ConnectionId
                                };
                                OnData(this, msg);
                            }
                            enetEvent.Packet.Dispose();
                            break;

                        default:
                            Console.WriteLine(enetEvent.Type);
                            break;
                    }
                }
            m_enetEventLists[(m_currentENetEventListIndex + 1) % 2].Clear();
            }
            return true;
        }

        public int GetPort()
        {
            return listenPort;
        }

        public IPEndPoint GetConnectionEndPoint(long connectionId)
        {
            NetWrapConnection connection;
            if (!m_connections.TryGetValue(connectionId, out connection))
                return null;

            return connection.Peer.GetRemoteAddress();
        }

        public void Shutdown(string p)
        {
            if (m_valid == false)
                return;

            if (peer.IsInitialized)
                peer.DisconnectNow(p.GetHashCode());
            using (MonitorLock.CreateLock(hostLock))
            {
                if (host.IsInitialized)
                {
                    m_stopThread = true;
                    m_threadStoppedEvent.WaitOne(5000);
                    try
                    {
                        host.Dispose();
                        m_valid = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to shut down ENet e:" + e);
                    }
                }
            }
        }

        public void SendPacket(NetWrapOutgoingMessage msg, long connectionId, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            if (m_valid == false)
                return;

            var flags = ENet.PacketFlags.None;
            if((int)delivery >= 30)
                flags |= ENet.PacketFlags.Reliable;
            if(delivery == NetWrapOrdering.ReliableUnordered)
                flags |= ENet.PacketFlags.Unsequenced;

            msg.ConnectionId = connectionId;
            msg.Flags = flags;
            using (MonitorLock.CreateLock(m_outgoingPacketListLocks[(m_currentOutgoingPacketListIndex + 1) % 2]))
            {
                m_outgoingPacketLists[(m_currentOutgoingPacketListIndex + 1) % 2].Add(msg);
            }
        }

        public void SendPacket(NetWrapOutgoingMessage msg, List<long> connectionIds, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            if (m_valid == false)
                return;

            var flags = ENet.PacketFlags.None;
            if ((int)delivery >= 30)
                flags |= ENet.PacketFlags.Reliable;
            if (delivery == NetWrapOrdering.ReliableUnordered)
                flags |= ENet.PacketFlags.Unsequenced;
            msg.Flags = flags;
            foreach (var connectionId in connectionIds)
            {
                var toSendMsg = new NetWrapOutgoingMessage(msg);
                toSendMsg.ConnectionId = connectionId;
                using (MonitorLock.CreateLock(m_outgoingPacketListLocks[(m_currentOutgoingPacketListIndex + 1) % 2]))
                {
                    m_outgoingPacketLists[(m_currentOutgoingPacketListIndex + 1) % 2].Add(toSendMsg);
                }
            }
        }

        public void SendRPC(object RPCObject, long connectionId, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            SendPacket(CreateRPCPacket(RPCObject), connectionId, delivery, sequenceChannel);
        }

        public void SendRPC(object RPCObject, List<long> connectionIds, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            if (connectionIds.Count == 0)
                return;

            SendPacket(CreateRPCPacket(RPCObject), connectionIds, delivery, sequenceChannel);
        }

        public void SendRPC(List<object> RPCObject, int connectionId, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            SendPacket(CreateRPCPacket(RPCObject), connectionId, delivery, sequenceChannel);
        }

        public void SendRPC(List<object> RPCObject, List<long> connectionIds, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            if (connectionIds.Count == 0)
                return;

            SendPacket(CreateRPCPacket(RPCObject), connectionIds, delivery, sequenceChannel);
        }

        public void BroadCastRPC(object RPCObject, List<long> exclusionIds, NetWrapOrdering delivery = NetWrapOrdering.ReliableOrdered, int sequenceChannel = 0)
        {
            List<long> sendIds = new List<long>();
            foreach(var connection in m_connections)
            {
                if (!exclusionIds.Contains(connection.Key))
                    sendIds.Add(connection.Key);
            }
            SendRPC(RPCObject, sendIds, delivery, sequenceChannel);
        }

        public void WaitForPacket(int millisecondsTimeout)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < millisecondsTimeout)
            {
                Thread.Sleep(1);
                if (Update() == true)
                    break;
            }
            
        }

        public void DisconnectConnection(long connectionId)
        {
            using (MonitorLock.CreateLock(m_disconnectionListLock))
            {
                m_disconnectionList.Add(connectionId);
            }
        }

        private void i_checkStartENetThread()
        {
            if (m_ENetTask == null || m_ENetTask.IsCompleted)
            {
                m_ENetTask = Task.Run(() =>
                {
                    Thread.CurrentThread.Name = "ENet Thread";
                    while (!m_stopThread || m_disconnectionList.Count != 0)
                    {
                        using (MonitorLock.CreateLock(m_outgoingPacketListLocks[m_currentOutgoingPacketListIndex]))
                        {
                            foreach (var outgoingPacket in m_outgoingPacketLists[m_currentOutgoingPacketListIndex])
                            {
                                NetWrapConnection connection;
                                if (!m_connections.TryGetValue(outgoingPacket.ConnectionId, out connection))
                                    continue;

                                if (connection.Peer.State == ENet.PeerState.Connected)
                                    connection.Peer.Send(0, outgoingPacket.Message, outgoingPacket.MessageOffset, outgoingPacket.MessageCount, outgoingPacket.Flags);
                            }
                            m_outgoingPacketLists[m_currentOutgoingPacketListIndex].Clear();
                        }

                        using (MonitorLock.CreateLock(m_disconnectionListLock))
                        {
                            foreach (var connectionId in m_disconnectionList)
                            {
                                NetWrapConnection connection;
                                if (m_connections.TryRemove(connectionId, out connection) && connection.Peer.UserData != IntPtr.Zero)
                                {
                                    GCHandle.FromIntPtr(connection.Peer.UserData).Free();
                                    connection.Peer.UserData = IntPtr.Zero;
                                }
                                if (connection.Peer.IsInitialized && connection.Peer.State == ENet.PeerState.Connected)
                                    connection.Peer.DisconnectLater("disconnect".GetHashCode());
                            }
                            m_disconnectionList.Clear();
                        }

                        using (MonitorLock.CreateLock(m_enetEventListLocks[m_currentENetEventListIndex]))
                        {
                            ENet.Event enetEvent;
                            using (MonitorLock.CreateLock(hostLock))
                            {
                                if (host.Service(0, out enetEvent))
                                {
                                    do
                                    {
                                        m_enetEventLists[m_currentENetEventListIndex].Add(enetEvent);
                                    } while (host.CheckEvents(out enetEvent));
                                }
                            }
                        }

                        Thread.Sleep(25);
                    }

                    m_threadStoppedEvent.Set();
                });
            }
        }
    }
}
