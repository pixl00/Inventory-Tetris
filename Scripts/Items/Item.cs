using Godot;
using System;
using Godot.Collections;

[Tool]
public partial class Item : Control
{
	public Godot.Collections.Array<InventoryTile> Tiles { get; set; }

	[Export]TextureRect Icon { get; set; }
	[Export]TextureRect Background { get; set; }
	[Export]TextureRect Border { get; set; }
   
    protected Item(){}
    public Item( InventoryStyle style, Array<InventoryTile> tiles ) 
	{
		Background.Texture = style.ItemBackground;
		Border.Texture = style.ItemBorder;

        Tiles = tiles;

		foreach( InventoryTile tile in Tiles )
			tile.Item = this;

		Reposition();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		
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

	public void Reposition()
	{
		Vector2 topLeft = new Vector2( int.MaxValue, int.MaxValue );

        foreach (var tile in Tiles)
        {
            if( tile.Position.X < topLeft.X )
				topLeft.X = tile.Position.X;
            if( tile.Position.Y < topLeft.Y )
                topLeft.Y = tile.Position.Y;
        }
		Position = topLeft;
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
