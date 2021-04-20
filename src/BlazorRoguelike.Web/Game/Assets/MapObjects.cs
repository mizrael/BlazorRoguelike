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
    public record MapObjects: IAsset
    {
        private readonly IDictionary<MapObjectType, MapObject[]> _byType;

        public MapObjects(string name, IEnumerable<MapObject> objects)
        {
            Name = name;
            Objects = objects ?? Enumerable.Empty<MapObject>();

            _byType = Objects.GroupBy(o => o.Type)
                            .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public string Name { get; }
        public IEnumerable<MapObject> Objects { get; }

        public MapObject GetRandomByType(MapObjectType type)
        {
            if (!_byType.ContainsKey(type))
                return null;
            var filtered = _byType[type];
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
                var type = Enum.Parse<MapObjectType>(d.type, true);
                return new MapObject(d.id, type, d.sprite);
            });

            return new MapObjects(meta.Path, mapObjects);
        }

        internal record MapObjectDTO(string id, string type, string sprite);
    }
}
