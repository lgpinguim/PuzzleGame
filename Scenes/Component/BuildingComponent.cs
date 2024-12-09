using System.Collections.Generic;
using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
    [Export(PropertyHint.File,"*.tres")] public string buildingResourcePath;
    
    public BuildingResource BuildingResource { get; private set; }
    
    public override void _Ready()
    {
        if (buildingResourcePath != null)
        {
            BuildingResource = GD.Load<BuildingResource>(buildingResourcePath);
        }
        AddToGroup(nameof(BuildingComponent));
        Callable.From(() => GameEvents.EmitBuildingPlaced(this)).CallDeferred();
    }

    public Vector2I GetGridCellPosition()
    {
        var gridposition = GlobalPosition / 64;
        gridposition = gridposition.Floor();
        return new Vector2I((int)gridposition.X, (int)gridposition.Y);
    }

    public List<Vector2I> GetOccupiedCellPositions()
    {
        var result = new List<Vector2I>();
        var gridposition = GetGridCellPosition();
        for (int x = gridposition.X; x < gridposition.X + BuildingResource.Dimensions.X; x++)
        {
            for (int y = gridposition.Y; y < gridposition.Y + BuildingResource.Dimensions.Y; y++)
            {
                result.Add(new Vector2I(x, y));
            }
        }
        
        return result;
    }

    public void DestroyBuilding()
    {
        GameEvents.EmitBuildingDestroyed(this);
        Owner.QueueFree();
    }
}