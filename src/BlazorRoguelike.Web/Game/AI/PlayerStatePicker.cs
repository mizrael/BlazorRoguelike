using BlazorRoguelike.Core.AI.FSM;

namespace BlazorRoguelike.Web.Game.AI
{
    public class PlayerStatePicker : IStatePicker
    {
        private State _currState;
        private State _prevState;

        public void SetState(State newState)
        {
            _prevState = _currState;
            if (null != _prevState)
                _prevState.Exit();
            _currState = newState;
            _currState.Enter();
        }

        public State GetCurrentState() => _currState;
    }
}
