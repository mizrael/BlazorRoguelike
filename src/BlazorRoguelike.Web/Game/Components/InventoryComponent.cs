using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;

namespace BlazorRoguelike.Web.Game.Components
{
    public class InventoryComponent : Component
    {
        private InventoryComponent(GameObject owner) : base(owner)
        {
        }

        public void Add(MapObject item) { }
    }
}