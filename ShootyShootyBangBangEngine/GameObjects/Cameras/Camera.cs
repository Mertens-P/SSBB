using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.GameObjects.Cameras
{
    public class Camera : GameObject
    {
        float m_zoomFactor = 1.0f;

        public void SetZoomFactor(float zoom) { m_zoomFactor = zoom; }
        public float GetZoomFactor() { return m_zoomFactor; }

        public Camera(Vector2 pos): base() { GetComponents().AddComponent(new Components.ComponentTransform(pos)); }
        public override void OnDelete() { }
    }
}
