using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System.Collections.Generic;
using System.Numerics;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateMoveBetweenPoints : ComponentAiSystem.AiState
    {
        int m_currentPoint = 0;
        List<Vector2> m_points;
        float m_movementSpeed = 1;

        public AiStateMoveBetweenPoints(List<Vector2> points, float movementSpeed) { m_points = points; m_movementSpeed = movementSpeed; }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            owner.GetComponents().GetComponent<ComponentAiSystem>().AddState(new AiStateGoToPoint(m_points[m_currentPoint], m_movementSpeed));
            m_currentPoint = (m_currentPoint + 1) % m_points.Count;
            return AiStateState.ASS_InProgress; 
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
