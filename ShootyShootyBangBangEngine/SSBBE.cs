using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ShootyShootyBangBangEngine
{
    public class SSBBE : GameWindow
    {
        Game m_game;
        string m_contentDirPath = "../../Content/";

        public struct RenderSettings
        {
            public int Width;
            public int Height;
            public float InvWidth;
            public float InvHeight;
        }

        public SSBBE(Game game, int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { m_game = game; }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            base.OnLoad(e);
            Directory.SetCurrentDirectory(m_contentDirPath);
            m_game.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            m_game.OnUpdateFrame(e.Time);
            if (!m_game.GetisRunning())
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            m_game.OnRenderFrame(new RenderSettings() { Width = Width, Height = Height, InvWidth = 1.0f / (float)Width, InvHeight = 1.0f / Height });

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            m_game.UnLoad();
            base.OnUnload(e);
        }
    }
}
