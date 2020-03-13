using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.GameObjects.Client
{
    class PlayerControlledCharacter : ClientCharacter
    {
        public PlayerControlledCharacter(Guid id, RenderControllers controllers, Vector2 position, Vector2 dimensions, Texture texture, Shader shader)
            : base(controllers, position, dimensions, texture, shader, ComponentReplicator.PeerType.PT_Local)
        {
            SetId(id);
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            var clientControllers = controllers as ClientControllers;
            Vector2 dir = new Vector2();
            if (clientControllers.GetInput().IsKeyDown(Key.A)) dir.X = -1;
            if (clientControllers.GetInput().IsKeyDown(Key.D)) dir.X = 1;
            if (clientControllers.GetInput().IsKeyDown(Key.W)) dir.Y = 1;
            if (clientControllers.GetInput().IsKeyDown(Key.S)) dir.Y = -1;
            if (dir.LengthSquared > 0)
            {
                dir.Normalize();
                var transform = GetComponents().GetComponent<ComponentTransform>();
                if (transform != null)
                    transform.SetPosition(transform.GetPosition() + dir * m_movementSpeed * (float)dt);
                var replicator = GetComponents().GetComponent<ComponentReplicator>();
                clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.CharacterUpdateClientPacket() { replicationData = replicator.GetReplicationData(controllers, this) }, ShootyShootyBangBangEngine.Network.NetWrapOrdering.Unreliable);
            }
            base.OnUpdate(dt, controllers);
        }
    }
}
