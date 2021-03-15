using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorRoguelike.Core.AI.FSM
{
	public class CompositeState : State
	{
		private Queue<State> _states;
		private State _currState;

		public CompositeState(GameObject owner) : this(owner, null){}

		public CompositeState (GameObject owner, IEnumerable<State> states) : base(owner)
		{
			_states = new Queue<State>(states ?? Enumerable.Empty<State>());
		}

		protected void EnqueueState(State state){
			if (null == state)
				throw new ArgumentNullException (nameof(state));
			_states.Enqueue (state);
		}

		protected override void OnExecute (GameContext game)
		{
			if (null == _currState) {
				if (0 == _states.Count) {
					this.Completed = true;
					return;
				}

				_currState = _states.Dequeue ();
				_currState.Enter ();
			}

			if (_currState.Completed) {
				_currState = null;
				return;
			}

			_currState.Execute (game);

			if (_currState.Completed) {
				_currState.Exit ();
				_currState = null;
			}
		}

        public bool HasStates{
            get{
                return _states.Any();
            }
        }
	}
}

