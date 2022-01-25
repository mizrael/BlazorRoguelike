using System;

namespace BlazorRoguelike.Core.AI.FSM
{
    public record StateTransition
    {
        private readonly State _from;        
        private readonly Predicate<State> _predicate;
        private readonly Action<State> _beforeTransition;

        public StateTransition(State from, State to, Predicate<State> predicate, Action<State> beforeTransition)
        {
            To = to;            
            _from = from;
            _predicate = predicate;
            _beforeTransition = beforeTransition;
        }

        public bool CanTransition()
            => _predicate(_from);

        public void BeforeTransition()
            => _beforeTransition?.Invoke(this.To);

        public State To { get; }
    }
}