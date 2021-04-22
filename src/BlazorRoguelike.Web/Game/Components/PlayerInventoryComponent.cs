using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerInventoryComponent : Component
    {        
        private PlayerInventoryComponent(GameObject owner) : base(owner)
        {
        }

        public void Add(MapObject item) { }

        public int Potions { get; private set; } = 1;
    }
}