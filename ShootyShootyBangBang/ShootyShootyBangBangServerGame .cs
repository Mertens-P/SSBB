using System.Collections.Generic;

namespace ShootyShootyBangBang
{
    class ShootyShootyBangBangServerGame : ShootyShootyBangBangGame
    {
        ShootyShootyBangBangEngine.Controllers.ServerControllers m_serverControllers;

        public ShootyShootyBangBangServerGame(ShootyShootyBangBangEngine.Controllers.ServerControllers controllers)
            :base(controllers)
        {
            m_serverControllers = controllers;
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnUpdateFrame(double dt)
        {
            base.OnUpdateFrame(dt);
            var packet = new Networking.ClientServer.NetPackets.ServerUpdatePacket() { ReplicationData = new List<ShootyShootyBangBangEngine.GameObjects.Components.ComponentReplicator.ReplicationData>() };
            foreach(var ent in m_serverControllers.GetRootScene().GetGameObjects())
            {
                var replicator = ent.GetComponents().GetComponent<ShootyShootyBangBangEngine.GameObjects.Components.ComponentReplicator>();
                if (replicator != null)
                    packet.ReplicationData.Add(replicator.GetReplicationData(m_controllers, ent));
            }
            m_serverControllers.Net.BroadCastRPC(packet, new List<long>(), ShootyShootyBangBangEngine.Network.NetWrapOrdering.Unreliable);
        }
    }
}
