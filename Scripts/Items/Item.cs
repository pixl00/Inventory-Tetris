using Godot;
using System;
using Godot.Collections;
using System.Drawing;

[GlobalClass]
[Tool]
public partial class Item : Control
{
	public Vector2I ItemSize { get; set; } = new Vector2I( 1, 1 );

	protected TextureRect Icon { get; set; }
	protected TextureRect Background { get; set; }
	protected TextureRect Border { get; set; }

	public InventoryStyle InventoryStyle { get; set; }

	public bool Rotated { get; set; } = false;
   
	public Item() 
	{
        InitInternal();
		SetTextures();
        MouseFilter = MouseFilterEnum.Ignore;
		ZAsRelative = true;
    }
	~Item()
	{
		if( Icon != null && Icon.Texture != null )
            Icon.Texture.Free();
        if( Background != null && Background.Texture != null )
            Background.Texture.Free();
        if( Border != null && Border.Texture != null )
            Border.Texture.Free();

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

	public void SetTileSize( Vector2I size )
	{
		if( InventoryStyle == null )
			GD.PrintErr( "Item: Inventory style has to be set before calling SetTileSize()" );

		SetSize( size * InventoryStyle.TileSize );
        Background.SetSize( size * InventoryStyle.TileSize );
        Border.SetSize( size * InventoryStyle.TileSize );
        Icon.SetSize( size * InventoryStyle.TileSize );
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Rotate( Vector2 point )
	{
        Rotated = !Rotated;

        ItemSize = new Vector2I( ItemSize.Y, ItemSize.X );

        SetSize( ItemSize * InventoryStyle.TileSize );
        //Background.SetSize( ItemSize * InventoryStyle.TileSize );
        //Border.SetSize( ItemSize * InventoryStyle.TileSize );
		//Icon.SetSize(ItemSize * InventoryStyle.TileSize );

		point = GetClosestMiddlePosition( point );

		float rotation = Rotated ? -90 : 0;

		Icon.RotationDegrees = rotation;
        Background.RotationDegrees = rotation;
        Border.RotationDegrees = rotation;

		float yPos = Rotated ? Size.Y : 0;

        Icon.Position = new Vector2( 0, yPos );
        Background.Position = new Vector2( 0, yPos );
        Border.Position = new Vector2( 0, yPos );

        //PivotOffset = point - GlobalPosition;
        GD.Print( point - GlobalPosition );
		GD.Print( point );
		GD.Print( GlobalPosition );
	
		
		//RotationDegrees = Rotated ? -90 : 0;

		GD.Print( "Get rotated pleb" );
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

        for (int y = 0; y < ItemSize.Y; y++ )
		{
			for (int x = 0; x < ItemSize.X; x++ )
			{
				positions.Add( topLeft + (Size / ItemSize * new Vector2( x, y )) );
			}
		}

		return positions;
    }

	public Vector2 GetClosestMiddlePosition( Vector2 pos )
	{
		Array<Vector2> positions = GetMiddlePositions();

		Vector2 closestMiddlePos = positions[0];

        for (int i = 1; i < positions.Count; i++)
        {
            if( (pos - positions[i]).LengthSquared() < (pos - closestMiddlePos).LengthSquared() ) 
			{
				closestMiddlePos = positions[i];
			}
        }
		return closestMiddlePos;
    }
}
