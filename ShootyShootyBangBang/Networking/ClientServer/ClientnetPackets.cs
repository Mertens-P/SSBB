using ShootyShootyBangBangEngine.Helpers;
using System;

namespace ShootyShootyBangBang.Networking.ClientServer.NetPackets
{
    // server originated
    [Serializable, NetSerializable]
    public class RequestPlayerCharacterClientPacket
    {
        public GameObjects.Client.Ai.AiFactory.AiType aiType;
        public OpenTK.Vector2 position; //System.Vector2 doesn't want to serialize
    }
    // server originated
    [Serializable, NetSerializable]
    public class CharacterUpdateClientPacket
    {
        public ShootyShootyBangBangEngine.GameObjects.Components.ComponentReplicator.ReplicationData replicationData;
    }
}
