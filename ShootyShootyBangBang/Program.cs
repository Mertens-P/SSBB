using ShootyShootyBangBangEngine.Helpers;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Numerics;
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
            CommandLineParser commandLineParser = new CommandLineParser();
            commandLineParser.Parse(args);

            int port = 4805;
            if (!int.TryParse(commandLineParser.GetValueForArgument("-port"), out port))
                port = 4805;

            InitializeSerializer();
            if (commandLineParser.IsArgumentSet("-Server"))
            {
                m_serverUpdateThread = new Thread(new ThreadStart(() => { ServerThread(port); }));
                m_serverUpdateThread.Start();
            }

            if (commandLineParser.IsArgumentSet("-Client"))
            {
                var clPacketHandler = new Networking.Client.ClientPacketHandler();
                string ip = commandLineParser.GetValueForArgument("-ip");
                if(string.IsNullOrEmpty(ip))
                    ip = "127.0.0.1";

                var clControllers = new Implementations.SSBBClientControllers(new Implementations.SSBBRenderPipeline(), clPacketHandler, ip, port);
                clPacketHandler.SetControllers(clControllers);

                var aiType = i_getAiType(commandLineParser);
                string titleAppendix = "";
                if (aiType != GameObjects.Client.Ai.AiFactory.AiType.AT_None)
                    titleAppendix = " " + aiType.ToString();

                using (ShootyShootyBangBangEngine.SSBBE engine = new ShootyShootyBangBangEngine.SSBBE(new ShootyShootyBangBangClientGame(clControllers, aiType), 800, 600, "ShootyShootyBangBang"+ titleAppendix))
                {
                    i_parseWindowPosition(engine, commandLineParser);
                    if (commandLineParser.IsArgumentSet("-AllBots"))
                        i_createAllBots(engine.GetWindowXPos(), engine.GetWindowYPos(), 20, 20);
                    engine.Run(60.0);
                }
                if (m_serverGame != null)
                    m_serverGame.Stop();
            }
        }

        protected static void ServerThread(int port)
        {
            Thread.CurrentThread.Name = "Server Update Thread";

            var packetHandler = new Networking.Server.ServerPacketHandler();
            var svControllers = new ShootyShootyBangBangEngine.Controllers.ServerControllers(packetHandler, "ShootyShootyBangBang", port, 64, false);
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

        static GameObjects.Client.Ai.AiFactory.AiType i_getAiType(CommandLineParser commandLineParser)
        {
            var botTypeStr = commandLineParser.GetValueForArgument("-BotType");
            if (!string.IsNullOrEmpty(botTypeStr) && Enum.TryParse<GameObjects.Client.Ai.AiFactory.AiType>(botTypeStr, out var botType))
                return botType;
            return GameObjects.Client.Ai.AiFactory.AiType.AT_None;
        }

        static void i_parseWindowPosition(ShootyShootyBangBangEngine.SSBBE engine, CommandLineParser commandLineParser)
        {
            var xLocStr = commandLineParser.GetValueForArgument("-WndXPos");
            var yLocStr = commandLineParser.GetValueForArgument("-WndYPos");
            int xPos = engine.GetWindowXPos();
            int yPos = engine.GetWindowYPos();
            if (xLocStr != null && int.TryParse(xLocStr, out int xPosParsed))
                xPos = xPosParsed;
            if (yLocStr != null && int.TryParse(yLocStr, out int yPosParsed))
                yPos = yPosParsed;
            engine.SetWindowLocation(xPos, yPos);
        }

        static void i_createAllBots(int selfXPosition, int selfYPosition, int xOffsetPerWindow, int yOffsetPerWindow)
        {
            int i = 0;
            foreach (var botType in Enum.GetValues(typeof(GameObjects.Client.Ai.AiFactory.AiType)))
            {
                if ((GameObjects.Client.Ai.AiFactory.AiType)botType == GameObjects.Client.Ai.AiFactory.AiType.AT_None) continue;
                i++;
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start(dir + "ShootyShootyBangBang.exe", $"-Client -BotType {botType.ToString()} -WndXPos {selfXPosition + i * xOffsetPerWindow} -WndYPos {selfYPosition + i * yOffsetPerWindow}");
            }
        }
    }
}
