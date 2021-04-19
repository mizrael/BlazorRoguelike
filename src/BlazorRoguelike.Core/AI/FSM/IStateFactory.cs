namespace BlazorRoguelike.Core.AI.FSM
{
	public interface IStateFactory
	{
		State Create(StatePickingRule rule);
	}
}

