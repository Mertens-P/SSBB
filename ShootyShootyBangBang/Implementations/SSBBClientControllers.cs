
namespace ShootyShootyBangBang.Implementations
{
    class SSBBClientControllers : ShootyShootyBangBangEngine.Controllers.ClientControllers
    {
        SSBBRenderPipeline m_pipeline;
        Networking.Client.ClientPacketHandler m_packetHandler;

        public SSBBRenderPipeline GetSSBBRenderPipeline() { return m_pipeline; }
        public Networking.Client.ClientPacketHandler GetPacketHandler() { return m_packetHandler; }

        public SSBBClientControllers(SSBBRenderPipeline pipeline, Networking.Client.ClientPacketHandler packetHandler, string ip = "127.0.0.1", int port = 4805)
            :base(pipeline, packetHandler, ip, port)
        {
            m_pipeline = pipeline;
            m_packetHandler = packetHandler;
        }
    }
}
