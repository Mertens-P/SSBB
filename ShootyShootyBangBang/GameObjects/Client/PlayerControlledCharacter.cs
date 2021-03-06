﻿using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang.GameObjects.Client
{
    class PlayerControlledCharacter : ClientCharacter
    {
        Vector2 m_lastPos;
        float m_lastAngle;
        public PlayerControlledCharacter(Guid id, Implementations.SSBBRenderPipeline pipeline, Vector2 position, Vector2 dimensions, Texture texture, Shader shader)
            : base(pipeline, position, dimensions, texture, shader, ComponentReplicator.PeerType.PT_Local)
        {
            SetId(id);
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            var clientControllers = controllers as ClientControllers;
            Vector2 dir = new Vector2();
            var transform = GetComponents().GetComponent<ComponentTransform>();
            if (!GetComponents().HasComponent<ComponentAiSystem>())
            {
                if (clientControllers.GetkeyboardState().IsKeyDown(Key.A)) dir.X = -1;
                if (clientControllers.GetkeyboardState().IsKeyDown(Key.D)) dir.X = 1;
                if (clientControllers.GetkeyboardState().IsKeyDown(Key.W)) dir.Y = 1;
                if (clientControllers.GetkeyboardState().IsKeyDown(Key.S)) dir.Y = -1;
                if (dir.LengthSquared() > 0)
                {
                    dir = Vector2.Normalize(dir);
                    if (transform != null)
                        transform.SetPosition(transform.GetPosition() + dir * m_movementSpeed * (float)dt);
                }
                if (transform != null)
                {
                    var mousePos = clientControllers.GetMousePosInScreenSpace();
                    var lookDir = mousePos - transform.GetPosition();
                    lookDir = Vector2.Normalize(lookDir);
                    var ang = (float)Math.Acos(Vector2.Dot(lookDir, new Vector2(1.0f, 0)));
                    if (Vector2.Dot(lookDir, new Vector2(0, 1)) < 0)
                        ang = -ang;
                    transform.SetAngle(ang);
                }
            }
            if(i_isDirty(transform))
            {
                var replicator = GetComponents().GetComponent<ComponentReplicator>();
                clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.CharacterUpdateClientPacket() { replicationData = replicator.GetReplicationData(controllers, this) }, ShootyShootyBangBangEngine.Network.NetWrapOrdering.Unreliable);
                m_lastAngle = transform.GetAngle();
                m_lastPos = transform.GetPosition();
            }
            base.OnUpdate(dt, controllers);
        }

        bool i_isDirty(ComponentTransform trans)
        {
            return trans.GetPosition() != m_lastPos || m_lastAngle != trans.GetAngle();
        }
    }
}
