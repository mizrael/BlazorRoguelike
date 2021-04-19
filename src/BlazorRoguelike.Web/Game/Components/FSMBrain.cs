using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public abstract class FSMBrain : Component
    {
        private IStatePicker _statePicker;

        protected FSMBrain(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask UpdateCore(GameContext game)
        {
            _statePicker ??= this.InitStatePicker();
            var state = _statePicker.GetCurrentState();
            if (state is not null)
            {
                state.Execute(game);
                if (state.Completed)
                    state.Exit();
            }
                
            return ValueTask.CompletedTask;
        }

        protected abstract IStatePicker InitStatePicker();
    }
}