using ShootyShootyBangBangEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Networking.ClientServer.NetPackets
{
    [Serializable, NetSerializable]
    public class HelloWorldPacket
    {
        public int SomeInt;
    }
}
