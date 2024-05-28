using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

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

    /// <summary>
    /// The tile of the item that we are currently holding with the mouse
    /// </summary>
    //InventoryTile HeldTile { get; set; } = null;

    [Export]
    private bool ManualUpdateGrid { get; set; } = true;

    [ExportGroup( "Nodes" )]
    [Export]
    GridContainer Grid { get; set; }

    [ExportGroup( "Nodes" )]
    [Export]
    TextureRect Background { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Debug.Assert( Style != null, "Inventory: Style is null" );

		Background.Texture = ((InventoryStyle)Style).Background;

        ResizeGrid();
        RepositionGrid();

        if( !Engine.IsEditorHint() )
			CallDeferred( "DeferredReady" );
    }

	/// <summary>
	/// Called from _Ready() with CallDeferred() <br/>
	/// Mostly used for adding items which needs the inventory tilesToOccupy positions
	/// </summary>
	public void DeferredReady()
	{
		InventoryTile tile = GetTile( 0, 0 );
		if( tile == null ) {
			GD.PrintErr( "Tile null" );
		}

        TryAddItem<PipeWrench>( new Vector2I(0, 0) );
        //TryAddItem<Item>( new Array<InventoryTile> { GetTile( 1, 0 ) } );
		//TryAddItem<Item>( new Array<InventoryTile> { GetTile( 2, 0 ) } );
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if( Engine.IsEditorHint() )
		{
			if( Background != null )
            Background.Texture = ((InventoryStyle)Style).Background;

            Array<Node> tiles = Grid.GetChildren();
            if( tiles.Count != GridSize.X * GridSize.Y || ((InventoryTile)tiles[0]).Size.X != ((InventoryStyle)Style).TileSize || ManualUpdateGrid )
			{
				ResizeGrid();
				RepositionGrid();
				ManualUpdateGrid = false;
			}
        }
	}

    /// <summary>
	/// Returns the tile at the indexes
	/// </summary>
    public InventoryTile GetTile( int x, int y )
	{
		if( x < 0 || y < 0 || x >= GridSize.X || y >= GridSize.Y )
			return null;

		return (InventoryTile)Grid.GetChild( x + GridSize.X * y );
	}

    /// <summary>
    /// Returns the closest tile to the position <br/>
    /// Returns null if outside the grid
    /// </summary>
	/// <param name="pos"> Position in the CanvasLayer </param>
    public InventoryTile GetClosestTile( Vector2 pos )
	{
        if( !IsInside( pos ) )
            return null;

        Vector2 topLeft = GetTile( 0, 0 ).GlobalPosition;
		Vector2 bottomRight = GetTile( GridSize.X - 1, GridSize.Y - 1 ).GlobalPosition - topLeft + (Vector2.One * ((InventoryStyle)Style).TileSize);
		Vector2 normalizedPos = pos - topLeft;

		int xIndex = (int)Math.Floor( normalizedPos.X / bottomRight.X * GridSize.X );
		int yIndex = (int)Math.Floor( normalizedPos.Y / bottomRight.Y * GridSize.Y );

		return GetTile( xIndex, yIndex );
    }

	private void ResizeGrid()
	{
		GD.Print( "Resize grid ", ((InventoryStyle)Style).TileSize );
		
		Array<Node> tiles = Grid.GetChildren();

        // Remove all tiles
		foreach( Node tile in tiles )
		{
			tile.QueueFree();
			Grid.RemoveChild( tile );
		}
		
        // Add new tiles
        for( int x = 0; x < GridSize.X; x++ )
		{
			for( int y = 0; y < GridSize.Y; y++ )
            {
				InventoryTile tile = new InventoryTile( (InventoryStyle)Style );
				tile.Index = new Vector2I( x, y );
				Grid.AddChild( tile );
                   tile.Name = x.ToString() + y.ToString();
                   tile.Owner = this;
            }
		}

		// Error if Columns < 1
        Grid.Columns = Math.Max(GridSize.X, 1);
		Grid.Size = Vector2.Zero;

		InventoryStyle style = (InventoryStyle)Style;

		// Set padding between tilesToOccupy
        Grid.AddThemeConstantOverride( "h_separation", 0 );
        Grid.AddThemeConstantOverride( "v_separation", 0 );
	   
		
        Vector2 NewSize = new Vector2(
			style.TileSize * GridSize.X + style.GridPadding * 2,
			style.TileSize * GridSize.Y + style.GridPadding * 2 );

        Size = NewSize;
    }

	public void RepositionGrid()
	{
		Grid.Position = Vector2.One * ((InventoryStyle)Style).GridPadding;
    }

	public bool IsInside( Vector2 pos )
	{
		GD.Print( pos, GlobalPosition );

		if( pos > GlobalPosition && pos < (GlobalPosition + Size) )
		{
			GD.Print( "Inside" );
            return true;
		}
		GD.Print( "Outside" );
		return false;
    }

    /// <summary>
    /// Places an item in the inventory, TestItemPlacement() Should be called before this
    /// </summary>
    /// <param name="item"> The item to place </param>
    /// <param name="moveLocal"> If the item already exists in this inventory and should only be moved locally </param>
    /// <returns></returns>
    public bool PlaceItem( Item item )
	{
		Array<Vector2> positions = item.GetMiddlePositions();

		Array<InventoryTile> tilesToOccupy = new Array<InventoryTile>();

        // Get the tiles that the items should occupy
        foreach (var position in positions)
        {
			InventoryTile tile = GetClosestTile( position );
			tilesToOccupy.Add( tile );
        }

        foreach (var tile in tilesToOccupy)
            tile.Item = item;

        tilesToOccupy[0].AddChild( item );
		item.Owner = this;

        item.GlobalPosition = tilesToOccupy[0].GlobalPosition;

		return true;
    }

	public bool TestItemPlacement( Item item )
	{
        Array<Vector2> positions = item.GetMiddlePositions();

        // Check the tiles that the items should occupy
        foreach( var position in positions )
        {
            InventoryTile tile = GetClosestTile( position );
            if( tile == null || (tile.HasItem() && tile.Item != item) )
			{
				GD.Print( "inv" );
                return false;
			}
        }
		return true;
    }

	public void RemoveItem( Item item )
	{
		Array<Node> tiles = Grid.GetChildren();
        foreach (InventoryTile tile in tiles )
		{
			if( tile.Item == item )
			{
				if( tile.IsAncestorOf( item ) )
					tile.RemoveChild( item );
				tile.Item = null;
			}

		}
    }

	
	public void ReturnItem( Item item )
	{
        Array<Node> tiles = Grid.GetChildren();
		Array<InventoryTile> tilesContainingItem = new Array<InventoryTile> { };
        foreach( InventoryTile tile in tiles )
            if( tile.Item == item )
                tilesContainingItem.Add( tile );

		InventoryTile topLeft = null;
		foreach( InventoryTile tile in tilesContainingItem )
			if( topLeft == null || tile.GlobalPosition < topLeft.GlobalPosition )
				topLeft = tile;

		item.GlobalPosition = topLeft.GlobalPosition;
    } 

	/// <summary>
	/// Adds a new item to the inventory
	/// </summary>
	/// <typeparam name="ItemType"> The type of item to add </typeparam>
	/// <param name="tileIndex"> The index of the top left tile that the item should be placed at </param>
	public bool TryAddItem<ItemType>( Vector2I tileIndex ) where ItemType : Item
	{
        Item item = (ItemType)Activator.CreateInstance( typeof( ItemType ) );
		item.SetTileSize( item.ItemSize * ((InventoryStyle)Style).TileSize );

		Array<InventoryTile> tilesToOccupy = new Array<InventoryTile>();

        for (int x = 0; x < item.ItemSize.X; x++ )
		{
			for (int y = 0; y < item.ItemSize.Y; y++ )
			{
				InventoryTile tile = GetTile( tileIndex.X + x, tileIndex.Y + y );
				if( !tile.HasItem() )
					tilesToOccupy.Add( tile );
				else
				{
					GD.Print( Name, ": Failed to add item, tile already occupied" );
					return false;
				}
			}
		}

        foreach (var tile in tilesToOccupy)
			tile.Item = item;

        tilesToOccupy[0].AddChild( item );
        item.Owner = this;

        item.GlobalPosition = tilesToOccupy[0].GlobalPosition;

        GD.Print( Name, ": Added item" );

        return true;
    }
}