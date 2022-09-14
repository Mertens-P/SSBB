using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Helpers;
using System;

namespace ShootyShootyBangBang.GameObjects.Components
{
    class CharacterReplicator : ComponentReplicator
    {
        [Serializable, NetSerializable]
        public class CharacterReplicationData : ComponentReplicator.ReplicationData
        {
            public OpenTK.Vector2 Position; //System.Vector2 doesn't want to serialize
            public float Angle;
        }

        public CharacterReplicator(PeerType peerType)
            :base(peerType)
        {
        }

        public override ReplicationData GetReplicationData(BaseControllers controllers, GameObject ownerEnt)
        {
            var transComp = ownerEnt.GetComponents().GetComponent<ComponentTransform>();
            return new CharacterReplicationData() { CharacterId = ownerEnt.GetId(), Position = MathHelpers.SytemVecToOpenTkVec(transComp.GetPosition()), Angle = transComp.GetAngle() };
        }

        public override void Tick(BaseControllers controllers, GameObject owner, double dt)
        {
            base.Tick(controllers, owner, dt);
            if (GetPeerType() != PeerType.PT_Remote) return;
        }

        public override void OnReplicate(BaseControllers controllers, GameObject ownerEnt, ComponentReplicator.ReplicationData replicationData)
        {
            if (GetPeerType() == PeerType.PT_Local) return;
            var characterReplicationData = replicationData as CharacterReplicationData;
            var transComp = ownerEnt.GetComponents().GetComponent<ComponentTransform>();
            transComp.SetPosition(MathHelpers.OpenTkVecToSystemVec(characterReplicationData.Position));
            transComp.SetAngle(characterReplicationData.Angle);
        }
    }
}
