﻿using OpenTK;
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
    class ClientCharacter : ClientServer.SSBBCharacter
    {
        TexturedQuad m_visual;
        public ClientCharacter(RenderControllers controllers, Vector2 position, Vector2 dimensions, Texture texture, Shader shader, ComponentReplicator.PeerType peertype)
            :base(position, peertype)
        {
            m_visual = new TexturedQuad(dimensions, texture, shader);
            var renderPipeLine = controllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddRenderable(m_visual, "Characters", 0);
        }

        public ClientCharacter(Guid id, RenderControllers controllers, Vector2 position, Vector2 dimensions, Texture texture, Shader shader)
            : base(position, ComponentReplicator.PeerType.PT_Remote)
        {
            SetId(id);
            m_visual = new TexturedQuad(dimensions, texture, shader);
            var renderPipeLine = controllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddRenderable(m_visual, "Characters", 0);
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            base.OnUpdate(dt, controllers);
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
