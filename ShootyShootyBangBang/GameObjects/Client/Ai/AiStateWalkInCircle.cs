using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Numerics;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateWalkInCircle : ComponentAiSystem.AiState
    {
        Vector2 m_center;
        float m_radius;
        float m_movementSpeed;

        public AiStateWalkInCircle(Vector2 center, float radius, float movementSpeed) { m_center = center; m_radius = radius; m_movementSpeed = movementSpeed; }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            var transformComp = owner.GetComponents().GetComponent<ComponentTransform>();

            var centerToMe = transformComp.GetPosition() - m_center;
            var perp = new Vector2(centerToMe.Y, -centerToMe.X);
            perp = Vector2.Normalize(perp);
            perp *= (float)(m_movementSpeed * dt);
            var newPoint = centerToMe + perp;
            newPoint = Vector2.Normalize(newPoint);
            newPoint *= m_radius;
            newPoint += m_center;
            var dir = newPoint - transformComp.GetPosition();
            if (transformComp.GetPosition() == newPoint || dir.LengthSquared() < 1)
                return AiStateState.ASS_InProgress;

            var dist = dir.Length();
            dir = Vector2.Normalize(dir);
            transformComp.SetPosition(transformComp.GetPosition() + dir * Math.Min(m_movementSpeed * (float)dt,dist));

            var angle = (float)Math.Acos(Vector2.Dot(dir, new Vector2(1,0)));
            if (Vector2.Dot(dir, new Vector2(0, 1)) < 0)
                angle = -angle;
            transformComp.SetAngle(angle);

            return AiStateState.ASS_InProgress; 
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
