using Godot;

namespace Game.Autoload;

public partial class Cursor : CanvasLayer
{
    
    private Sprite2D CursorSprite2D;
    
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        CursorSprite2D = GetNode<Sprite2D>("CursorSprite2D");
    }
    
    public override void _Process(double delta)
    {
        CursorSprite2D.GlobalPosition = CursorSprite2D.GetGlobalMousePosition();
    }
}
