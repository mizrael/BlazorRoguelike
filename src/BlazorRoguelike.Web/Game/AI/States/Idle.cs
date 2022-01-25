using System;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;

namespace BlazorRoguelike.Web.Game.AI.States
{
    public class Idle : State
	{
		private float _duration;
		private bool _hasDuration = false;
		
		public Idle(GameObject owner) : this(owner, 0f){}

		public Idle(GameObject owner, float duration) : base(owner){
			_duration = Math.Abs(duration);
			_hasDuration = _duration > 0f;
		}

		protected override void OnExecute (GameContext game)
		{
			if (_hasDuration && this.ElapsedMilliseconds > _duration) {
				this.IsCompleted = true;
				return;
			}
			base.OnExecute (game);
		}
	}
}

