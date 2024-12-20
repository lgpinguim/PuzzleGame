using Game.Manager;
using Game.Resources.Building;
using Godot;

namespace Game.UI;

public partial class GameUI : CanvasLayer
{
    [Signal]
    public delegate void BuildingResourceSelectedEventHandler(BuildingResource buildingResource);

    private VBoxContainer buildingSectionContainer;
    private Label resourceLabel;

    [Export] private BuildingManager buildingManager { get; set; }
    [Export] private BuildingResource[] buildingResources;
    [Export] private PackedScene buildingSectionScene;
    
    public override void _Ready()
    {
        buildingSectionContainer = GetNode<VBoxContainer>("%BuildingSectionContainer");
        resourceLabel = GetNode<Label>("%ResourceLabel");
        CreateBuildingSections();

        buildingManager.AvailableResourceCountChanged += OnAvailableResourceCountChanged;
    }

    private void CreateBuildingSections()
    {
        foreach (var buildingResource in buildingResources)
        {
            var buildingButton = buildingSectionScene.Instantiate<BuildingSection>();
            buildingSectionContainer.AddChild(buildingButton);
            buildingButton.SetBuildingResource(buildingResource);

            buildingButton.SelectButtonPressed += () =>
            {
                EmitSignal(SignalName.BuildingResourceSelected, buildingResource);
            };
        }
    }

    private void OnAvailableResourceCountChanged(int availableResourceCount)
    {
        resourceLabel.Text = availableResourceCount.ToString();
    }
}