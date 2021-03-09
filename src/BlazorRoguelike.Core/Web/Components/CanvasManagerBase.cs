using System.Collections.Generic;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components;

namespace BlazorRoguelike.Core.Web.Components
{
    public abstract class CanvasManagerBase : ComponentBase 
    {
        protected readonly Dictionary<string, CanvasOptions> _names = new ();
        protected readonly Dictionary<string, BECanvasComponent> _canvases = new();

        public void CreateCanvas(string name, CanvasOptions options = null)
        {
            options ??= CanvasOptions.Defaults;
            _names.Add(name, options);
            this.StateHasChanged();
        }       

        public BECanvasComponent GetCanvas(string name) => _canvases[name];
    }

    public record CanvasOptions(bool Hidden = false, bool Primary = false){
        public static CanvasOptions Defaults = new CanvasOptions(false, true);
        public static CanvasOptions Offscreen = new CanvasOptions(false, false);
    }
}