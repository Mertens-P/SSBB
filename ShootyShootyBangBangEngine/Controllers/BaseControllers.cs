using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Cameras;

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
        public virtual void OnUpdate(double dt)
        {
            m_scene.OnUpdate(dt, this);
        }
        public virtual void OnDelete()
        {
            m_scene.OnDelete();
        }
    }
}
