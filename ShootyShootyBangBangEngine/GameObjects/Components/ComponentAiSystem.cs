using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootyShootyBangBangEngine.GameObjects.EntityComponentManager;

namespace ShootyShootyBangBangEngine.GameObjects.Components
{
    public class ComponentAiSystem : EntityComponentBase
    {
        public enum AiStateState
        {
            ASS_InProgress,
            ASS_Completed,
            ASS_Failed,
            ASS_Cancelled
        } // hehe, "ASS"
        public interface AiState
        {
            AiStateState Update(Controllers.BaseControllers controllers, GameObject owner, double dt);
            void OnChildStateFinished(ShootyShootyBangBangEngine.Controllers.BaseControllers controllers, GameObject owner, AiStateState state);
        }

        List<AiState> m_states = new List<AiState>();

        public void AddState(AiState state)
        {
            m_states.Add(state);
        }

        public override void Tick(Controllers.BaseControllers controllers, GameObject owner, double dt)
        {
            base.Tick(controllers, owner, dt);
            if (m_states.Count > 0)
            {
                var currentStateId = m_states.Count - 1;
                var currentState = m_states.Last();
                var currentStateState = currentState.Update(controllers, owner, dt);
                switch (currentStateState)
                {
                    case AiStateState.ASS_Completed:
                    case AiStateState.ASS_Failed:
                    case AiStateState.ASS_Cancelled:
                        m_states.Remove(currentState);
                        if(m_states.Count > currentStateId - 1)
                            m_states[currentStateId - 1].OnChildStateFinished(controllers, owner, currentStateState);
                        break;
                }
            }
        }
    }
}
