using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Assets.Loaders;
using BlazorRoguelike.Core.Utils;
using BlazorRoguelike.Web.Game.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Assets
{
    public record MapObjects : IAsset
    {
        private readonly IDictionary<MapObjectType.Groups, MapObject[]> _byGroup;

        public MapObjects(string name, IEnumerable<MapObject> objects)
        {
            Name = name;
            Objects = objects ?? Enumerable.Empty<MapObject>();

            _byGroup = Objects.GroupBy(o => o.Type.Group)
                            .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public string Name { get; }
        public IEnumerable<MapObject> Objects { get; }

        public MapObject GetRandomByGroup(MapObjectType.Groups group)
        {
            if (!_byGroup.ContainsKey(group))
                return null;
            var filtered = _byGroup[group];
            var index = MathUtils.Random.Next(filtered.Length);
            return filtered[index];
        }
    }

    public class MapObjectsLoader : IAssetLoader<MapObjects>
    {
        private readonly HttpClient _httpClient;

        public MapObjectsLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<MapObjects> Load(AssetMeta meta)
        {
            var dtos = await _httpClient.GetFromJsonAsync<MapObjectRaw[]>(meta.Path);
            var mapObjects = dtos.Select(d => d.ToModel());

            return new MapObjects(meta.Path, mapObjects);
        }

        private record MapObjectRaw(string id, string type, IDictionary<string, object> properties)
        {
            public MapObject ToModel()
            {
                var type = MapObjectType.Create(this.type);
                return new MapObject(type, this.id, this.properties ?? new Dictionary<string, object>());
            }
        }
    }
}
