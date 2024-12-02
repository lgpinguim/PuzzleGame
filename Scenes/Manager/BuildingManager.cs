using System;
using Game.Building;
using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class BuildingManager : Node
{
    private readonly StringName ACTION_LEFT_CLICK = "left_click";
    private readonly StringName ACTION_CANCEL = "cancel";
    private readonly StringName ACTION_RIGHT_CLICK = "right_click";

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
    private int startingResourceCount = 4;
    private int currentlyUsedResourceCount;
    private BuildingResource toPlaceBuildingResource;
    private Vector2I hoveredGridCell;
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
                         IsBuildingPlaceableAtTile(hoveredGridCell))
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
        var gridposition = gridManager.GetMouseGridCellPosition();

        if (hoveredGridCell != gridposition)
        {
            hoveredGridCell = gridposition;
            UpdateHoveredGridCell();
        }

        switch (currentState)
        {
            case State.Normal:
                break;

            case State.PlacingBuilding:
                buildingGhost.GlobalPosition = gridposition * 64;
                break;
        }
    }

    private void UpdateGridDisplay()
    {
        gridManager.ClearHighlightTiles();
        gridManager.HighlightBuildableTiles();

        if (!IsBuildingPlaceableAtTile(hoveredGridCell))
        {
            buildingGhost.SetInvalid();
            return;
        }

        gridManager.HighlightExpandedBuildableTiles(hoveredGridCell, toPlaceBuildingResource.BuildableRadius);
        gridManager.HighlightResourceTiles(hoveredGridCell, toPlaceBuildingResource.ResourceRadius);
        buildingGhost.SetValid();
    }

    private void PlaceBuildingAtHoveredCellPosition()
    {
        var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        ySortRoot.AddChild(building);

        building.GlobalPosition = hoveredGridCell * 64;
        currentlyUsedResourceCount += toPlaceBuildingResource.ResourceCost;

        ChangeState(State.Normal);
    }

    private void DestroyBuildingAtHoveredCellPosition()
    {
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

    private bool IsBuildingPlaceableAtTile(Vector2I tilePosition)
    {
        return gridManager.IsTilePositionBuildable(tilePosition) &&
               AvailableResourceCount >= toPlaceBuildingResource.ResourceCost;
    }

    private void UpdateHoveredGridCell()
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
        var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
        buildingGhost.AddChild(buildingSprite);
        toPlaceBuildingResource = buildingResource;
        UpdateGridDisplay();
    }
}