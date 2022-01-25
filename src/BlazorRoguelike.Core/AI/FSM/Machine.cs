using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorRoguelike.Core.AI.FSM
{
    public class Machine
    {
        private readonly Dictionary<State, List<StateTransition>> _states;
        private State _currState;

        public Machine(IEnumerable<State> states)
        {
            _states = states.ToDictionary(s => s, _ => new List<StateTransition>());
            _currState = states.First();
        }

        public void AddTransition(State from, State to, Predicate<State> predicate)
            => this.AddTransition(from, to, predicate, null);

        public void AddTransition(State from, State to, Predicate<State> predicate, Action<State> beforeTransition)
            => _states[from].Add(new StateTransition(from, to, predicate, beforeTransition));

        public void Update(GameContext game)
        {
            if (null == _currState)
                return;

            var transitions = _states[_currState];
            var validTransition = transitions.FirstOrDefault(t => t.CanTransition());
            if(null != validTransition)
            {
                _currState.Exit(game);

                validTransition.BeforeTransition();

                _currState = validTransition.To;
                _currState.Enter(game);
            }

            if(!_currState.IsCompleted)
                _currState.Execute(game);
        }
    }
}
