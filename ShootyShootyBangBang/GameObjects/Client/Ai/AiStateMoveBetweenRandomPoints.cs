using ShootyShootyBangBangEngine.GameObjects;
using ShootyShootyBangBangEngine.GameObjects.Components;
using System;
using System.Numerics;
using static ShootyShootyBangBangEngine.GameObjects.Components.ComponentAiSystem;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    class AiStateMoveBetweenRandomPoints : ComponentAiSystem.AiState
    {
        Vector2 m_minPoints;
        Vector2 m_maxPoints;
        float m_movementSpeed;

        public AiStateMoveBetweenRandomPoints(Vector2 minPoints, Vector2 maxPoints, float movementSpeed) { m_minPoints = minPoints; m_maxPoints = maxPoints; m_movementSpeed = movementSpeed; }

        public AiStateState Update(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, double dt) 
        {
            var transformComp = owner.GetComponents().GetComponent<ComponentTransform>();

            var rnd = new Random();
            var angle = rnd.NextDouble() * Math.PI * 2;
            owner.GetComponents().GetComponent<ComponentAiSystem>().AddState(new AiStateGoToPoint(new Vector2(rnd.Next((int)m_minPoints.X, (int)m_maxPoints.X), rnd.Next((int)m_minPoints.Y, (int)m_maxPoints.Y)), m_movementSpeed, new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle))));
            return AiStateState.ASS_InProgress;
        }

        public void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state) { }
    }
}
