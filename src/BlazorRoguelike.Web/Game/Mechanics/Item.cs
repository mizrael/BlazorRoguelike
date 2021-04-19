namespace BlazorRoguelike.Web.Game.Mechanics
{
    public record Item
    {
        public Item(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}
