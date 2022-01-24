using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public abstract class FSMBrainComponent : Component
    {
        private State _currState;

        protected FSMBrainComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask UpdateCore(GameContext game)
        {               
            if (_currState is not null && !_currState.Completed)
            {
                _currState.Execute(game);
                if (_currState.Completed)
                {
                    _currState.Exit();
                    _currState = null;
                }                    
            }
                
            return ValueTask.CompletedTask;
        }

        protected void SetState(State newState)
        {
            if (_currState is not null)
                _currState.Exit();
            _currState = newState;
            _currState.Enter();
        }
    }
}