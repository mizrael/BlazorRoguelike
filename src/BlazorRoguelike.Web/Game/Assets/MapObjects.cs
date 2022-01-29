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
            var dtos = await _httpClient.GetFromJsonAsync<MapObjectDTO[]>(meta.Path);
            var mapObjects = dtos.Select(d => {
                var type = MapObjectType.Create(d.type);
                return new MapObject(type, d.id, d.properties ?? new Dictionary<string, object>());
            });

            return new MapObjects(meta.Path, mapObjects);
        }

        internal record MapObjectDTO(string id, string type, IReadOnlyDictionary<string, object> properties);
    }
}
