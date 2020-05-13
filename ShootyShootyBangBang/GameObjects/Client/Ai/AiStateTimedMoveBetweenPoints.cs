using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateTimedMoveBetweenPoints : ComponentAiSystem.AiState
    {
        int m_currentPoint = 0;
        List<Vector2> m_points;
        float m_movementSpeed = 1;
        int m_secInterval = 0;

        public AiStateTimedMoveBetweenPoints(List<Vector2> points, float movementSpeed, int secInterval) { m_points = points; m_movementSpeed = movementSpeed; m_secInterval = secInterval; }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            if (DateTime.Now.Second % m_secInterval == 0)
            {
                var lastPoint = m_currentPoint - 1;
                if (lastPoint < 0) lastPoint = m_points.Count-1;
                var startTime = DateTime.Now;
                startTime = startTime.AddMilliseconds(-startTime.Millisecond);
                owner.GetComponents().GetComponent<ComponentAiSystem>().AddState(new AiStateGoToPointTimed(m_points[lastPoint], m_points[m_currentPoint], m_movementSpeed, startTime));
                m_currentPoint = (m_currentPoint + 1) % m_points.Count;
            }
            return AiStateState.ASS_InProgress; 
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
