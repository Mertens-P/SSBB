using System.Numerics;

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
