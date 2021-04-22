using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerStatsComponent : Component
    {
        private PlayerStatsComponent(GameObject owner) : base(owner)
        {
        }

        public int Health { get; private set; } = 6;
        public int MaxHealth { get; private set; } = 6;

        private const int HealthPerLevel = 2;
    }
}