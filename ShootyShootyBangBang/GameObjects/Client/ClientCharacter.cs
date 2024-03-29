﻿using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Numerics;

namespace ShootyShootyBangBang.GameObjects.Client
{
    class ClientCharacter : ClientServer.SSBBCharacter
    {
        TexturedQuad m_visual;
        public ClientCharacter(Implementations.SSBBRenderPipeline pipeline, Vector2 position, Vector2 dimensions, Texture texture, Shader shader, ComponentReplicator.PeerType peertype)
            :base(position, peertype)
        {
            m_visual = new TexturedQuad(dimensions, texture, shader);
            pipeline.AddRenderable(m_visual, Implementations.SSBBRenderPipeline.LayerIdentifiers.LI_Characters, 0);
        }

        public ClientCharacter(Guid id, Implementations.SSBBRenderPipeline pipeline, Vector2 position, Vector2 dimensions, Texture texture, Shader shader)
            : base(position, ComponentReplicator.PeerType.PT_Remote)
        {
            SetId(id);
            m_visual = new TexturedQuad(dimensions, texture, shader);
            pipeline.AddRenderable(m_visual, Implementations.SSBBRenderPipeline.LayerIdentifiers.LI_Characters, 0);
        }

        protected override void i_onUpdate(double dt, BaseControllers controllers)
        {
            var transComp = GetComponents().GetComponent<ComponentTransform>();
            m_visual.SetPosition(transComp.GetPosition());
            m_visual.SetAngle(transComp.GetAngle());
        }

        public override void OnDelete()
        {
            m_visual.OnDelete();
        }
    }
}
