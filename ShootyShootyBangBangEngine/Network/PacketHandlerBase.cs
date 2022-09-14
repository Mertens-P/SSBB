
namespace ShootyShootyBangBangEngine.Network
{
    public interface PacketHandlerBase
    {
        void Initialize(RPCDispatcher dispatcher);
    }

    public interface PacketHandlerServer : PacketHandlerBase
    {
        void OnConnect(object sender, long connectionId);
    }
}
