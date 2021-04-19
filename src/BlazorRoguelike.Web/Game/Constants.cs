namespace BlazorRoguelike.Web.Game
{
    public enum RenderLayers
    {
        Background = 0,
        Enemies,
        Player,
        Items,
        UI
    }

    public sealed class SceneNames{
        public const string Play = "play";
    }

    public sealed class ObjectNames{
        public const string Player = "player";
        public const string Map = "map";
        public const string MovementCursor = "movementCursor";
    }
}