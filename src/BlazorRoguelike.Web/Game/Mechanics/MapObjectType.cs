using System.Collections.Generic;

namespace BlazorRoguelike.Web.Game.Mechanics
{
    public record MapObjectType
    {
        private MapObjectType(MapObjectType.Groups group, string name)
        {
            Group = group;
            Name = name;
        }

        public Groups Group { get; }
        public string Name { get; }

        public enum Groups
        {
            Unknown = 0,
            Collectibles,
            Enemies
        }

        private static Dictionary<string, MapObjectType> _byGroup = new()
        {
            { "enemy", new MapObjectType(MapObjectType.Groups.Enemies, "enemy") },
            { "weapon", new MapObjectType(MapObjectType.Groups.Collectibles, "weapon") },
            { "potion", new MapObjectType(MapObjectType.Groups.Collectibles, "potion") },
        };

        public static MapObjectType Create(string type)
        {
            if (_byGroup.TryGetValue(type.ToLower(), out MapObjectType mapObjectType))
                return mapObjectType;

            return new MapObjectType(MapObjectType.Groups.Unknown, type);
        }
    }
}
