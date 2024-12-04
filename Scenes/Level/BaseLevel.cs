using Game.Manager;
using Godot;

namespace Game;

public partial class BaseLevel : Node
{
	private GridManager gridManager;
	private GoldMine goldMine;
	
	public override void _Ready()
	{
		gridManager = GetNode<GridManager>("GridManager");
		goldMine = GetNode<GoldMine>("%GoldMine");

		gridManager.GridStateUpdated += OnGridStateUpdated;

	}

	private void OnGridStateUpdated()
	{
		var goldMineTilePosition = gridManager.ConvertWolrdPositionToGridPosition(goldMine.GlobalPosition);

		if (gridManager.IsTilePositionBuildable(goldMineTilePosition))
		{
			goldMine.SetActive();
			GD.PrintRich("[rainbow freq=1.0 sat=0.8 val=0.8] You WON! [/rainbow]");
		}
	}
}
