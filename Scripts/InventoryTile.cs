using Godot;
using System;

[Tool]
public partial class InventoryTile : TextureRect
{
	public Item Item { set; get; }

	public Vector2I Index { set; get; }

	private InventoryTile(){}
    public InventoryTile( InventoryStyle style )
	{
		Texture = style.TileTexture;
        ExpandMode = ExpandModeEnum.IgnoreSize;
		CustomMinimumSize = new Vector2( style.TileSize, style.TileSize );
        SetSize( new Vector2( style.TileSize, style.TileSize ) );
		MouseFilter = MouseFilterEnum.Ignore;
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
		return Position + (Size / 2f);
	}

	public bool HasItem()
	{
		return Item != null;
	}
}
