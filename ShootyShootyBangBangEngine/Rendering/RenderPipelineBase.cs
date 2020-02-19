using ShootyShootyBangBangEngine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
