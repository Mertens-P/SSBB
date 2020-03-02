using ShootyShootyBangBangEngine.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class ServerControllers : BaseControllers
    {

        public enum LoginDeniedErrorCode
        {
            LDEC_NoCode,
            LDEC_DeniedByServer,
            LDEC_UserKickedByAdmin,
            LDEC_UserBannedByAdmin,
            LDEC_ServerRestart,
            LDEC_ConnectionLost,
            LDEC_ServerError,
            LDEC_InvalidPassword,
            LDEC_NetServerStatusChanged
        }

        public event EventHandler<long> OnConnect = delegate { };
        public event EventHandler<long> OnDisconnect = delegate { };

        NetServer m_netServer = null;

        Thread m_packetRecieverThread;
        Stopwatch m_dtTimer = new Stopwatch();
        long m_dtLastTick = 0;
        public float TickrateInSeconds { get { return 1.0f / 100.0f; } }
        public long TickrateInMilliseconds { get { return (long)Math.Ceiling(TickrateInSeconds * 1000.0f); } }

        public NetServer Net
        {
            get { return m_netServer; }
        }

        public void Connect(long connectionId)
        {
            OnConnect(this, connectionId);
        }

        public void Disconnect(long connectionId, LoginDeniedErrorCode errorCode = LoginDeniedErrorCode.LDEC_NoCode, string errorString = "")
        {
            Console.WriteLine(string.Format("Disconnecting player: {0}, reason: {1}", connectionId, errorCode));
            OnDisconnect(this, connectionId);
            Net.DisconnectConnection(connectionId);
        }

        public ServerControllers(PacketHandlerBase packetHandler)
        {
            try
            {
                m_netServer = new NetServer("CardGame", 4805, 64, false);
                m_netServer.OnStatusChanged += netServer_OnStatusChanged;
                m_netServer.OnMessage += netServer_OnMessage;
                packetHandler.Initialize(m_netServer.RpcDispatcher);
                Console.WriteLine("Server initiate success!!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to initate server!! e: " + e);
            }
        }

        public int GetPort()
        {
            return m_netServer.GetPort();
        }

        void netServer_OnMessage(object sender, string e)
        {
            Console.WriteLine("Server (Internal Message): " + e);
        }

        void netServer_OnStatusChanged(object s, NetWrapConnectionStatus e)
        {
            var sender = e.EndPoint;
            switch (e.Status)
            {
                case NetWrapConnectionStatusEnum.Connected:
                    Connect(e.ConnectionId);
                    break;
                case NetWrapConnectionStatusEnum.Disconnected:
                    Disconnect(e.ConnectionId, LoginDeniedErrorCode.LDEC_NetServerStatusChanged);
                    break;
                case NetWrapConnectionStatusEnum.TimedOut:
                    Disconnect(e.ConnectionId, LoginDeniedErrorCode.LDEC_ConnectionLost);
                    break;
            }
        }

        public void Flush()
        {
            if (m_netServer != null)
                m_netServer.Flush();
        }

        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            if (m_netServer != null)
                m_netServer.Update();

            if (m_packetRecieverThread == null)
            {
                m_packetRecieverThread = new System.Threading.Thread(new System.Threading.ThreadStart(i_updateOffthreadLoop));
                m_packetRecieverThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                m_packetRecieverThread.Start();
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();
            if (m_netServer != null)
                m_netServer.UPnPCleanup();
        }


        protected void i_updateOffthreadLoop()
        {
            System.Threading.Thread.CurrentThread.Name = "Server Update Thread";
#if !DEBUG
            try
            {
#endif
            while (true)
            {
                // slumber before next tick to go easy on the CPU
                long waitFor = (m_dtLastTick + TickrateInMilliseconds) - m_dtTimer.ElapsedMilliseconds;
                if (waitFor <= 0)
                    waitFor = 1;
                i_waitForPacket((int)waitFor);
            }
#if !DEBUG
            }
            catch(ThreadAbortException e)
            {
                log.Warn(string.Format("Thread was aborted, notify developers if you weren't closing the server or leaving a game, stack trace: {0}", e.StackTrace), "Main");
                LogHelpers.LogInnerExceptionStackTraces("Main", e, log.Warn);
            }
            catch(Exception e)
            {
                log.Fatal("Server crash!\n" + e.StackTrace, "Exceptions", e);
                LogHelpers.LogInnerExceptions("Exceptions", e, log.Fatal);
                foreach (var player in m_controllers.Players.AllPlayers())
                    player.SendLoginDenied(LoginDeniedErrorCode.LDEC_ServerError, "Server Crashed!");
                if (Program.CommandLineArgs.Contains("-server"))
                    throw e;
                else
                    File.Create(string.Format("{0}.crashed", Process.GetCurrentProcess().Id)).Close();
            }
#endif
        }

        void i_waitForPacket(int millisecondsTimeout)
        {
            if (m_netServer != null)
                m_netServer.WaitForPacket(millisecondsTimeout);
        }
    }
}
