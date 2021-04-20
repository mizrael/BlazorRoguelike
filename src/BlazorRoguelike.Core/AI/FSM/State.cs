namespace BlazorRoguelike.Core.AI.FSM
{
    public abstract class State
	{
		public State(GameObject owner)
		{
			this.Owner = owner;

			this.ExecutionTime = 0f;
		}

		protected virtual void OnEnter (){}

		protected virtual void OnExecute (GameContext game){}

		protected virtual void OnExit () {}

		public void Enter()
		{
			this.ExecutionTime = 0f;
			this.Completed = false;
			this.OnEnter();
		}

		public void Execute(GameContext game)
		{
			this.ExecutionTime += game.GameTime.ElapsedMilliseconds;
			OnExecute(game);
		}

		public void Exit()
		{
			this.Completed = true;
			this.OnExit();
		}

		#region Properties

		public readonly GameObject Owner;

		public float ExecutionTime { get; private set; }

		public bool Completed { get; protected set; }

		#endregion Properties
	}
}

