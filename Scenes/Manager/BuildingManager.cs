using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class BuildingManager : Node
{
    [Export] private GridManager gridManager;
    [Export] private GameUI gameUI;
    [Export] private Node2D ySortRoot;
    [Export] private Node2D cursor;

    private int currentResourceCount;
    private int startingResourceCount = 4;
    private int currentlyUsedResourceCount;
    private BuildingResource toPlaceBuildingResource;
    private Vector2I? hoveredGridCell;
    
    private int AvailableResourceCount => startingResourceCount + currentResourceCount - currentlyUsedResourceCount;
    

    public override void _Ready()
    {
        gameUI.BuildingResourceSelected += OnBuildingResourceSelected;
        gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (
            hoveredGridCell.HasValue &&
            toPlaceBuildingResource != null &&
            evt.IsActionPressed("left_click") &&
            gridManager.IsTilePositionBuildable(hoveredGridCell.Value) &&
            AvailableResourceCount >= toPlaceBuildingResource.ResourceCost
            )
        {
            PlaceBuildingAtHoveredCellPosition();
            cursor.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        var gridposition = gridManager.GetMouseGridCellPosition();
        cursor.GlobalPosition = gridposition * 64;

        if (toPlaceBuildingResource != null && cursor.Visible &&
            (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridposition))
        {
            hoveredGridCell = gridposition;
            gridManager.ClearHighlightTiles();
            gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, toPlaceBuildingResource.BuildableRadius);
            gridManager.HighlightResourceTiles(hoveredGridCell.Value, toPlaceBuildingResource.ResourceRadius);
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
        currentlyUsedResourceCount += toPlaceBuildingResource.ResourceCost;
        GD.Print(AvailableResourceCount);
    }

    private void OnResourceTilesUpdated(int resourceCount)
    {
        currentResourceCount = resourceCount;
    }

    private void OnBuildingResourceSelected(BuildingResource buildingResource)
    {
        toPlaceBuildingResource = buildingResource;
        cursor.Visible = true;
        gridManager.HighlightBuildableTiles();
    }
}