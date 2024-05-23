using Godot;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

[Tool]
public partial class Inventory : Control
{

	[Export]
	Resource Style { get; set; } = GD.Load( "res://Resources/CustomResources/DefaultInventoryStyle.tres" );

	private Vector2I _gridSize;
    [Export]
	Vector2I GridSize {
		get { return _gridSize; } 
		set { _gridSize = value.Clamp( Vector2I.One, Vector2I.MaxValue ); }
    }

    [Export]
	InventoryTile HeldTile { get; set; } = null;

    [ExportGroup( "Nodes" )]
    [Export]
    GridContainer Grid { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Debug.Assert( Style != null, "Inventory: Style is null" );

        ResizeGrid();
        RepositionGrid();

		AddItem( 0, 0 );
		AddItem( 1, 0 );
		AddItem( 3, 0 );
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if( Engine.IsEditorHint() )
		{
            ResizeGrid();
			RepositionGrid();
        }
	}


    public override void _GuiInput( InputEvent @event )
    {
		if( @event is InputEventMouseButton click )
		{
			if( click.ButtonIndex == MouseButton.Left && click.IsPressed() )
			{
				InventoryTile tile = GetClosestTile( click.Position );
				if( tile.ItemSlot != null )
					HeldTile = tile;
			}
			if( click.ButtonIndex == MouseButton.Left && click.IsReleased() )
			{
				DropHeldItem();
				ClearTilesHoveredColor();
			}
			if( click.ButtonIndex == MouseButton.Right && click.IsPressed() )
			{
				AddItem( GetClosestTile( click.Position ) );
			}
        }
        if( @event is InputEventMouseMotion motion )
        {
            if( HeldTile != null && HeldTile.ItemSlot != null )
			{
				HeldTile.ItemSlot.Position += motion.Relative;
				UpdateTileHoveredColor();
			}
        }
    }

    /// <summary>
	/// Returns the tile at the indexes
	/// </summary>
    public InventoryTile GetTile( int x, int y )
	{
		Debug.Assert( x >= 0 && y >= 0 && x < GridSize.X && y < GridSize.Y, "Inventory::GetItem(): Item index was outside the range" );

		return (InventoryTile)Grid.GetChild( x + GridSize.X * y );
	}

    /// <summary>
    /// Returns the closest tile to the position <br/>
    /// Returns null if outside the grid
    /// </summary>
    public InventoryTile GetClosestTile( Vector2 position )
	{
		InventoryTile topLeftTile = GetTile( 0, 0 );
		Vector2 bottomRight = GetTile( GridSize.X - 1, GridSize.Y - 1 ).Position + topLeftTile.Size - topLeftTile.Position;
		Vector2 normalizedPos = position - topLeftTile.Position;
	
		// Outside the grid
        if( normalizedPos.X < 0f || normalizedPos.X >= bottomRight.X ||
			normalizedPos.Y < 0f || normalizedPos.Y >= bottomRight.Y )
		{
			return null;
		}

		int xIndex = (int)Math.Floor( normalizedPos.X / bottomRight.X * GridSize.X );
		int yIndex = (int)Math.Floor( normalizedPos.Y / bottomRight.Y * GridSize.Y );

		return GetTile( xIndex, yIndex );
    }

	private void ResizeGrid()
	{
        Godot.Collections.Array<Node> tiles = Grid.GetChildren( true );
		if( tiles.Count != GridSize.X * GridSize.Y  )
		{
			GD.Print( "Resize grid" );

            // Remove all tiles
            foreach( Node tile in tiles )
			{
				tile.QueueFree();
				Grid.RemoveChild( tile );
			}
			
            // Add new tiles
            for( uint x = 0; x < GridSize.X; x++ )
			{
                for( uint y = 0; y < GridSize.Y; y++ )
                {
					InventoryTile tile = new InventoryTile( ((InventoryStyle)Style).TileTexture );
					Grid.AddChild( tile );
                    tile.Name = x.ToString() + y.ToString();
                    tile.Owner = this;
                }
			}
		}
		// Error if Columns < 1
        Grid.Columns = Math.Max(GridSize.X, 1);
		Grid.Size = Vector2.Zero;

		InventoryStyle style = (InventoryStyle)Style;

		// Set padding between tiles
        Grid.AddThemeConstantOverride( "h_separation", style.TilePadding );
        Grid.AddThemeConstantOverride( "v_separation", style.TilePadding );
	   
	
        Vector2 NewSize = new Vector2(
          style.TileTexture.GetWidth() * GridSize.X + (GridSize.X - 1) * style.TilePadding + style.GridPadding * 2,
          style.TileTexture.GetHeight() * GridSize.Y + (GridSize.Y - 1) * style.TilePadding + style.GridPadding * 2 );

        Size = NewSize;
    }

	public void RepositionGrid()
	{
        Grid.Position = Vector2.One * ((InventoryStyle)Style).GridPadding;
    }

	private void DropHeldItem()
	{
		if( HeldTile == null )
			return;

		Vector2 itemPos = HeldTile.ItemSlot.GetMiddle();

		InventoryTile tile = GetClosestTile( itemPos );

        GD.Print( HeldTile.Position );
        GD.Print( tile.Position );

        if( tile == null || tile.HasItem() || tile == HeldTile )
		{
			HeldTile.ItemSlot.Position = HeldTile.Position;
            HeldTile = null;
            return;
		}

		HeldTile.ItemSlot.Position = tile.Position;
		tile.ItemSlot = HeldTile.ItemSlot;
		
		HeldTile.ItemSlot = null;
		HeldTile = null;
	}

	private void ClearTilesHoveredColor()
	{
        Godot.Collections.Array<Node> tiles = Grid.GetChildren( true );
        foreach( InventoryTile itemTile in tiles )
        {
            itemTile.SelfModulate = new Color( 1, 1, 1 );
        }
    }

    private void UpdateTileHoveredColor()
    {
        ClearTilesHoveredColor();

        InventoryTile tile = GetClosestTile( HeldTile.ItemSlot.GetMiddle() );

		if( tile == null )
			return;

		if( !tile.HasItem() || tile == HeldTile )
		{
			tile.SelfModulate = new Color( 0, 1, 0 );
        }
		else
		{
			tile.SelfModulate = new Color( 1, 0, 0 );
		}
    }

	public void AddItem( int x, int y )
	{
        InventoryTile tile = GetTile( x, y );

		if( tile == null || tile.HasItem() ) 
			return;

       AddItem( tile );
    }

	public void AddItem( InventoryTile tile )
	{
        tile.ItemSlot = new Item( ((InventoryStyle)Style).ItemBorder );

        tile.AddChild( tile.ItemSlot );
        tile.ItemSlot.Owner = tile;

        tile.ItemSlot.Position = Vector2.Zero;
    }
}