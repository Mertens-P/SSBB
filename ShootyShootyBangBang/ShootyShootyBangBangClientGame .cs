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
    class ShootyShootyBangBangClientGame : ShootyShootyBangBangGame
    {
        ShootyShootyBangBangEngine.Controllers.ClientControllers m_clientControllers;
        TexturedQuad m_background;
        public ShootyShootyBangBangClientGame(ShootyShootyBangBangEngine.Controllers.ClientControllers controllers)
            :base(controllers)
        {
            m_clientControllers = controllers;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            var renderPipeLine = m_clientControllers.GetRenderPipeline() as LayeredRenderPipeline;
            renderPipeLine.AddLayer("Background", 0);
            renderPipeLine.AddLayer("Characters", 1);

            m_background = new TexturedQuad(new Vector2(3600, 2800), m_clientControllers.GetTextureManager().GetOrCreateTexture("sand", "Textures/sand.jpg"), m_clientControllers.GetShaderManager().GetDefaultShader());
            m_background.SetRepeating(10.0f);
            renderPipeLine.AddRenderable(m_background, "Background", 0);

            m_clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.HelloWorldPacket());
        }

        public override void OnUpdateFrame(double dt)
        {
            base.OnUpdateFrame(dt);
            if (m_clientControllers.GetkeyboardState().IsKeyDown(Key.Escape))
                m_isRunning = false;
        }
    }
}
