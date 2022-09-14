using ShootyShootyBangBangEngine.Controllers;

namespace ShootyShootyBangBangEngine.Rendering
{
    public abstract class Renderable
    {
        public virtual void OnUpdate(RenderControllers controllers, double dt) { }
        public abstract void OnRender(SSBBE.RenderSettings renderSettings, GameObjects.Cameras.Camera camera);
    }

    public abstract class RenderPipelineBase
    {
        public abstract void OnUpdate(RenderControllers controllers, double dt);
        public abstract void OnRender(RenderControllers controllers, SSBBE.RenderSettings renderSettings);
    }
}
