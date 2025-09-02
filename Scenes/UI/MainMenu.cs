using Game.Autoload;
using Godot;

namespace Game.UI;

public partial class MainMenu : Node
{
	[Export]
	private PackedScene optionsMenuScene;
	
	private Button playButton;
	private Control mainMenuContainer;
	private LevelSelectScreen levelSelectScreen;
	private Button optionsButton;
	private Button quitButton;
	
	public override void _Ready()
	{
		playButton = GetNode<Button>("%PlayButton");
		optionsButton = GetNode<Button>("%OptionsButton");
		quitButton = GetNode<Button>("%QuitButton");
		
		AudioHelpers.RegisterButtons([playButton, optionsButton, quitButton]);
		
		
		mainMenuContainer = GetNode<Control>("%MainMenuContainer");
		levelSelectScreen = GetNode<LevelSelectScreen>("%LevelSelectScreen");
		
		levelSelectScreen.Visible = false;
		mainMenuContainer.Visible = true;

		playButton.Pressed += OnPlayButtonPressed;
		optionsButton.Pressed += OnOptionsButtonPressed;
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

	private void OnOptionsButtonPressed()
	{
		mainMenuContainer.Visible = false;
		var optionsMenu = optionsMenuScene.Instantiate<OptionsMenu>();
		AddChild(optionsMenu);
		optionsMenu.DonePressed += () =>
		{
			OnOptionsDonePressed(optionsMenu);
		};
	}

	private void OnOptionsDonePressed(OptionsMenu optionsMenu)
	{
		optionsMenu.QueueFree();
		mainMenuContainer.Visible = true;
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
