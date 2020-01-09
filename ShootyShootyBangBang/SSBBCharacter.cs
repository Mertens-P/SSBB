using OpenTK;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.Controllers;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
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

        public SSBBCharacter(Vector2 pos, Vector2 dimensions, string textureFilePath, Shader shader)
        {
            GetComponents().AddComponent(new ComponentTransform(pos));
            m_visual = new TexturedQuad(pos, dimensions, textureFilePath, shader);
        }

        public override void OnRender(RenderControllers controllers, SSBBE.RenderSettings renderSettings)
        {
            base.OnRender(controllers, renderSettings);
            var transComp = GetComponents().GetComponent<ComponentTransform>();
            m_visual.OnRender(transComp.GetPosition(), renderSettings, controllers.GetCamera());
        }

        public override void OnDelete()
        {
            m_visual.OnDelete();
        }
    }
}
