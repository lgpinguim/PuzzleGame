using System.Collections.Generic;
using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
	private HashSet<Vector2I> occupiedCells = new();
	
	[Export]
	private TileMapLayer highlightTileMapLayer;
	[Export]
	private TileMapLayer baseTerrainTileMapLayer;
	
	public bool IsTilePositionValid(Vector2I tilePosition)
	{
		var customData = baseTerrainTileMapLayer.GetCellTileData(tilePosition);
		if (customData is null) return false;
		if (!(bool)customData.GetCustomData("buildable")) return false;
		
		return !occupiedCells.Contains(tilePosition) ;
	}

	public void MarkTileAsOccupied(Vector2I tilePosition)
	{
		occupiedCells.Add(tilePosition);
	}

	public void HighlightValidTilesInRadius(Vector2I rootCell, int radius)
	{
		ClearHighlightTiles();
		
		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y -radius; y <= rootCell.Y + radius; y++)
			{
				var tilePosition = new Vector2I(x, y);
				if(!IsTilePositionValid(tilePosition)) continue;
				highlightTileMapLayer.SetCell(tilePosition, 0,Vector2I.Zero);
			}
		}
	}

	public void ClearHighlightTiles()
	{
		highlightTileMapLayer.Clear();
	}
	
	public Vector2I GetMouseGridCellPosition()
	{
		var mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
		var gridposition = mousePosition / 64;
		gridposition = gridposition.Floor();
		 return new Vector2I((int)gridposition.X, (int)gridposition.Y);
	}
}
