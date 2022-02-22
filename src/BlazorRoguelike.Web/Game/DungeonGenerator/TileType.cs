namespace BlazorRoguelike.Web.Game.DungeonGenerator
{
    public enum TileType
    {
        WallSN = 178,
        WallNS,

        WallSE,
        WallSO,
        
        WallNE,
        WallNO,        
        
        WallEO,
        WallOE,

        WallESO,
        WallNEO,
        WallNES,
        WallNSO,

        WallNESO,

        Door = 210,

        Void = 248,
        Empty = 249
    }
}