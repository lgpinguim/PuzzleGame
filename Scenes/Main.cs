using System.Collections.Generic;
using Godot;

namespace Game;

public partial class Main : Node
{
	private Sprite2D _cursor;
	private PackedScene _buildingScene;
	private Button _placeBuildingButton;
	private TileMapLayer _highlightTileMapLayer;
	
	private Vector2? _hoveredGridCell;
	private HashSet<Vector2> _occupiedCells = new();
	
	public override void _Ready()
	{
		_buildingScene = GD.Load<PackedScene>("res://Scenes/Building/Building.tscn");
		_cursor = GetNode<Sprite2D>("Cursor");
		_placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		_highlightTileMapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");
		
		_cursor.Visible = false;

		_placeBuildingButton.Pressed += OnButtonPressed;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (_hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !_occupiedCells.Contains(_hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			_cursor.Visible = false;
			
		}
	}
	
	public override void _Process(double delta)
	{
		var gridposition = GetMouseGridCellPosition();
		_cursor.GlobalPosition = gridposition * 64;
		
		if (_cursor.Visible && (!_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridposition))
		{
			_hoveredGridCell = gridposition;
			UpdateHighlightTileMapLayer();
		}
	}

	private Vector2 GetMouseGridCellPosition()
	{
		var mousePosition = _highlightTileMapLayer.GetGlobalMousePosition();
		var gridposition = mousePosition / 64;
		return gridposition = gridposition.Floor();
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!_hoveredGridCell.HasValue)
		{
			return;
		}
		
		var building = _buildingScene.Instantiate<Node2D>();
		AddChild(building);
		
		building.GlobalPosition = _hoveredGridCell.Value * 64;
		_occupiedCells.Add(_hoveredGridCell.Value);

		_hoveredGridCell = null;
		UpdateHighlightTileMapLayer();
	}

	private void UpdateHighlightTileMapLayer()
	{
		_highlightTileMapLayer.Clear();
		if (!_hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = _hoveredGridCell.Value.X - 3; x <= _hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = _hoveredGridCell.Value.Y -3; y <= _hoveredGridCell.Value.Y + 3; y++)
			{
				_highlightTileMapLayer.SetCell(new Vector2I((int)x, (int)y), 0,Vector2I.Zero);
			}
		}
	}

	private void OnButtonPressed()
	{
		_cursor.Visible = true;
	}
}
