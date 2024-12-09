using System;
using System.Collections.Generic;
using System.Linq;
using Game.Building;
using Game.Component;
using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class BuildingManager : Node
{
    private readonly StringName ACTION_LEFT_CLICK = "left_click";
    private readonly StringName ACTION_CANCEL = "cancel";
    private readonly StringName ACTION_RIGHT_CLICK = "right_click";

    [Export] private int startingResourceCount = 4;
    [Export] private GridManager gridManager;
    [Export] private GameUI gameUI;
    [Export] private Node2D ySortRoot;
    [Export] private PackedScene buildingGhostScene;

    private enum State
    {
        Normal,
        PlacingBuilding,
    }

    private int currentResourceCount;

    private int currentlyUsedResourceCount;
    private BuildingResource toPlaceBuildingResource;
    private Rect2I hoveredGridArea =new Rect2I(Vector2I.Zero, Vector2I.One);
    private BuildingGhost buildingGhost;
    private State currentState;

    private int AvailableResourceCount => startingResourceCount + currentResourceCount - currentlyUsedResourceCount;


    public override void _Ready()
    {
        gameUI.BuildingResourceSelected += OnBuildingResourceSelected;
        gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        switch (currentState)
        {
            case State.Normal:
                if (evt.IsActionPressed(ACTION_LEFT_CLICK))
                {
                    DestroyBuildingAtHoveredCellPosition();
                }

                break;

            case State.PlacingBuilding:
                if (evt.IsActionPressed(ACTION_CANCEL))
                {
                    ChangeState(State.Normal);
                }

                else if (toPlaceBuildingResource != null &&
                         evt.IsActionPressed(ACTION_LEFT_CLICK) &&
                         IsBuildingPlaceableAtArea(hoveredGridArea))
                {
                    PlaceBuildingAtHoveredCellPosition();
                }

                break;

            default:
                break;
        }
    }

    public override void _Process(double delta)
    {
        var mouseGridPosition = gridManager.GetMouseGridCellPosition();
        var rootCell = hoveredGridArea.Position;
        if (rootCell != mouseGridPosition)
        {
            hoveredGridArea.Position = mouseGridPosition;
            UpdateHoveredGridArea();
        }

        switch (currentState)
        {
            case State.Normal:
                break;

            case State.PlacingBuilding:
                buildingGhost.GlobalPosition = mouseGridPosition * 64;
                break;
        }
    }

    private void UpdateGridDisplay()
    {
        gridManager.ClearHighlightTiles();
        gridManager.HighlightBuildableTiles();

        if (!IsBuildingPlaceableAtArea(hoveredGridArea))
        {
            buildingGhost.SetInvalid();
            return;
        }

        gridManager.HighlightExpandedBuildableTiles(hoveredGridArea, toPlaceBuildingResource.BuildableRadius);
        gridManager.HighlightResourceTiles(hoveredGridArea, toPlaceBuildingResource.ResourceRadius);
        buildingGhost.SetValid();
    }

    private void PlaceBuildingAtHoveredCellPosition()
    {
        var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        ySortRoot.AddChild(building);

        building.GlobalPosition = hoveredGridArea.Position * 64;
        currentlyUsedResourceCount += toPlaceBuildingResource.ResourceCost;

        ChangeState(State.Normal);
    }

    private void DestroyBuildingAtHoveredCellPosition()
    {
        var rootCell = hoveredGridArea.Position;
        var buildingComponent = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>()
            .FirstOrDefault((BuildingComponent) =>
            {
                return BuildingComponent.BuildingResource.IsDeletable &&
                       BuildingComponent.GetGridCellPosition() == rootCell;
            });
        if (buildingComponent == null) return;
        
        currentlyUsedResourceCount -= buildingComponent.BuildingResource.ResourceCost;
        buildingComponent.DestroyBuilding();
    }

    private void ClearBuildingGhost()
    {
        gridManager.ClearHighlightTiles();
        if (IsInstanceValid(buildingGhost))
        {
            buildingGhost.QueueFree();
        }

        buildingGhost = null;
    }

    private bool IsBuildingPlaceableAtArea(Rect2I tileArea)
    {
        var tilesInArea = GetTilePositionsInTileArea(tileArea);
        var allTilesBuildable = tilesInArea.All((tilePosition) => gridManager.IsTilePositionBuildable(tilePosition));
        return allTilesBuildable && AvailableResourceCount >= toPlaceBuildingResource.ResourceCost;
    }

    private List<Vector2I> GetTilePositionsInTileArea(Rect2I tileArea)
    {
        var result = new List<Vector2I>();
        for (int x = tileArea.Position.X; x < tileArea.End.X; x++)
        {
            for (int y = tileArea.Position.Y; y < tileArea.End.Y; y++)
            {
                result.Add(new Vector2I(x, y));
            }
        }
        return result;
    }

    private void UpdateHoveredGridArea()
    {
        switch (currentState)
        {
            case State.Normal:
                break;

            case State.PlacingBuilding:
                UpdateGridDisplay();
                break;
        }
    }

    private void ChangeState(State toState)
    {
        switch (currentState)
        {
            case State.Normal:
                break;

            case State.PlacingBuilding:
                ClearBuildingGhost();
                toPlaceBuildingResource = null;
                break;
        }

        currentState = toState;

        switch (currentState)
        {
            case State.Normal:
                break;

            case State.PlacingBuilding:
                buildingGhost = buildingGhostScene.Instantiate<BuildingGhost>();
                ySortRoot.AddChild(buildingGhost);
                break;

            default:
                break;
        }
    }

    private void OnResourceTilesUpdated(int resourceCount)
    {
        currentResourceCount = resourceCount;
    }

    private void OnBuildingResourceSelected(BuildingResource buildingResource)
    {
        ChangeState(State.PlacingBuilding);
        hoveredGridArea.Size = buildingResource.Dimensions;
        var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
        buildingGhost.AddChild(buildingSprite);
        toPlaceBuildingResource = buildingResource;
        UpdateGridDisplay();
    }
}