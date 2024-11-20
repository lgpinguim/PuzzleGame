using Game.Autoload;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export]
	public int BuildableRadius { get; private set; }
	
	
	public override void _Ready()
	{
		AddToGroup(nameof(BuildingComponent));
		Callable.From(() => GameEvents.EmitBuildingPlaced(this)).CallDeferred();
	}

	public Vector2I GetGridCellPosition()
	{
		var gridposition = GlobalPosition / 64;
		gridposition = gridposition.Floor();
		return new Vector2I((int)gridposition.X, (int)gridposition.Y);
	}
	
}
