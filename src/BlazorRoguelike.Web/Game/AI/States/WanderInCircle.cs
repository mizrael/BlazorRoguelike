using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Components;
using BlazorRoguelike.Web.Game.Mechanics;

namespace BlazorRoguelike.Web.Game.AI.States
{
	//TODO: remove this

 //   public class WanderInCircle : State
 //   {
	//	private TileInfo _startTile;
	//	private TileInfo _currDest;
	//	private PathFollowerComponent _pathFollower;

	//	public WanderInCircle(GameObject owner) : base(owner) { }

 //       protected override void OnEnter(GameContext game)
 //       {
	//		_pathFollower = this.Owner.Components.Get<PathFollowerComponent>();
 //           _pathFollower.OnArrived += _ =>
 //           {
 //               SetNextDest();
 //           };

	//		var transform = this.Owner.Components.Get<TransformComponent>();

	//		var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);			
	//		var mapRenderer = map.Components.Get<MapRenderComponent>();

	//		_startTile = mapRenderer.GetTileAt(transform.World.Position);
			
	//		base.OnEnter(game);
 //       }

 //       private void SetNextDest()
 //       {
 //           if (_currDest == _startTile)
 //               _currDest = PickRandomTileInRange(_startTile, 10f);
 //           else
 //               _currDest = _startTile;
 //           _pathFollower.SetDestination(_currDest);
 //       }
	//}
}

