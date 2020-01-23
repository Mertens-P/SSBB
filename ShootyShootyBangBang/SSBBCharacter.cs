﻿using OpenTK;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang
{
    class SSBBCharacter : GameObject
    {
        protected float m_movementSpeed = 1000.0f;
        TexturedQuad m_visual;

        public SSBBCharacter(RenderControllers controllers, Vector2 pos, Vector2 dimensions, Texture texture, Shader shader)
        {
            GetComponents().AddComponent(new ComponentTransform(pos));
            m_visual = new TexturedQuad(dimensions, texture, shader);
            var renderPipeLine = controllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddRenderable(m_visual, "Characters", 0);
        }

        public override void OnUpdate(double dt, BaseControllers controllers)
        {
            base.OnUpdate(dt, controllers);
            var transComp = GetComponents().GetComponent<ComponentTransform>();
            m_visual.SetPosition(transComp.GetPosition());
        }

        public override void OnDelete()
        {
            m_visual.OnDelete();
        }
    }
}
