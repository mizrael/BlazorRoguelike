using System.Collections.Generic;

namespace BlazorRoguelike.Web.Game.Mechanics
{
    // TODO: this assumes that the item is always rendered using a static sprite and does not allow customization of the tileset.
    // Consider adding a key/value collection or some inheritance (based on the type perhaps), or a simple flag "is animated"
    public record MapObject(MapObjectType Type, string Id, IReadOnlyDictionary<string, object> Properties)
    {
    
    }    
}
