using OpenTK;
using OpenTK.Input;
using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.SSBBE;

namespace ShootyShootyBangBangEngine
{
    public abstract class Game
    {
        protected BaseControllers m_controllers;
        protected bool m_isRunning = true;

        public bool GetisRunning() { return m_isRunning; }
        public void Stop() { m_isRunning = false; }

        public Game(BaseControllers controllers) { m_controllers = controllers; }
        public virtual void OnLoad()
        {
            m_controllers.Init();
        }
        public virtual void UnLoad()
        {
            m_controllers.OnDelete();
        }
        public virtual void OnUpdateFrame(double dt)
        {
            m_controllers.OnUpdate(dt);
        }

        public virtual void OnRenderFrame(RenderSettings renderSettings)
        {
            var renderControllers = m_controllers as RenderControllers;
            renderControllers.OnRender(renderSettings);
        }
    }
}
