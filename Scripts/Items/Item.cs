using Godot;
using System;
using Godot.Collections;

[GlobalClass]
[Tool]
public partial class Item : Control
{
	public Vector2I ItemSize { get; set; } = new Vector2I( 1, 1 );

	[Export] protected TextureRect Icon { get; set; }
	[Export] protected TextureRect Background { get; set; }
	[Export] protected TextureRect Border { get; set; }
   
	public Item() 
	{
        InitInternal();
		SetTextures();
        MouseFilter = MouseFilterEnum.Ignore;
		ZAsRelative = true;
    }
	~Item()
	{
		// free texture
	}
	private void InitInternal()
	{
		Icon = new TextureRect();
        Background = new TextureRect();
        Border = new TextureRect();

        AddChild( Icon );
        AddChild( Background );
        AddChild( Border );

		Icon.Owner = this;
        Background.Owner = this;
        Border.Owner = this;

		Icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        Background.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        Border.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
    }

	public virtual void SetTextures()
	{
		GD.PrintErr( Name, ": No SetTextures override defined, loading default" );
        Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" ); ;
        Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" ); ;
        Icon.Texture = (Texture2D)GD.Load( "res://icon.svg" ); ;
    }

	public void SetTileSize( Vector2 size )
	{
		SetSize( size );
        Background.SetSize( size );
        Border.SetSize( size );
        Icon.SetSize( size );
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    /// <summary>
    /// Gets the position of the top left tile of the tile that this item would occupy 
    /// </summary>
    /// <returns></returns>
    public Vector2 GetTopLeftMiddlePos()
	{
		return GlobalPosition + (Size / ItemSize / 2);
	}

	/// <summary>
	/// Gets the middle position of all the tiles that this item would occupy
	/// </summary>
	/// <returns></returns>
	public Array<Vector2> GetMiddlePositions()
	{
		Array<Vector2> positions = new Array<Vector2>();

		Vector2 topLeft = GetTopLeftMiddlePos();

        for (int y = 0; y < ItemSize.Y; y++)
			for (int x = 0; x < ItemSize.X; x++)
				positions.Add( topLeft + (Size / ItemSize * new Vector2( x, y )) );

		return positions;
    }
}
