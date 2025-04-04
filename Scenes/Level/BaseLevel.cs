using Game.Autoload;
using Game.Manager;
using Game.Resources.Level;
using Game.UI;
using Godot;

namespace Game;

public partial class BaseLevel : Node
{
    [Export] private PackedScene LevelCompleteScreenScene;
    [Export] private LevelDefinitionResource levelDefinitionResource;

    private GridManager gridManager;
    private GoldMine goldMine;
    private GameCamera gameCamera;
    private TileMapLayer baseTerrainTileMapLayer;
    private Node2D baseBuilding;
    private GameUI gameUI;
    private BuildingManager buildingManager;
    private bool isComplete;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>("GridManager");
        goldMine = GetNode<GoldMine>("%GoldMine");
        gameCamera = GetNode<GameCamera>("GameCamera");
        baseTerrainTileMapLayer = GetNode<TileMapLayer>("%BaseTerrainTileMapLayer");
        baseBuilding = GetNode<Node2D>("%Base");
        gameUI = GetNode<GameUI>("GameUI");
        buildingManager = GetNode<BuildingManager>("BuildingManager");

        buildingManager.SetStartingResourceCount(levelDefinitionResource.StartingResourceCount);

        gameCamera.SetBoundingRect(baseTerrainTileMapLayer.GetUsedRect());
        gameCamera.CenterOnPosition(baseBuilding.GlobalPosition);

        gridManager.GridStateUpdated += OnGridStateUpdated;
    }

    private void ShowLevelComplete()
    {
        isComplete = true;
        SaveManager.SaveLevelCompletion(levelDefinitionResource);

        var levelCompleteScreen = LevelCompleteScreenScene.Instantiate<LevelCompleteScreen>();
        AddChild(levelCompleteScreen);
        goldMine.SetActive();
        gameUI.HideUI();
    }

    private void OnGridStateUpdated()
    {
        if (isComplete) return;
        var goldMineTilePosition = gridManager.ConvertWorldPositionToTilePosition(goldMine.GlobalPosition);

        if (gridManager.IsTilePositionInAnyBuildingRadius(goldMineTilePosition))
        {
            ShowLevelComplete();
        }
    }
}