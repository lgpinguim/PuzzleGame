using Game.Resources.Building;
using Godot;

namespace Game.UI;

public partial class BuildingSection : PanelContainer
{
    [Signal]
    public delegate void SelectButtonPressedEventHandler();
    
    private Label titleLabel;
    private Label descriptionLabel;
    private Label costLabel;
    private Button selectButton;

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("%TitleLabel");
        descriptionLabel = GetNode<Label>("%DescriptionLabel");
        costLabel = GetNode<Label>("%CostLabel");
        selectButton = GetNode<Button>("%Button");
        
        selectButton.Pressed += OnselectButtonPressed;
    }

    public void SetBuildingResource(BuildingResource buildingResource)
    {
        titleLabel.Text = buildingResource.DisplayName;
        descriptionLabel.Text = buildingResource.Description;
        costLabel.Text = $"{buildingResource.ResourceCost}";
    }

    private void OnselectButtonPressed()
    {
        EmitSignal(SignalName.SelectButtonPressed);
    }
}
