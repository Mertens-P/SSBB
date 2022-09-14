using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Numerics;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateGoToPoint : ComponentAiSystem.AiState
    {
        Vector2 m_point;
        Vector2? m_lookat;
        float m_movementSpeed;

        public AiStateGoToPoint(Vector2 point, float movementSpeed, Vector2? lookat = null) { m_point = point; m_movementSpeed = movementSpeed; m_lookat = lookat; }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            var transformComp = owner.GetComponents().GetComponent<ComponentTransform>();
            var dir = m_point - transformComp.GetPosition();
            if (transformComp.GetPosition() == m_point || dir.LengthSquared() < 1)
                return AiStateState.ASS_Completed;

            var dist = dir.Length();
            dir = Vector2.Normalize(dir);
            transformComp.SetPosition(transformComp.GetPosition() + dir * Math.Min(m_movementSpeed * (float)dt,dist));

            if (m_lookat.HasValue)
                dir = m_lookat.Value;
            var angle = (float)Math.Acos(Vector2.Dot(dir, new Vector2(1,0)));
            if (Vector2.Dot(dir, new Vector2(0, 1)) < 0)
                angle = -angle;
            transformComp.SetAngle(angle);

            return AiStateState.ASS_InProgress; 
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
