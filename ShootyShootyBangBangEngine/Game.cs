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
            m_controllers.GetRootScene().OnUpdate(dt, m_controllers);
            m_controllers.GetCamera().OnUpdate(dt, m_controllers);
        }

        public virtual void OnRenderFrame(RenderSettings renderSettings)
        {
            var renderControllers = m_controllers as RenderControllers;
            renderControllers.GetRenderPipeline().OnRender(renderControllers, renderSettings);
        }
    }
}
