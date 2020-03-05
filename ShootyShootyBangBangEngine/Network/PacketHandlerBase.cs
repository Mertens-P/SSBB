using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
