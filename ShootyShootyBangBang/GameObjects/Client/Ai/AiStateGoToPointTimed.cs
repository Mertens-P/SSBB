using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Numerics;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateGoToPointTimed : ComponentAiSystem.AiState
    {
        Vector2 m_startPoint;
        Vector2 m_endPoint;
        float m_movementSpeed;
        double m_timer;
        DateTime m_startTime;

        public AiStateGoToPointTimed(Vector2 startPoint, Vector2 endPoint, float movementSpeed, DateTime startTime) 
        {
            m_startPoint = startPoint;
            m_endPoint = endPoint; 
            m_movementSpeed = movementSpeed; 
            m_startTime = startTime;
        }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            m_timer += dt;

            var transformComp = owner.GetComponents().GetComponent<ComponentTransform>();
            var startToEnd = m_endPoint - m_startPoint;

            var dist = startToEnd.Length();
            var totalTime = dist / m_movementSpeed;
            var fac = m_timer / totalTime;
            if (fac > 1.0f) return AiStateState.ASS_Completed;

            var tarPos = m_startPoint + startToEnd * (float)fac;

            var curPosToTarPos = tarPos - transformComp.GetPosition();
            curPosToTarPos = Vector2.Normalize(curPosToTarPos);
            var dot = Vector2.Dot(curPosToTarPos, new Vector2(1, 0));
            var angle = (float)Math.Acos(dot);
            if (Vector2.Dot(curPosToTarPos, new Vector2(0, 1)) < 0)
                angle = -angle;
            transformComp.SetAngle(angle);

            transformComp.SetPosition(tarPos);

            return AiStateState.ASS_InProgress; 
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
