using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang
{
    class ShootyShootyBangBangGame : ShootyShootyBangBangEngine.Game
    {
        ShootyShootyBangBangEngine.GameObjects.TexturedQuad m_background;
        public ShootyShootyBangBangGame(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers)
            :base(controllers)
        {

        }

        public override void OnLoad()
        {
            base.OnLoad();
            var renderControllers = m_controllers as ShootyShootyBangBangEngine.Controllers.RenderControllers; 
            m_background = new ShootyShootyBangBangEngine.GameObjects.TexturedQuad(new Vector2(), new Vector2(1600,1200), "Textures/sand.jpg", renderControllers.GetShaderManager().GetDefaultShader());
            m_background.SetRepeating(10.0f);
            
            var testChar = new SSBBPlayerControlledCharacter(new Vector2(), new Vector2(16, 16), "Textures/Circle_blue.png", renderControllers.GetShaderManager().GetDefaultShader());
            m_controllers.GetRootScene().AddGameObject(testChar);
            
            var camera = new FollowCamera(testChar.GetId());
            camera.SetExtends(new Vector2(-800, -600), new Vector2(800, 600));
            m_controllers.GetRootScene().AddGameObject(camera);
            m_controllers.SetCamera(camera);
        }

        public override void OnUpdateFrame(double dt)
        {
            base.OnUpdateFrame(dt);
            var renderControllers = m_controllers as ShootyShootyBangBangEngine.Controllers.RenderControllers;
            renderControllers.UpdateInput();
            if (renderControllers.GetInput().IsKeyDown(Key.Escape))
            {
                m_isRunning = false;
            }
        }

        public override void OnRenderFrame(SSBBE.RenderSettings renderSettings)
        {
            m_background.OnRender(new Vector2(), renderSettings, m_controllers.GetCamera());
            base.OnRenderFrame(renderSettings);
        }
    }
}
