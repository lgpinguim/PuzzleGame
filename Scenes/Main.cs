using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	
	private Vector2I? hoveredGridCell;
	
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://Scenes/Building/Building.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		
		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
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
			gridManager.HighlightBuildableTiles();
		}
	}
	
	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}
		
		var building = buildingScene.Instantiate<Node2D>();

		AddChild(building);
		
		building.GlobalPosition = hoveredGridCell.Value * 64;
	
		hoveredGridCell = null;
		gridManager.ClearHighlightTiles();
	}
	
	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
