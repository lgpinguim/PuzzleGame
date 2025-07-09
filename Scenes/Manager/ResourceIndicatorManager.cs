using System.Collections.Generic;
using System.Linq;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class ResourceIndicatorManager : Node
{
    [Export]
    private GridManager gridManager;
    
    [Export]
    private PackedScene resourceIndicatorScene;
    
    private HashSet<Vector2I> indicatedTiles = new ();
    private Dictionary<Vector2I, ResourceIndicator> tileToResourceindicator = new ();

    public override void _Ready()
    {
        gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
    }

    private void UpdateIndicators(IEnumerable<Vector2I> newIndicatedTiles, IEnumerable<Vector2I> toRemoveTiles)
    {
        foreach (var newTile in newIndicatedTiles)
        {
            var indicator = resourceIndicatorScene.Instantiate<ResourceIndicator>();
            AddChild(indicator);
            indicator.GlobalPosition = newTile * 64;
            tileToResourceindicator[newTile] = indicator;
        }

        foreach (var removeTile in toRemoveTiles)
        {
            tileToResourceindicator.TryGetValue(removeTile, out var indicator);
            if (IsInstanceValid(indicator))
            {
                indicator.Destroy();
            }
            tileToResourceindicator.Remove(removeTile);
        }
    }

    private void HandleResourceTilesUpdated()
    {
        var currentResourceTiles = gridManager.GetCollectedResourceTiles();
        var newlyIndicatedTiles = currentResourceTiles.Except(indicatedTiles);
        var toRemoveTiles = indicatedTiles.Except(currentResourceTiles);
        indicatedTiles = currentResourceTiles;
        UpdateIndicators(newlyIndicatedTiles, toRemoveTiles);
    }

    private void OnResourceTilesUpdated(int _)
    {
        Callable.From(HandleResourceTilesUpdated).CallDeferred();
    }
}
