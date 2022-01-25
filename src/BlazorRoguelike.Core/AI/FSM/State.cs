namespace BlazorRoguelike.Core.AI.FSM
{
    public abstract class State
	{
		public State(GameObject owner)
		{
			this.Owner = owner;

			this.ElapsedMilliseconds = 0f;
		}

		protected virtual void OnEnter (GameContext game) {}

		protected virtual void OnExecute (GameContext game){}

		protected virtual void OnExit (GameContext game) {}

		public void Enter(GameContext game)
		{
			this.ElapsedMilliseconds = 0f;
			this.IsCompleted = false;
			this.OnEnter(game);
		}

		public void Execute(GameContext game)
		{
			this.ElapsedMilliseconds += game.GameTime.ElapsedMilliseconds;
			OnExecute(game);
		}

		public void Exit(GameContext game)
		{
			this.IsCompleted = true;
			this.OnExit(game);
		}

		#region Properties

		public readonly GameObject Owner;

		public float ElapsedMilliseconds { get; private set; }

		public bool IsCompleted { get; protected set; }

		#endregion Properties
	}
}

