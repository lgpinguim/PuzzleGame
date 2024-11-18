using System.Collections.Generic;
using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
	private HashSet<Vector2> occupiedCells = new();
	
	[Export]
	private TileMapLayer highlightTileMapLayer;
	[Export]
	private TileMapLayer baseTerrainTileMapLayer;
	
	public override void _Ready()
	{
	}

	public bool IsTilePositionValid(Vector2 tilePosition)
	{
		return !occupiedCells.Contains(tilePosition);
	}

	public void MarkTileAsOccupied(Vector2 tilePosition)
	{
		occupiedCells.Add(tilePosition);
	}

	public void HighlightValidTilesInRadius(Vector2 rootCell, int radius)
	{
		ClearHighlightTiles();
		
		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y -radius; y <= rootCell.Y + radius; y++)
			{
				if(!IsTilePositionValid(new Vector2(x, y))) continue;
				highlightTileMapLayer.SetCell(new Vector2I((int)x, (int)y), 0,Vector2I.Zero);
			}
		}
	}

	public void ClearHighlightTiles()
	{
		highlightTileMapLayer.Clear();
	}
	
	public Vector2 GetMouseGridCellPosition()
	{
		var mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
		var gridposition = mousePosition / 64;
		return gridposition = gridposition.Floor();
	}
	
}
