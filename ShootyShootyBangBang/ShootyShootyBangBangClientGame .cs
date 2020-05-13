using OpenTK.Input;
using ShootyShootyBangBang.GameObjects.Client.Ai;
using ShootyShootyBangBangEngine;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShootyShootyBangBangEngine.Helpers;

namespace ShootyShootyBangBang
{
    class ShootyShootyBangBangClientGame : ShootyShootyBangBangGame
    {
        Implementations.SSBBClientControllers m_clientControllers;
        TexturedQuad m_background;
        List<TexturedQuad> m_dirtPatches = new List<TexturedQuad>();
        AiFactory.AiType m_aiType = GameObjects.Client.Ai.AiFactory.AiType.AT_None;

        public ShootyShootyBangBangClientGame(Implementations.SSBBClientControllers controllers, AiFactory.AiType aiType)
            :base(controllers)
        {
            m_clientControllers = controllers;
            m_aiType = aiType;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            m_background = new TexturedQuad(new Vector2(3600, 2800), m_clientControllers.GetTextureManager().GetOrCreateTexture("sand", "Textures/sand.jpg"), m_clientControllers.GetShaderManager().GetDefaultShader());
            m_background.SetRepeating(10.0f);
            m_clientControllers.GetSSBBRenderPipeline().AddRenderable(m_background, Implementations.SSBBRenderPipeline.LayerIdentifiers.LI_Background, 0);

            i_createDirtPatch(new Vector2(-400, 300), new Vector2(400,400));
            i_createDirtPatch(new Vector2(0, -100), new Vector2(1200, 32));
            i_createDirtPatch(new Vector2(0, -200), new Vector2(1200, 32));
            i_createDirtPatch(new Vector2(0, -300), new Vector2(1200, 32));

            var dirtPatch = new TexturedQuad(new Vector2(450, 450), m_clientControllers.GetTextureManager().GetOrCreateTexture("dirtCircle", "Textures/DirtCircle.png"), m_clientControllers.GetShaderManager().GetDefaultShader());
            dirtPatch.SetPosition(new Vector2(100, 300));
            m_clientControllers.GetSSBBRenderPipeline().AddRenderable(dirtPatch, Implementations.SSBBRenderPipeline.LayerIdentifiers.LI_Background, 1);

            m_clientControllers.GetNetClient().SendRPC(new Networking.ClientServer.NetPackets.RequestPlayerCharacterClientPacket() { aiType = m_aiType, position = MathHelpers.SytemVecToOpenTkVec(new Vector2()) });
        }

        public override void OnUpdateFrame(double dt)
        {
            base.OnUpdateFrame(dt);
            if (m_clientControllers.GetkeyboardState().IsKeyDown(Key.Escape))
                m_isRunning = false;
        }

        void i_createDirtPatch(Vector2 position, Vector2 dimensions)
        {
            var dirtPatch = new TexturedQuad(dimensions, m_clientControllers.GetTextureManager().GetOrCreateTexture("dirt", "Textures/dirt.jpg"), m_clientControllers.GetShaderManager().GetDefaultShader());
            dirtPatch.SetUvScale(dimensions / new Vector2(400,400));
            dirtPatch.SetPosition(position);
            m_clientControllers.GetSSBBRenderPipeline().AddRenderable(dirtPatch, Implementations.SSBBRenderPipeline.LayerIdentifiers.LI_Background, 1);
        }
    }
}
