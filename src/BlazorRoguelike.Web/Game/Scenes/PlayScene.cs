using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;
using Blazor.Extensions;
using BlazorRoguelike.Web.Game.Components;
using BlazorRoguelike.Core.Web.Components;
using System;
using BlazorRoguelike.Web.Game.Mechanics;
using BlazorRoguelike.Web.Game.Assets;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class PlayScene : Scene
    {
        #region private members

        private readonly IAssetsResolver _assetsResolver;
        private readonly CollisionService _collisionService;
        private Map _map;
        private MapRenderComponent _mapRenderer;

        #endregion private members

        public PlayScene(GameContext game, IAssetsResolver assetsResolver, CollisionService collisionService) : base(game)
        {
            _assetsResolver = assetsResolver;
            _collisionService = collisionService;
        }

        protected override async ValueTask EnterCore()
        {
            await InitMap();

            InitCursor();
            InitMovementCursor();

            var player = InitPlayer();
            InitUI(player);

            await base.EnterCore();
        }

        private void InitCursor()
        {
            var inputService = this.Game.GetService<InputService>();

            var cursor = new GameObject(this);
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

            var movementCursor = new GameObject(this, ObjectNames.MovementCursor);
            var transform = movementCursor.Components.Add<TransformComponent>();

            var renderer = movementCursor.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("cursor-move");
            renderer.LayerIndex = (int)RenderLayers.UI;

            var lambda = movementCursor.Components.Add<LambdaComponent>();
            lambda.OnUpdate = (_, g) =>
            {
                if (!movementCursor.Enabled)
                    return ValueTask.CompletedTask;

                transform.Local.Scale.Y = transform.Local.Scale.X = 1f + MathF.Sin(g.GameTime.TotalMilliseconds * 0.005f) * 0.5f;

                return ValueTask.CompletedTask;
            };

            movementCursor.Enabled = false;

            this.Root.AddChild(movementCursor);
        }

        private async ValueTask InitMap()
        {
            var availableMapObjects = _assetsResolver.Get<MapObjects>("assets/map-objects.json");

            var roomGenerator = new DungeonGenerator.RoomGenerator(5, 2, 3, 2, 3);
            var generator = new DungeonGenerator.DungeonGenerator(9, 7, 70, 25, 100, roomGenerator);
            var dungeon = generator.Generate();
            _map = new Map(dungeon, availableMapObjects);

            var canvas = await this.Game.Display.CanvasManager.CreateCanvas("map", new CanvasOptions() { Hidden = true });
            var canvasContext = await canvas.CreateCanvas2DAsync();
            var tileset = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            var offscreenRenderer = new OffscreenMapRenderer(canvasContext, tileset);
            offscreenRenderer.Map = _map;

            var map = new GameObject(this, ObjectNames.Map);
            map.Components.Add<TransformComponent>();
            _mapRenderer = map.Components.Add<MapRenderComponent>();
            _mapRenderer.OffscreenRenderer = offscreenRenderer;
            _mapRenderer.LayerIndex = (int)RenderLayers.Background;

            this.Game.Display.OnSizeChanged += () =>
            {
                offscreenRenderer.ForceRendering();
            };

            this.Root.AddChild(map);

            InitMapObjects(tileset, map);
        }

        private void InitMapObjects(SpriteSheet tileset, GameObject map)
        {
            foreach (var item in _map.Objects)
            {
                if (string.IsNullOrWhiteSpace(item.mapObject.SpriteName))
                    continue;

                var sprite = tileset.GetSprite(item.mapObject.SpriteName);
                if (null == sprite)
                    continue;

                var mapObject = new GameObject(this);          
                
                var transform = mapObject.Components.Add<TransformComponent>();
                transform.Local.Position = _mapRenderer.GetTilePos(item.tile);

                var renderer = mapObject.Components.Add<SpriteRenderComponent>();
                renderer.Sprite = sprite;

                var bbox = mapObject.Components.Add<BoundingBoxComponent>();
                bbox.SetSize(sprite.Bounds.Size);

                _collisionService.RegisterCollider(bbox);

                switch (item.mapObject.Type)
                {
                    case MapObjectType.Item:
                        renderer.LayerIndex = (int)RenderLayers.Items;
                        var brain = mapObject.Components.Add<GroundItemBrainComponent>();
                        brain.Item = item.mapObject;
                        break;
                    case MapObjectType.Enemy:
                        renderer.LayerIndex = (int)RenderLayers.Enemies;
                        break;
                }

                map.AddChild(mapObject);
            }
        }

        private GameObject InitPlayer()
        {
            var playerStartTile = _map.GetRandomEmptyTile();

            var player = new GameObject(this, ObjectNames.Player);

            var transform = player.Components.Add<TransformComponent>();
            transform.Local.Position = _mapRenderer.GetTilePos(playerStartTile);

            var renderer = player.Components.Add<SpriteRenderComponent>();
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");
            renderer.Sprite = spriteSheet.GetSprite("player-base");
            renderer.LayerIndex = (int)RenderLayers.Player;

            var bbox = player.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(renderer.Sprite.Bounds.Size);
            _collisionService.RegisterCollider(bbox);

            player.Components.Add<PathFollowerComponent>();
            player.Components.Add<PlayerBrainComponent>();
            player.Components.Add<PlayerStatsComponent>();
            player.Components.Add<PlayerInventoryComponent>();

            this.Root.AddChild(player);

            return player;
        }

        private void InitUI(GameObject player)
        {
            var ui = new GameObject(this);

#if DEBUG
       //     var debugStats = ui.Components.Add<DebugStatsUIComponent>();
#endif
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/tilesets/dungeon4.json");

            var playerUI = ui.Components.Add<PlayerUIComponent>();
            playerUI.Player = player;
            playerUI.PotionSprite = spriteSheet.GetSprite("ui-potion3");
            playerUI.HeartSprite = spriteSheet.GetSprite("ui-heart3");

            this.Root.AddChild(ui);
        }
    }
}