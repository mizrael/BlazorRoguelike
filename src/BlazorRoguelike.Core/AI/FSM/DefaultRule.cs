namespace BlazorRoguelike.Core.AI.FSM
{
    public class DefaultRule : StatePickingRule
    {
        public DefaultRule(GameObject owner, float weight) : base(owner, weight)
        {
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}

