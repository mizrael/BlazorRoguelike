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
            if (_currState is not null && !_currState.IsCompleted)
            {
                _currState.Execute(game);
                if (_currState.IsCompleted)
                {
                    _currState.Exit(game);
                    _currState = null;
                }                    
            }
                
            return ValueTask.CompletedTask;
        }

        protected void SetState(GameContext game, State newState)
        {
            if (_currState is not null)
                _currState.Exit(game);
            _currState = newState;
            _currState.Enter(game);
        }
    }
}