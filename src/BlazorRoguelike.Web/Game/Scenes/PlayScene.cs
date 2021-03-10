using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;
using Blazor.Extensions;
using BlazorRoguelike.Web.Game.Components;
using BlazorRoguelike.Core.Web.Components;

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
            await InitDungeon();

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

        private async ValueTask InitDungeon()
        {
            var roomGenerator = new DungeonGenerator.RoomGenerator(5, 2, 3, 2, 3);
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 70, 25, 100, roomGenerator);
            var dungeon = generator.Generate();

            var mapRenderer = new OffscreenMapRenderer();
            var canvas = await this.Game.Display.CanvasManager.CreateCanvas("map", new CanvasOptions(){ Hidden = true});
            mapRenderer.Canvas = await canvas.CreateCanvas2DAsync();
            mapRenderer.Dungeon = dungeon;
            mapRenderer.Tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");

            var map = new GameObject();
            map.Components.Add<TransformComponent>();
            var renderComp = map.Components.Add<MapRenderComponent>();
            renderComp.Renderer = mapRenderer;
            renderComp.LayerIndex = (int)RenderLayers.Background;

            this.Game.Display.OnSizeChanged += () =>
            {
                mapRenderer.ForceRendering();
            };

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