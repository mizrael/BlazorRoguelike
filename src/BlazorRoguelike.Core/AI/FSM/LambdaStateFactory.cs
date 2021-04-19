using System;

namespace BlazorRoguelike.Core.AI.FSM
{
	public class LambdaStateFactory : IStateFactory
	{
		private Func<StatePickingRule, State> _factory;

		public LambdaStateFactory (Func<StatePickingRule, State> factory)
		{
			_factory = factory ?? throw new ArgumentException (nameof(factory));
		}

		public State Create (StatePickingRule rule)
			=> _factory(rule);
	}
}

