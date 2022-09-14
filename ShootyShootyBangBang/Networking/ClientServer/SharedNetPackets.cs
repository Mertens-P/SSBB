using ShootyShootyBangBangEngine.Helpers;
using System;

namespace ShootyShootyBangBang.Networking.ClientServer.NetPackets
{
    [Serializable, NetSerializable]
    public class HelloWorldPacket
    {
        public int SomeInt;
    }
}
