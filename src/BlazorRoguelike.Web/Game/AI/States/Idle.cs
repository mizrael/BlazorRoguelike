using System;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;

namespace BlazorRoguelike.Web.Game.AI
{
	public class Idle : State
	{
		private float _duration;
		private bool _hasDuration = false;
		
		public Idle(GameObject owner) : this(owner, 5f){}

		public Idle(GameObject owner, float duration) : base(owner){
			_duration = Math.Abs(duration);
			_hasDuration = _duration > 0f;
		}

		protected override void OnExecute (GameContext game)
		{
			if (_hasDuration && this.ExecutionTime > _duration) {
				this.Completed = true;
				return;
			}
			base.OnExecute (game);
		}
	}
}

