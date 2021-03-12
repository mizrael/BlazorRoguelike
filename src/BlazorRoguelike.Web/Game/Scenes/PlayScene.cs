using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;
using Blazor.Extensions;
using BlazorRoguelike.Web.Game.Components;
using BlazorRoguelike.Core.Web.Components;
using System;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class PlayScene : Scene
    {
        #region "private members"

        private readonly IAssetsResolver _assetsResolver;
        private Map _map;
        private MapRenderComponent _mapRenderer;
        private GameObject _movementCursor;

        #endregion "private members"

        public PlayScene(GameContext game, IAssetsResolver assetsResolver) : base(game)
        {
            _assetsResolver = assetsResolver;
        }

        protected override async ValueTask EnterCore()
        {
            await InitMap();

            InitCursor();
            InitMovementCursor();

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

                var tile = _mapRenderer.GetTileAt(transform.Local.Position);
                var spriteName = tile.IsWalkable ? "cursor" : "cursor-x";
                renderer.Sprite = spriteSheet.GetSprite(spriteName);

                return ValueTask.CompletedTask;
            };

            this.Root.AddChild(cursor);
        }

        private void InitMovementCursor()
        {
            var inputService = this.Game.GetService<InputService>();

            _movementCursor = new GameObject();
            var transform = _movementCursor.Components.Add<TransformComponent>();

            var renderer = _movementCursor.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("cursor-move");
            renderer.LayerIndex = (int)RenderLayers.UI;

            var lambda = _movementCursor.Components.Add<LambdaComponent>();
            lambda.OnUpdate = (_, g) =>
            {
                if (!_movementCursor.Enabled)
                    return ValueTask.CompletedTask;

                transform.Local.Scale.Y = transform.Local.Scale.X = 1f + MathF.Sin(g.GameTime.TotalMilliseconds * 0.005f) * 0.5f;

                return ValueTask.CompletedTask;
            };

            _movementCursor.Enabled = false;

            this.Root.AddChild(_movementCursor);
        }

        private async ValueTask InitMap()
        {
            var roomGenerator = new DungeonGenerator.RoomGenerator(5, 2, 3, 2, 3);
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 70, 25, 100, roomGenerator);
            var dungeon = generator.Generate();
            _map = new Map(dungeon);

            var canvas = await this.Game.Display.CanvasManager.CreateCanvas("map", new CanvasOptions() { Hidden = true });
            var offscreenRenderer = new OffscreenMapRenderer();
            offscreenRenderer.Canvas = await canvas.CreateCanvas2DAsync();
            offscreenRenderer.Map = _map;
            offscreenRenderer.Tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");

            var map = new GameObject();
            map.Components.Add<TransformComponent>();
            _mapRenderer = map.Components.Add<MapRenderComponent>();
            _mapRenderer.OffscreenRenderer = offscreenRenderer;
            _mapRenderer.LayerIndex = (int)RenderLayers.Background;

            this.Game.Display.OnSizeChanged += () =>
            {
                offscreenRenderer.ForceRendering();
            };

            this.Root.AddChild(map);
        }

        private void InitPlayer()
        {
            var playerStartTile = _map.GetRandomEmptyTile();

            var player = new GameObject();
            var transform = player.Components.Add<TransformComponent>();
            transform.Local.Position = _mapRenderer.GetTilePos(playerStartTile);

            var renderer = player.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("player-base");
            renderer.LayerIndex = (int)RenderLayers.Player;

            var brain = player.Components.Add<PlayerBrain>();
            brain.MapRenderer = _mapRenderer;
            brain.MovementCursor = _movementCursor;

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