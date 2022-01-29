using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;
using System.Collections.Generic;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerInventoryComponent : Component
    {        
        private readonly List<MapObject> _inventory = new List<MapObject>(MaxSlots);

        private PlayerInventoryComponent(GameObject owner) : base(owner)
        {
        }

        public bool TryAdd(MapObject item) {
            if (item.Type.Group == MapObjectType.Groups.Collectibles &&
                _inventory.Count < MaxSlots)
            {
                _inventory.Add(item);
                return true;
            }                

            return false;
        }    

        public const int MaxSlots = 5;
    }
}