using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace ShootyShootyBangBangEngine
{
    public class SSBBE : GameWindow
    {
        Game m_game;
        string m_contentDirPath = "../../Content/";
        int m_mousePosX;
        int m_mousePosY;

        public struct RenderSettings
        {
            public int Width;
            public int Height;
            public float InvWidth;
            public float InvHeight;
            public bool Focused;
            public int MousePosX;
            public int MousePosY;
        }

        public SSBBE(Game game, int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { m_game = game; }

        public void SetWindowLocation(int xPos, int yPos) { Location = new System.Drawing.Point(xPos, yPos); }

        public int GetWindowXPos() { return Location.X; }
        public int GetWindowYPos() { return Location.Y; }

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

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            m_mousePosX = e.X;
            m_mousePosY = e.Y;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            m_game.OnKeyPressed(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            m_game.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            m_game.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            m_game.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            m_game.OnMouseUp(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            m_game.OnRenderFrame(new RenderSettings() { Width = Width, Height = Height, InvWidth = 1.0f / (float)Width, InvHeight = 1.0f / Height, Focused = Focused, MousePosX = m_mousePosX, MousePosY = m_mousePosY });

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
