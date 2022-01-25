namespace BlazorRoguelike.Core.AI.FSM
{
    public interface IStatePicker
    {
        State GetNextState(GameContext game);
    }
}