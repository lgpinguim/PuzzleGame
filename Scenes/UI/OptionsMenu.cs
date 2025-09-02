using Godot;
using System;
using System.Globalization;
using Game.Autoload;

namespace  Game.UI;

public partial class OptionsMenu : CanvasLayer
{
    [Signal]
    public delegate void DonePressedEventHandler();
    
    //SFX
    private const string SFX_BUS_NAME = "SFX";
    private Button sfxUpButton;
    private Button sfxDownButton;
    private Label sfxLabel;
    
    //Music
    private const string MUSIC_BUS_NAME = "Music";
    private Button musicUpButton;
    private Button musicDownButton;
    private Label musicLabel;

    //Window
    private Button windowButton;
    
    private Button doneButton;

    public override void _Ready()
    {
        sfxDownButton = GetNode<Button>("%SFXDownButton");
        sfxUpButton = GetNode<Button>("%SFXUpButton");
        sfxLabel = GetNode<Label>("%SFXLabel");
        
        musicUpButton = GetNode<Button>("%MusicUpButton");
        musicDownButton = GetNode<Button>("%MusicDownButton");
        musicLabel = GetNode<Label>("%MusicLabel");
        
        windowButton = GetNode<Button>("%WindowButton");
        
        doneButton = GetNode<Button>("%DoneButton");
        
        AudioHelpers.RegisterButtons([sfxUpButton, sfxDownButton, musicUpButton, windowButton, doneButton]);
        
        UpdateDisplay();

        sfxUpButton.Pressed += () =>
        {
            ChangeBusVolume(SFX_BUS_NAME, .1f);
        };
        
        sfxDownButton.Pressed += () =>
        {
            ChangeBusVolume(SFX_BUS_NAME, -.1f);
        };

        musicUpButton.Pressed += () =>
        {
            ChangeBusVolume(MUSIC_BUS_NAME, .1f);
        };

        musicDownButton.Pressed += () =>
        {
            ChangeBusVolume(MUSIC_BUS_NAME, -.1f);
        };

        windowButton.Pressed += OnWindowButtonPressed;

        doneButton.Pressed += OnDoneButtonPressed;
    }

    private void UpdateDisplay()
    {
        sfxLabel.Text = MathF.Round(OptionsHelper.GetBusVolumePercent(SFX_BUS_NAME) * 10).ToString();
        musicLabel.Text = MathF.Round(OptionsHelper.GetBusVolumePercent(MUSIC_BUS_NAME) * 10).ToString();
        windowButton.Text = OptionsHelper.IsFullScreenMode() ? "Fullscreen" : "Windowed";
    }

    private void ChangeBusVolume(string busName, float change)
    {
        var busVolumePercent = OptionsHelper.GetBusVolumePercent(busName);
        busVolumePercent = Mathf.Clamp(busVolumePercent + change, 0, 1);
        OptionsHelper.SetBusVolumePercent(busName, busVolumePercent);
        UpdateDisplay();
    }

    private void OnWindowButtonPressed()
    {
        OptionsHelper.ToggleWindowMode();
        UpdateDisplay();
    }

    private void OnDoneButtonPressed()
    {
        EmitSignal(SignalName.DonePressed);
    }

}
