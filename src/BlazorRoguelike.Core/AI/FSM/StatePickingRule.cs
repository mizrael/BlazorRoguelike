using System;

namespace BlazorRoguelike.Core.AI.FSM
{
    public abstract class StatePickingRule
    {
        public StatePickingRule(GameObject owner, float weight = 1f)
        {
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));

            this.Weight = weight;
        }

        public abstract bool CanExecute();

        public GameObject Owner { get; }

        public float Weight { get; }
    }
}
