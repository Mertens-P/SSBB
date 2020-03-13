using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetSerializer;

namespace ShootyShootyBangBang
{
    class Program
    {
        static Thread m_serverUpdateThread;
        static ShootyShootyBangBangGame m_serverGame = null;

        static void Main(string[] args)
        {
            InitializeSerializer();
            if (args.Contains("-Server"))
            {
                m_serverUpdateThread = new System.Threading.Thread(new System.Threading.ThreadStart(ServerThread));
                m_serverUpdateThread.Start();
            }

            if (args.Contains("-Client"))
            {
                var clPacketHandler = new Networking.Client.ClientPacketHandler();
                var clControllers = new ShootyShootyBangBangEngine.Controllers.ClientControllers(new LayeredRenderPipeline(), clPacketHandler);
                clPacketHandler.SetControllers(clControllers);
                using (ShootyShootyBangBangEngine.SSBBE engine = new ShootyShootyBangBangEngine.SSBBE(new ShootyShootyBangBangClientGame(clControllers), 800, 600, "ShootyShootyBangBang"))
                {
                    engine.Run(60.0);
                }
                if (m_serverGame != null)
                    m_serverGame.Stop();
            }
        }


        protected static void ServerThread()
        {
            System.Threading.Thread.CurrentThread.Name = "Server Update Thread";

            var packetHandler = new Networking.Server.ServerPacketHandler();
            var svControllers = new ShootyShootyBangBangEngine.Controllers.ServerControllers(packetHandler);
            packetHandler.SetControllers(svControllers);
            m_serverGame = new ShootyShootyBangBangServerGame(svControllers);
            Stopwatch svUpdateSw = new Stopwatch();
            long tickDuration = 0;
            double tarTickTime = 1.0 / 30.0;
            long tarFrameTimeInMs = (long)(tarTickTime * 1000.0);

            while (m_serverGame.GetisRunning())
            {
                svUpdateSw.Restart();
                m_serverGame.OnUpdateFrame(tarTickTime);

                tickDuration += svUpdateSw.ElapsedMilliseconds;
                if (tickDuration < tarFrameTimeInMs)
                {
                    Thread.Sleep((int)(tarFrameTimeInMs - tickDuration));
                    tickDuration = 0;
                }
                else
                    tickDuration -= tarFrameTimeInMs;
            }
            m_serverGame.UnLoad();
        }

        public static void InitializeSerializer()
        {
            var types = NetSerializable.GetTypesWithAttribute(System.Reflection.Assembly.GetExecutingAssembly()).ToList();
            if (System.Reflection.Assembly.GetCallingAssembly() != System.Reflection.Assembly.GetExecutingAssembly())
                types.AddRange(NetSerializable.GetTypesWithAttribute(System.Reflection.Assembly.GetCallingAssembly()).ToList());

            var serializerSettings = new Settings();
            //serializerSettings.CustomTypeSerializers = new ITypeSerializer[] { new SortedDictionarySerializer() };
            serializerSettings.SupportIDeserializationCallback = true;
            serializerSettings.SupportSerializationCallbacks = true;
            Serialization.s_globalSerializer = new NetSerializer.Serializer(types.ToArray(), serializerSettings);
        }
    }
}
