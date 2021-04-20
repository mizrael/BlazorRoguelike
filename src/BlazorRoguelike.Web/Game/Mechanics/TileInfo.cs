namespace BlazorRoguelike.Web.Game.Mechanics
{
    public record TileInfo(int Row, int Col, DungeonGenerator.TileType Type)
    {
        public bool IsWalkable => Type == DungeonGenerator.TileType.Door || Type == DungeonGenerator.TileType.Empty;
        public static readonly TileInfo Void = new TileInfo(-1, -1, DungeonGenerator.TileType.Void);
    }
}
