using ShootyShootyBangBangEngine.Helpers;
using System;
using static ShootyShootyBangBangEngine.GameObjects.EntityComponentManager;

namespace ShootyShootyBangBangEngine.GameObjects.Components
{
    public abstract class ComponentReplicator : EntityComponentBase
    {
        public enum PeerType
        {
            PT_Local,
            PT_Remote,
            PT_Server
        }

        PeerType m_peerType;

        protected PeerType GetPeerType() { return m_peerType; }

        [Serializable, NetSerializable]
        public class ReplicationData
        {
            public Guid CharacterId;
        }
        public ComponentReplicator(PeerType peerType)    { m_peerType = peerType;  }

        public abstract ReplicationData GetReplicationData(Controllers.BaseControllers controllers, GameObject ownerEnt);
        public abstract void OnReplicate(Controllers.BaseControllers controllers, GameObject ownerEnt, ReplicationData replicationData);
    }
}
