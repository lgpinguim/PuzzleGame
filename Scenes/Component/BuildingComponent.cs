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
}