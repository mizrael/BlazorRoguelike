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
        private Map _map;
        private OffscreenMapRenderer _mapRenderer;

        #endregion "private members"

        public PlayScene(GameContext game, IAssetsResolver assetsResolver) : base(game)
        {
            _assetsResolver = assetsResolver;
        }

        protected override async ValueTask EnterCore()
        {
            await InitMap();

            InitCursor();
            InitPlayer();

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
            renderer.Sprite = spriteSheet.GetSprite("cursor-x");
            renderer.LayerIndex = (int)RenderLayers.UI;

            var lambda = cursor.Components.Add<LambdaComponent>();
            lambda.OnUpdate = (_, g) =>
            {
                transform.Local.Position.X = inputService.Mouse.X;
                transform.Local.Position.Y = inputService.Mouse.Y;

                var x = (int)(transform.Local.Position.X / _mapRenderer.TileWidth);
                var y = (int)(transform.Local.Position.Y / _mapRenderer.TileHeight);
                var isWalkable = _map.IsWalkable(x, y);
                var spriteName = isWalkable ? "cursor" : "cursor-x";
                renderer.Sprite = spriteSheet.GetSprite(spriteName);

                return ValueTask.CompletedTask;
            };

            this.Root.AddChild(cursor);
        }

        private async ValueTask InitMap()
        {
            var roomGenerator = new DungeonGenerator.RoomGenerator(5, 2, 3, 2, 3);
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 70, 25, 100, roomGenerator);
            var dungeon = generator.Generate();
            _map = new Map(dungeon);

            var canvas = await this.Game.Display.CanvasManager.CreateCanvas("map", new CanvasOptions() { Hidden = true });
            _mapRenderer = new OffscreenMapRenderer();
            _mapRenderer.Canvas = await canvas.CreateCanvas2DAsync();
            _mapRenderer.Map = _map;
            _mapRenderer.Tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");

            var map = new GameObject();
            map.Components.Add<TransformComponent>();
            var renderComp = map.Components.Add<MapRenderComponent>();
            renderComp.Renderer = _mapRenderer;
            renderComp.LayerIndex = (int)RenderLayers.Background;

            this.Game.Display.OnSizeChanged += () =>
            {
                _mapRenderer.ForceRendering();
            };

            this.Root.AddChild(map);
        }

        private void InitPlayer()
        {
            var playerStartTile = _map.GetRandomWalkableTile();

            var player = new GameObject();
            var transform = player.Components.Add<TransformComponent>();
            transform.Local.Position.X = playerStartTile.row * _mapRenderer.TileWidth + _mapRenderer.TileWidth/2;
            transform.Local.Position.Y = playerStartTile.col * _mapRenderer.TileHeight + _mapRenderer.TileHeight/2;

            var renderer = player.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("player-base");
            renderer.LayerIndex = (int)RenderLayers.Player;

            this.Root.AddChild(player);
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