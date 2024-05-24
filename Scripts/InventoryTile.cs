using Godot;
using System;

[Tool]
public partial class InventoryTile : TextureRect
{
	public Item Item { set; get; }

	private InventoryTile(){}
    public InventoryTile( Texture2D borderTexture )
	{
		Texture = borderTexture;
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//Texture = TileTexture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector2 GetMiddle()
	{
		return Position + Size / 2f;
	}

	public bool HasItem()
	{
		return Item != null;
	}
}
