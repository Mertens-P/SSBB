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
            var clientControllers = m_controllers as ShootyShootyBangBangEngine.Controllers.ClientControllers;
            if (clientControllers != null)
                i_onClientLoad(clientControllers);
        }

        public override void OnUpdateFrame(double dt)
        {
            base.OnUpdateFrame(dt);
            var renderControllers = m_controllers as ShootyShootyBangBangEngine.Controllers.RenderControllers;
            if (renderControllers != null)
            {
                if (renderControllers.GetInput().IsKeyDown(Key.Escape))
                    m_isRunning = false;
            }
        }

        void i_onClientLoad(ShootyShootyBangBangEngine.Controllers.ClientControllers clientControllers)
        {
            var renderPipeLine = clientControllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddLayer("Background", 0);
            renderPipeLine.AddLayer("Characters", 1);

            m_background = new TexturedQuad(new Vector2(3600, 2800), clientControllers.GetTextureManager().GetOrCreateTexture("sand", "Textures/sand.jpg"), clientControllers.GetShaderManager().GetDefaultShader());
            m_background.SetRepeating(10.0f);
            renderPipeLine.AddRenderable(m_background, "Background", 0);

            //var testChar = new SSBBPlayerControlledCharacter(clientControllers, new Vector2(), new Vector2(32, 32), clientControllers.GetTextureManager().GetOrCreateTexture("player", "Textures/Circle_blue.png"), clientControllers.GetShaderManager().GetDefaultShader());
            //m_controllers.GetRootScene().AddGameObject(testChar);
            //
            //var camera = new FollowCamera(testChar.GetId());
            //camera.SetExtends(new Vector2(-800, -600), new Vector2(800, 600));
            //m_controllers.GetRootScene().AddGameObject(camera);
            //m_controllers.SetCamera(camera);

            Console.WriteLine("Sending hello world");
            clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.HelloWorldPacket());
        }
    }
}
