using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene towerScene;
	private PackedScene villageScene;
	private Button placeBuildingButton;
	private Button placeVillageButton;
	private Node2D ySortRoot;
	
	private Vector2I? hoveredGridCell;
	private PackedScene? toPlaceBuildingScene;
	
	public override void _Ready()
	{
		towerScene = GD.Load<PackedScene>("res://Scenes/Building/Tower.tscn");
		villageScene = GD.Load<PackedScene>("res://Scenes/Building/Village.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceTowerButton");
		placeVillageButton = GetNode<Button>("PlaceVillageButton");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		
		cursor.Visible = false;

		placeBuildingButton.Pressed += OnPlaceBuildingButtonPressed;
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
		
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridposition))
		{
			hoveredGridCell = gridposition;
			gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value,3);
		}
	}
	
	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}
		
		var building = toPlaceBuildingScene.Instantiate<Node2D>();

		ySortRoot.AddChild(building);
		
		building.GlobalPosition = hoveredGridCell.Value * 64;
	
		hoveredGridCell = null;
		gridManager.ClearHighlightTiles();
	}
	
	private void OnPlaceBuildingButtonPressed()
	{
		toPlaceBuildingScene = towerScene;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}

	private void OnButtonPlaceVillagePressed()
	{
		toPlaceBuildingScene = villageScene;
		cursor.Visible = true;
		gridManager.HighlightBuildableTiles();
	}
}
