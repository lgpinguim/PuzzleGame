using Game.Manager;
using Game.Resources.Building;
using Godot;

namespace Game;

public partial class Main : Node
{
	private GridManager gridManager;
	private Sprite2D cursor;
	private BuildingResource towerResource;
	private BuildingResource villageResource;
	private Button placeBuildingButton;
	private Button placeVillageButton;
	private Node2D ySortRoot;
	
	private Vector2I? hoveredGridCell;
	private BuildingResource toPlaceBuildingResource;
	
	public override void _Ready()
	{
		towerResource = GD.Load<BuildingResource>("res://Resources/Building/tower.tres");
		villageResource = GD.Load<BuildingResource>("res://Resources/Building/village.tres");
		gridManager = GetNode<GridManager>("GridManager");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceTowerButton");
		placeVillageButton = GetNode<Button>("PlaceVillageButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		
		cursor.Visible = false;

		placeBuildingButton.Pressed += OnPlaceTowerButtonPressed;
		placeVillageButton.Pressed += OnButtonPlaceVillagePressed;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionBuildable(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
			
		}
	}
	
	public override void _Process(double delta)
	{
		var gridposition = gridManager.GetMouseGridCellPosition();
		cursor.GlobalPosition = gridposition * 64;
		
		if (toPlaceBuildingResource != null && cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridposition))
		{
			hoveredGridCell = gridposition;
			gridManager.ClearHighlightTiles();
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value,toPlaceBuildingResource.BuildableRadius);
			gridManager.HighlightResourceTiles(hoveredGridCell.Value,toPlaceBuildingResource.ResourceRadius);
		}
	}
	
	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}
		
		var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();

		ySortRoot.AddChild(building);
		
		building.GlobalPosition = hoveredGridCell.Value * 64;
	
		hoveredGridCell = null;
		gridManager.ClearHighlightTiles();
	}
	
	private void OnPlaceTowerButtonPressed()
	{
		toPlaceBuildingResource = towerResource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnButtonPlaceVillagePressed()
	{
		toPlaceBuildingResource = villageResource;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}
}
