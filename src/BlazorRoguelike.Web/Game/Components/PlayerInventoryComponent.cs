using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;
using System;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerInventoryComponent : Component
    {        
        private PlayerInventoryComponent(GameObject owner) : base(owner)
        {
        }

        public void Add(MapObject item) {
            if (item.Id == "potion")
                this.Potions++;
        }

        private const int MaxPotions = 3;
        public int Potions { get; private set; } = 1;

        public bool CanAdd(MapObject item)
        {
            if (item.Id == "potion")
                return this.Potions <= MaxPotions;

            return false;
        }
    }
}