using System.Collections.Generic;
using System.Linq;
using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
    [Export(PropertyHint.File,"*.tres")] 
    private string buildingResourcePath;
    
    [Export]
    private BuildingAnimatorComponent buildingAnimatorComponent;
    
    public BuildingResource BuildingResource { get; private set; }
    
    public bool IsDestroying { get; private set; }
    
    private HashSet<Vector2I> occupiedTiles = new();

    public static IEnumerable<BuildingComponent> GetValidBuildingComponents(Node node)
    {
        return node.GetTree()
            .GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>()
            .Where(buildingComponent => !buildingComponent.IsDestroying);
    }

    public static IEnumerable<BuildingComponent> GetDangerBuildingComponents(Node node)
    {
        return GetValidBuildingComponents(node).Where(buildingComponent => buildingComponent.BuildingResource.IsDangerBuilding());
    }
    
    public override void _Ready()
    {
        if (buildingResourcePath != null)
        {
            BuildingResource = GD.Load<BuildingResource>(buildingResourcePath);
        }

        if (buildingAnimatorComponent != null)
        {
            buildingAnimatorComponent.DestroyAnimationFinished += OnDestroyAnimationFinished;
        }
        
        AddToGroup(nameof(BuildingComponent));
        Callable.From(Initialize).CallDeferred();
    }

    public Vector2I GetGridCellPosition()
    {
        var gridposition = GlobalPosition / 64;
        gridposition = gridposition.Floor();
        return new Vector2I((int)gridposition.X, (int)gridposition.Y);
    }

    public HashSet<Vector2I> GetOccupiedCellPositions()
    {
        return occupiedTiles.ToHashSet();
    }

    public Rect2I GetTileArea()
    {
        var rootCell = GetGridCellPosition();
        return new Rect2I(rootCell, BuildingResource.Dimensions);
    }

    public bool IsTileInBuildingArea(Vector2I tilePosition)
    {
        return occupiedTiles.Contains(tilePosition);
    }

    public void Destroy()
    {
        IsDestroying = true;
        GameEvents.EmitBuildingDestroyed(this);
        buildingAnimatorComponent?.PlayDestroyAnimation();
        if (buildingAnimatorComponent == null)
        {
            Owner.QueueFree();
        }
    }
    
    private void CalculateOccupiedCellPositions()
    {
        var gridposition = GetGridCellPosition();
        for (int x = gridposition.X; x < gridposition.X + BuildingResource.Dimensions.X; x++)
        {
            for (int y = gridposition.Y; y < gridposition.Y + BuildingResource.Dimensions.Y; y++)
            {
                occupiedTiles.Add(new Vector2I(x, y));
            }
        }
    }
    
    private void Initialize()
    {
        CalculateOccupiedCellPositions();
        GameEvents.EmitBuildingPlaced(this);
    }

    private void OnDestroyAnimationFinished()
    {
        Owner.QueueFree();
    }
}