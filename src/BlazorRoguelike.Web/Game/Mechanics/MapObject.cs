using Blazor.Extensions.Canvas.WebGL;
using System.Collections.Generic;
using System.Text.Json;

namespace BlazorRoguelike.Web.Game.Mechanics
{
    public record MapObject
    {
        private IDictionary<string, object> _properties;

        public MapObject(MapObjectType type, string id, IDictionary<string, object> properties)
        {
            Type = type;
            Id = id;
            _properties = properties;
        }

        public bool TryGetProperty<TP>(string propName, out TP result)
        {
            result = default(TP);

            if (!_properties.ContainsKey(propName))
                return false;

            var value = _properties[propName];
            if (value is TP p)
            {
                result = p;
                return true;
            }

            if(value is JsonElement je)
            {
                result = je.Deserialize<TP>();
                _properties[propName] = result;
                return true;
            } 
            return false;            
        }

        public MapObjectType Type { get; }
        public string Id { get; }        
    }    
}
