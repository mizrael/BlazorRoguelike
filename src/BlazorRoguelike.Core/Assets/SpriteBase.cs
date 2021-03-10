using System;
using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace BlazorRoguelike.Core.Assets
{
    public class SpriteBase : IAsset
    {
        public SpriteBase(string name, ElementReference elementRef, Rectangle bounds)
        {
            Name = name;
            ElementRef = elementRef;
            Bounds = bounds;

            Origin = new Point((int)MathF.Floor((float)bounds.Width * .5f),
                                (int)MathF.Floor((float)bounds.Height * .5f));
        }

        public string Name { get; }
        public ElementReference ElementRef { get; set; }
        public Rectangle Bounds { get; }
        public Point Origin { get; }
    }
}