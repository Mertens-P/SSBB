using OpenTK;
using ShootyShootyBangBangEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.Networking.ClientServer.NetPackets
{
    // server originated
    [Serializable, NetSerializable]
    public class CharacterUpdateClientPacket
    {
        public ShootyShootyBangBangEngine.GameObjects.Components.ComponentReplicator.ReplicationData replicationData;
    }
}
