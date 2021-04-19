using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class GroundItemComponent : Component
    {
        private GroundItemComponent(GameObject owner) : base(owner)
        {
        }

        public Item Item { get; set; }
    }
}
