using ShootyShootyBangBangEngine.Helpers;
using System;
using System.Collections.Generic;

namespace ShootyShootyBangBang.Networking.ClientServer.NetPackets
{
    // server originated
    [Serializable, NetSerializable]
    public class SpawnPlayerServerPacket
    {
        public GameObjects.Client.Ai.AiFactory.AiType aiType;
        public Guid id;
        public OpenTK.Vector2 position; //System.Vector2 doesn't want to serialize
    }

    // server originated
    [Serializable, NetSerializable]
    public class ServerUpdatePacket
    {
        public List<ShootyShootyBangBangEngine.GameObjects.Components.ComponentReplicator.ReplicationData> ReplicationData;
    }
}
