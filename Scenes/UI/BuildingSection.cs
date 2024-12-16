using Game.Resources.Building;
using Godot;

namespace Game.UI;

public partial class BuildingSection : PanelContainer
{
    [Signal]
    public delegate void SelectButtonPressedEventHandler();
    
    private Label titleLabel;
    private Button selectButton;

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("%Label");
        selectButton = GetNode<Button>("%Button");
        
        selectButton.Pressed += OnselectButtonPressed;
    }

    public void SetBuildingResource(BuildingResource buildingResource)
    {
        titleLabel.Text = buildingResource.DisplayName;
        selectButton.Text = $"Select (Cost {buildingResource.ResourceCost})";
    }

    private void OnselectButtonPressed()
    {
        EmitSignal(SignalName.SelectButtonPressed);
    }
}
