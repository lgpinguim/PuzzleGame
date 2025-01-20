using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class LevelSelectScreen : MarginContainer
{
    [Export] private PackedScene LevelSelectSectionScene;

    private GridContainer gridContainer;

    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("%GridContainer");

        var levelDefinitons = LevelManager.GetLevelDefinitions();
        for (int i = 0; i < levelDefinitons.Length; i++)
        {
            var levelDefinition = levelDefinitons[i];
            var levelSelectSection = LevelSelectSectionScene.Instantiate<LevelSelectSection>();
            gridContainer.AddChild(levelSelectSection);

            levelSelectSection.SetLevelDefinition(levelDefinition);
            levelSelectSection.SetLevelIndex(i);
            levelSelectSection.LevelSelected += OnLevelSelected;
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        LevelManager.Instance.ChangeToLevel(levelIndex);
    }
}