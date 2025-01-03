using Game.Manager;
using Game.UI;
using Godot;

namespace Game;

public partial class BaseLevel : Node
{
	[Export]
	private PackedScene LevelCompleteScreenScene;
	
	private GridManager gridManager;
	private GoldMine goldMine;
	private GameCamera gameCamera;
	private TileMapLayer baseTerrainTileMapLayer;
	private Node2D baseBuilding;
	private GameUI gameUI;
	
	public override void _Ready()
	{
		gridManager = GetNode<GridManager>("GridManager");
		goldMine = GetNode<GoldMine>("%GoldMine");
		gameCamera = GetNode<GameCamera>("GameCamera");
		baseTerrainTileMapLayer = GetNode<TileMapLayer>("%BaseTerrainTileMapLayer");
		baseBuilding = GetNode<Node2D>("%Base");
		gameUI = GetNode<GameUI>("GameUI");
		
		gameCamera.SetBoundingRect(baseTerrainTileMapLayer.GetUsedRect());
		gameCamera.CenterOnPosition(baseBuilding.GlobalPosition);

		gridManager.GridStateUpdated += OnGridStateUpdated;

	}

	private void OnGridStateUpdated()
	{
		var goldMineTilePosition = gridManager.ConvertWolrdPositionToGridPosition(goldMine.GlobalPosition);

		if (gridManager.IsTilePositionBuildable(goldMineTilePosition))
		{
			var levelCompleteScreen = LevelCompleteScreenScene.Instantiate<LevelCompleteScreen>();
			AddChild(levelCompleteScreen);
			goldMine.SetActive();
			gameUI.HideUI();
		}
	}
}
