using OpenTK.Input;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using ShootyShootyBangBangEngine.GameObjects.Components;
using ShootyShootyBangBangEngine.Rendering;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.SSBBE;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class RenderControllers : BaseControllers
    {
        public struct CurrentMouseState
        {
            public int XPos;
            public int YPos;
        }
        TextureManager m_textureManager = new TextureManager();
        ShaderManager m_shaderManager = new ShaderManager();
        RenderPipelineBase m_renderPipeline;
        KeyboardState m_keyboardState;
        CurrentMouseState m_mouseState;
        RenderSettings m_lastRenderSettings;

        public RenderControllers(RenderPipelineBase renderPipeline) { m_renderPipeline = renderPipeline; }

        public TextureManager       GetTextureManager()     { return m_textureManager; }
        public ShaderManager        GetShaderManager()      { return m_shaderManager; }
        public RenderPipelineBase   GetRenderPipeline()     { return m_renderPipeline; }
        public KeyboardState        GetkeyboardState()      { return m_keyboardState; }
        public CurrentMouseState    GetMouseState()         { return m_mouseState; }
        public RenderSettings       GetLastRenderSettings() { return m_lastRenderSettings; }

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
                m_keyboardState = Keyboard.GetState();
            else
                m_keyboardState = new KeyboardState();
            GetRenderPipeline().OnUpdate(this, dt);
        }

        public void OnRender(RenderSettings renderSettings)
        {
            m_lastRenderSettings = renderSettings;
            m_renderPipeline.OnRender(this, renderSettings);
            m_mouseState.XPos = renderSettings.MousePosX;
            m_mouseState.YPos = renderSettings.MousePosY;
        }

        public Vector2 GetMousePosInScreenSpace()
        {
            if (GetCamera() == null) return new Vector2();
            var cameraTrans = GetCamera().GetComponents().GetComponent<ComponentTransform>();
            if (cameraTrans == null) return new Vector2();
            var camPos = cameraTrans.GetPosition();

            var screenCenter = new Vector2(m_lastRenderSettings.Width, m_lastRenderSettings.Height);
            var mousePos = new Vector2(GetMouseState().XPos, GetMouseState().YPos);
            mousePos.X = Helpers.MathHelpers.Lerp(-screenCenter.X, screenCenter.X, mousePos.X / m_lastRenderSettings.Width);
            mousePos.Y = Helpers.MathHelpers.Lerp(screenCenter.Y, -screenCenter.Y, mousePos.Y / m_lastRenderSettings.Height);

            return camPos + mousePos / GetCamera().GetZoomFactor();
        }
    }
}
