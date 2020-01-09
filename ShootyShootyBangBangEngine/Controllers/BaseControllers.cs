using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Controllers
{
    public class BaseControllers
    {
        Scene m_scene = new Scene();
        Camera m_mainCamera;

        public void SetCamera(Camera camera) { m_mainCamera = camera; }
        public Scene GetRootScene() { return m_scene; }
        public Camera GetCamera() { return m_mainCamera; }

        public virtual void Init()  {}
        public virtual void OnDelete()
        {
            m_scene.OnDelete();
        }
    }
}
