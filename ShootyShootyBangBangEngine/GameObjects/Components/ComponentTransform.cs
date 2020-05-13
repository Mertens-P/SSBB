using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.GameObjects.EntityComponentManager;

namespace ShootyShootyBangBangEngine.GameObjects.Components
{
    public class ComponentTransform : EntityComponentBase
    {
        Vector2 m_position;
        float m_angleInRad;
        Vector2 m_scale = new Vector2(1,1);

        public void SetPosition(Vector2 pos) { m_position = pos; }
        public void SetAngle(float angleInRad) { m_angleInRad = angleInRad; }
        public void SetScale(Vector2 scale) { m_scale = scale; }
        public Vector2 GetPosition() { return m_position; }
        public float GetAngle() { return m_angleInRad; }
        public Vector2 GetScale() { return m_scale; }

        public ComponentTransform(Vector2 position)
        {
            m_position = position;
        }
    }
}
