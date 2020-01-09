using OpenTK;
using System;
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

        public void SetPosition(Vector2 pos) { m_position = pos; }
        public Vector2 GetPosition() { return m_position; }

        public ComponentTransform(Vector2 position)
        {
            m_position = position;
        }
    }
}
