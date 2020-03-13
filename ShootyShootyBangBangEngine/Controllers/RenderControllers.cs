﻿using OpenTK.Input;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.SSBBE;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class RenderControllers : BaseControllers
    {
        TextureManager m_textureManager = new TextureManager();
        ShaderManager m_shaderManager = new ShaderManager();
        RenderPipelineBase m_renderPipeline;
        KeyboardState m_input;
        RenderSettings m_lastRenderSettings;

        public RenderControllers(RenderPipelineBase renderPipeline) { m_renderPipeline = renderPipeline; }

        public TextureManager       GetTextureManager() { return m_textureManager; }
        public ShaderManager        GetShaderManager()  { return m_shaderManager; }
        public RenderPipelineBase   GetRenderPipeline() { return m_renderPipeline; }
        public KeyboardState        GetInput()          { return m_input; }

        public override void Init()
        {
            base.Init();
            m_shaderManager.Init();
        }

        public override void OnDelete()
        {
            base.OnDelete();
            m_shaderManager.OnUnload();
        }

        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            if (m_lastRenderSettings.Focused)
                m_input = Keyboard.GetState();
            else
                m_input = new KeyboardState();
            GetRenderPipeline().OnUpdate(this, dt);
        }

        public void OnRender(RenderSettings renderSettings)
        {
            m_lastRenderSettings = renderSettings;
            m_renderPipeline.OnRender(this, renderSettings);
        }
    }
}
