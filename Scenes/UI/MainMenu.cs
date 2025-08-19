using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class MainMenu : Node
{
	private Button playButton;
	private Control mainMenuContainer;
	private LevelSelectScreen levelSelectScreen;
	private Button quitButton;
	
	public override void _Ready()
	{
		playButton = GetNode<Button>("%PlayButton");
		quitButton = GetNode<Button>("%QuitButton");
		
		AudioHelpers.RegisterButtons([playButton, quitButton]);
		
		
		mainMenuContainer = GetNode<Control>("%MainMenuContainer");
		levelSelectScreen = GetNode<LevelSelectScreen>("%LevelSelectScreen");
		
		levelSelectScreen.Visible = false;
		mainMenuContainer.Visible = true;

		playButton.Pressed += OnPlayButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;
		levelSelectScreen.BackPressed += OnLevelSelectBackPressed;
	}

	private void OnPlayButtonPressed()
	{
		levelSelectScreen.Visible = true;
		mainMenuContainer.Visible = false;
	}

	private void OnLevelSelectBackPressed()
	{
		levelSelectScreen.Visible = false;
		mainMenuContainer.Visible = true;
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
