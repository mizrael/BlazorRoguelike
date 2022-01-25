using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.AI.States;
using BlazorRoguelike.Web.Game.Components;

namespace BlazorRoguelike.Web.Game.AI
{
    public static class StateMachines
    {
        public static Machine WanderAroundSpot(GameObject owner, GameContext game)
        {
            var transform = owner.Components.Get<TransformComponent>();
            var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);
            var mapRenderer = map.Components.Get<MapRenderComponent>();
            var startTile = mapRenderer.GetTileAt(transform.World.Position);

            var idle = new Idle(owner, 2f * 1000f); //TODO: move the delay to set method
            var arrive = new Arrive(owner);

            var machine = new Machine(new State[]
            {
                idle,
                arrive
            });

            machine.AddTransition(idle, arrive, s => s.IsCompleted, s =>
            {
                var currTile = mapRenderer.GetTileAt(transform.World.Position);
                var isStartTile = currTile == startTile;
                var destTile = isStartTile ? mapRenderer.Map.GetRandomEmptyTile(startTile, 5) : startTile;

                var a = s as Arrive;
                a.SetDestination(destTile);
            });

            // TODO: set a random idle delay before transitioning
            machine.AddTransition(arrive, idle, s => s.IsCompleted); 

            return machine;
        }
    }
}