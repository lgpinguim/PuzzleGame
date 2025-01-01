using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class MainMenu : Node
{
	private Button _playButton;
	
	public override void _Ready()
	{
		_playButton = GetNode<Button>("%PlayButton");

		_playButton.Pressed += OnPlayButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		LevelManager.Instance.ChangeToLevel(0);
	}
	
}
