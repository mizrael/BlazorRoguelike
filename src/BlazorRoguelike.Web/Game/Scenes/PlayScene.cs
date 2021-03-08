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
            InitDungeon();
            InitCursor();

#if DEBUG
            InitUI();
#endif
            await base.EnterCore();
        }

        private void InitCursor()
        {
            var inputService = this.Game.GetService<InputService>();

            var cursor = new GameObject();
            var transform = cursor.Components.Add<TransformComponent>();

            var renderer = cursor.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("cursor");
            renderer.LayerIndex = (int)RenderLayers.UI;

            var lambda = cursor.Components.Add<LambdaComponent>();
            lambda.OnUpdate = (_, g) =>
            {
                transform.Local.Position.X = inputService.Mouse.X;
                transform.Local.Position.Y = inputService.Mouse.Y;
                return ValueTask.CompletedTask;
            };

            this.Root.AddChild(cursor);
        }

        private void InitDungeon()
        {
            var roomGenerator = new DungeonGenerator.RoomGenerator(5, 2, 3, 2, 3);
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 70, 25, 100, roomGenerator);
            var dungeon = generator.Generate();

            var map = new GameObject();

            map.Components.Add<TransformComponent>();

            var renderer = map.Components.Add<MapRenderer>();
            renderer.Dungeon = dungeon;
            renderer.Tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.LayerIndex = (int)RenderLayers.Background;

            this.Root.AddChild(map);
        }

        private void InitUI()
        {
            var ui = new GameObject();

            var debugStats = ui.Components.Add<DebugStatsUIComponent>();            
            debugStats.LayerIndex = (int)RenderLayers.UI;

            this.Root.AddChild(ui);
        }
    }
}