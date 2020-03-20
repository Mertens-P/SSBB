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
            if (clientControllers.GetkeyboardState().IsKeyDown(Key.A)) dir.X = -1;
            if (clientControllers.GetkeyboardState().IsKeyDown(Key.D)) dir.X = 1;
            if (clientControllers.GetkeyboardState().IsKeyDown(Key.W)) dir.Y = 1;
            if (clientControllers.GetkeyboardState().IsKeyDown(Key.S)) dir.Y = -1;
            bool dirty = false;
            var transform = GetComponents().GetComponent<ComponentTransform>();
            if (dir.LengthSquared > 0)
            {
                dir.Normalize();
                if (transform != null)
                    transform.SetPosition(transform.GetPosition() + dir * m_movementSpeed * (float)dt);
                dirty = true;
            }
            if (transform != null)
            {
                var mousePos = clientControllers.GetMousePosInScreenSpace();
                var lookDir = mousePos - transform.GetPosition();
                lookDir.Normalize();
                var ang = (float)Math.Acos(Vector2.Dot(lookDir, new Vector2(1.0f, 0)));
                if (Vector2.Dot(lookDir, new Vector2(0, 1)) < 0)
                    ang = -ang;
                dirty = dirty || transform.GetAngle() != ang;
                transform.SetAngle(ang);
            }
            if(dirty)
            {
                var replicator = GetComponents().GetComponent<ComponentReplicator>();
                clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.CharacterUpdateClientPacket() { replicationData = replicator.GetReplicationData(controllers, this) }, ShootyShootyBangBangEngine.Network.NetWrapOrdering.Unreliable);
            }
            base.OnUpdate(dt, controllers);
        }
    }
}
