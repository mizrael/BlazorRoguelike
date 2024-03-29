﻿using System.Collections.Generic;
using System;

namespace BlazorRoguelike.Core.AI.FSM
{
	public class RulesStatePicker : IStatePicker
    {
        private readonly List<(StatePickingRule rule, IStateFactory factory)> _rules = new();
		private readonly IStateFactory _defaultStateFactory;

		private State _currState;
		private IStateFactory _currFactory;

		public RulesStatePicker(IStateFactory defaultStateFactory){
			_defaultStateFactory = defaultStateFactory;
		}

		public void Add(StatePickingRule rule, IStateFactory factory){
			if (null == rule)
				throw new ArgumentNullException (nameof(rule));
			if (null == factory)
				throw new ArgumentNullException (nameof(factory));
            _rules.Add ( new (rule, factory));
            _rules.Sort((a, b) => a.rule.Weight >= b.rule.Weight ? 1 : 0); //TODO: ensure sorting is descending
		}

		public State GetNextState(GameContext game)
		{
			var (rule, factory) = FindValidStateBuilder ();

			if(!CheckCanReplaceCurrentState(rule, factory))
				return _currState;
			
            if (!CheckIsRunning(_currState) || _currFactory != _defaultStateFactory) {
				SwitchState (game, _defaultStateFactory, null);
			} else if (null != rule) {
				SwitchState (game, factory, rule);
			}

			return _currState;
		}

		private void SwitchState(GameContext game, IStateFactory factory, StatePickingRule rule){
            if (CheckIsRunning(_currState))
				_currState.Exit(game);
			_currFactory = factory;
			_currState = _currFactory.Create (rule);
			_currState.Enter (game);
		}

		private (StatePickingRule rule, IStateFactory factory) FindValidStateBuilder(){
            int count = _rules.Count;
            for (int i = 0; i != count;++i){
                var tuple = _rules[i];
                if (!tuple.rule.CanExecute())
                    continue;
                return tuple;
            }

            return (null, null);
		}

		private bool CheckCanReplaceCurrentState(StatePickingRule rule, IStateFactory factory){
			if (null == rule || null == factory)
				return false;

            var isRunning = CheckIsRunning (_currState);
            if (!isRunning)
                return true;
				//return tuple.First.Weight > _currFactory; //TODO: check state rule weight
			
			var isSameState = (factory == _currFactory);

            return !isSameState;
		}

		private static bool CheckIsRunning(State state){
            return (null != state && !state.IsCompleted);
		}
	}
}
