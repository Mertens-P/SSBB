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
        static void Main(string[] args)
        {
            InitializeSerializer();
            m_serverUpdateThread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateOffthreadLoop));
            m_serverUpdateThread.Start();

            var clPacketHandler = new Networking.Client.ClientPacketHandler();
            var clControllers = new ShootyShootyBangBangEngine.Controllers.ClientControllers(new LayeredRenderPipeline(), clPacketHandler);
            clPacketHandler.SetControllers(clControllers);
            using (ShootyShootyBangBangEngine.SSBBE engine = new ShootyShootyBangBangEngine.SSBBE(new ShootyShootyBangBangGame(clControllers), 800, 600, "ShootyShootyBangBang"))
            {
                engine.Run(60.0);
            }
        }


        protected static void UpdateOffthreadLoop()
        {
            System.Threading.Thread.CurrentThread.Name = "Server Update Thread";

            var packetHandler = new Networking.Server.ServerPacketHandler();
            var svControllers = new ShootyShootyBangBangEngine.Controllers.ServerControllers(packetHandler);
            packetHandler.SetControllers(svControllers);
            var svGame = new ShootyShootyBangBangGame(svControllers);
            Stopwatch svUpdateSw = new Stopwatch();
            long tickDuration = 0;
            double tarTickTime = 1.0 / 60.0;
            long tarFrameTimeInMs = (long)(tarTickTime * 1000.0);

            while (svGame.GetisRunning())
            {
                svUpdateSw.Restart();
                svGame.OnUpdateFrame(tarTickTime);

                tickDuration += svUpdateSw.ElapsedMilliseconds;
                if (tickDuration < tarFrameTimeInMs)
                {
                    Thread.Sleep((int)(tarFrameTimeInMs - tickDuration));
                    tickDuration = 0;
                }
                else
                    tickDuration -= tarFrameTimeInMs;
            }
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
