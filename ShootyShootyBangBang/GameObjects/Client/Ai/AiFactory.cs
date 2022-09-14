using ShootyShootyBangBangEngine.GameObjects.Components;
using System.Numerics;
using System.Collections.Generic;

namespace ShootyShootyBangBang.GameObjects.Client.Ai
{
    public class AiFactory
    {
        public enum AiType
        {
            AT_None,
            AT_TestBot_1,
            AT_RaceBot_1,
            AT_RaceBot_2,
            AT_TestBot_Circle,
            AT_RandomBot
        }

        public static ComponentAiSystem.AiState CreateBaseAiState(AiType aiType, float movementSpeed)
        {
            switch(aiType)
            {
                case AiType.AT_TestBot_1: return new AiStateMoveBetweenPoints(new List<Vector2>() { new Vector2(-600, -100), new Vector2(600, -100) }, movementSpeed);
                case AiType.AT_RaceBot_1: return new AiStateTimedMoveBetweenPoints(new List<Vector2>() { new Vector2(-600, -200), new Vector2(600, -200) }, movementSpeed, 5);
                case AiType.AT_RaceBot_2: return new AiStateTimedMoveBetweenPoints(new List<Vector2>() { new Vector2(-600, -300), new Vector2(600, -300) }, movementSpeed, 5);
                case AiType.AT_TestBot_Circle: return new AiStateWalkInCircle(new Vector2(100, 300), 200, movementSpeed);
                case AiType.AT_RandomBot: return new AiStateMoveBetweenRandomPoints(new Vector2(-600, 100), new Vector2(-200, 500), movementSpeed);
                default: return null;
            }
        }
    }
}
