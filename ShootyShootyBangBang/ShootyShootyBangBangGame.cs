using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBang
{
    class ShootyShootyBangBangGame : ShootyShootyBangBangEngine.Game
    {
        TexturedQuad m_background;
        public ShootyShootyBangBangGame(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers)
            :base(controllers)
        {

        }

        public override void OnLoad()
        {
            base.OnLoad();
            var renderControllers = m_controllers as ShootyShootyBangBangEngine.Controllers.RenderControllers;
            var renderPipeLine = renderControllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddLayer("Background", 0);
            renderPipeLine.AddLayer("Characters", 1);

            m_background = new TexturedQuad(new Vector2(3200,2400), renderControllers.GetTextureManager().GetOrCreateTexture("sand", "Textures/sand.jpg"), renderControllers.GetShaderManager().GetDefaultShader());
            m_background.SetRepeating(10.0f);
            renderPipeLine.AddRenderable(m_background, "Background", 0);
            
            var testChar = new SSBBPlayerControlledCharacter(renderControllers, new Vector2(), new Vector2(32, 32), renderControllers.GetTextureManager().GetOrCreateTexture("player", "Textures/Circle_blue.png"), renderControllers.GetShaderManager().GetDefaultShader());
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
    }
}
