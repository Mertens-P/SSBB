using OpenTK.Input;
using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class RenderControllers : BaseControllers
    {
        TextureManager m_textureManager = new TextureManager();
        ShaderManager m_shaderManager = new ShaderManager();
        KeyboardState m_input;

        public TextureManager GetTextureManager() { return m_textureManager; }
        public ShaderManager GetShaderManager() { return m_shaderManager; }
        public KeyboardState GetInput() { return m_input; }

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

        public void UpdateInput()
        {
            m_input = Keyboard.GetState();
        }
    }
}
