using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;
using BlazorRoguelike.Web.Game.Components;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class PlayScene : Scene
    {
        #region "private members"

        private readonly IAssetsResolver _assetsResolver;

        #endregion "private members"

        public PlayScene(GameContext game, IAssetsResolver assetsResolver) : base(game)
        {
            _assetsResolver = assetsResolver;
        }

        protected override async ValueTask EnterCore()
        {
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 40, 40, 60, new DungeonGenerator.RoomGenerator(4, 2, 3, 2, 3)); 
            var dungeon = generator.Generate();

            var map = new GameObject();

            map.Components.Add<TransformComponent>();

            var renderer = map.Components.Add<MapRenderer>();
            renderer.Dungeon = dungeon;
            renderer.Tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon3.json");

            this.Root.AddChild(map);

            await base.EnterCore();
        }

    }
}