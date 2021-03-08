using System;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Core.Exceptions
{
    public class ComponentNotFoundException<TC> : Exception where TC : Component
    {
        public ComponentNotFoundException() : base($"{typeof(TC).Name} not found on owner")
        {
        }
    }
}