using Godot;
using System;

[Tool]
public partial class Item : TextureRect
{
	[Export]
	Vector2I TileSize { get; set; } = Vector2I.One;

   
    private Item(){}
    public Item( Texture2D borderTexture ) 
	{
		Texture = borderTexture;
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//Texture = ItemBorder;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _GuiInput( InputEvent @event )
    {
        if( @event is InputEventMouseButton click ) 
		{
			if( click.ButtonIndex == MouseButton.Left && click.IsReleased() )
			{
				// Inventory.updateItem
			}
		}
        if( @event is InputEventMouseMotion motion )
		{
			//Position.
		}
    }

    public Vector2 GetMiddle()
    {
        return Position + (Size / 2f);
    }

	public Vector2 GetMiddleGlobal() 
	{
        return GlobalPosition + (Size / 2f);
    }
}
