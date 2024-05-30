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
        TryAddItem<Money>( new Vector2I(0, 2), 150_000 );
        TryAddItem<Money>( new Vector2I(3, 0), 10_000 );
        TryAddItem<Computer>( new Vector2I(1, 0) );
        TryAddItem<Drill>( new Vector2I(0, 3) );
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

    public override void _Draw()
    {
        base._Draw();
        Array<Node> tiles = Grid.GetChildren();

		foreach( InventoryTile tile in tiles )
		{
			if( tile.Item != null ) 
			{
				DrawCircle( tile.Item.GetTopLeftMiddlePos(), 40, new Color( 0, 1, 0 ) );
				GD.Print( tile.Name );
			}
		}
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
		if( pos > GlobalPosition && pos < (GlobalPosition + Size) )
		{
            return true;
		}
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
		if( item == null )
			return false;

		Array<Vector2> positions = item.GetMiddlePositions();

		Array<InventoryTile> tilesToOccupy = new Array<InventoryTile>();

		// Get the tiles that the items should occupy
		foreach( var position in positions )
		{
			InventoryTile tile = GetClosestTile( position );
			tilesToOccupy.Add( tile );
		}

		foreach( var tile in tilesToOccupy )
			tile.Item = item;

		tilesToOccupy[0].AddChild( item );
		item.Owner = this;
		item.GlobalPosition = tilesToOccupy[0].GlobalPosition;
		item.ZIndex = ZIndex + 2;
		return true;
    }

    public int StackItem( StackableItem item )
    {
        if( item == null )
            return 0;

        InventoryTile tile = GetClosestTile( item.GetTopLeftMiddlePos() );

		int remainder = ((StackableItem)tile.Item).AddAmount( item.ItemAmount );

		return remainder;
    }

    public bool TestItemPlacement( Item item )
	{
        Array<Vector2> positions = item.GetMiddlePositions();

        // Check the tiles that the item should occupy
        foreach( var position in positions )
        {
            InventoryTile tile = GetClosestTile( position );
            if( !CheckTile( tile, item ) )
			{
				GD.Print( "Item can not be placed here" );
                return false;
			}
        }
        GD.Print( "Item can be placed here" );
        return true;
    }

    /// <summary>
    /// Returns true if the an item can be placed in the tile,
    /// Input an item to include it as a valid place.
	/// Also returns true if the item and the tile.Item Stackable and the same type
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CheckTile( InventoryTile tile, Item item = null )
    {
        if( tile == null )
        {
            GD.Print( "Tile was null" );
            return false;
        }

        if( tile.HasItem() )
        {
			if( item != null ) 
			{
				if( (tile.Item == item) )
				{
                    GD.Print( "item is tile item" );
                    return true;
				}
				if( (item is StackableItem) )
				{
					if( tile.Item is StackableItem )
					{
						if( item.GetType() == tile.Item.GetType() )
						{
							return true;
						}
						else
						{
                            GD.Print( "item and tile are not the same type" );
                            return false;
                        }
					}
					else
					{
                        GD.Print( "tile is not stackable" );
						return false;
                    }
				}
				else
				{
					GD.Print( "item is not stackable" );
					return false;
				}
            }
			else
			{
                GD.Print( "item can be placed" );
                return false;
			}
        }
		else
		{
            GD.Print( "item can be placed" );
            return true;
		}
    }

    public void DisplayItemPlacement( Item item )
	{
        Array<Vector2> positions = item.GetMiddlePositions();

        // Check the tiles that the item should occupy
        foreach( var position in positions )
        {
            InventoryTile tile = GetClosestTile( position );
            if( tile == null )
				continue;

			if( CheckTile( tile, item ) )
			{
                Color green = new Color( 0, 1, 0, 0.5f );
				tile.Modulate = green;
				//tile.Item.Modulate = green;
                item.Modulate = green;
            }
			else
			{
				Color red = new Color( 1, 0, 0, 0.5f );
				tile.Modulate = red;
                tile.Item.Modulate = red;
                item.Modulate = red;
            }
        }
    }

    public void ClearDisplayItemPlacement()
	{
        Array<Node> tiles = Grid.GetChildren();

        foreach( InventoryTile tile in tiles )
		{
            tile.Modulate = new Color( 1, 1, 1 );
			if( tile.HasItem() )
				tile.Item.Modulate = new Color( 1, 1, 1 );
        }
    }

    public void RemoveItem( Item item )
	{
		Array<Node> tiles = Grid.GetChildren();
        foreach (InventoryTile tile in tiles )
		{
			if( tile.Item == item )
			{
				if( tile.GetChildCount() >= 1 && tile.GetChild( 0 ) == item )
				{
					// Removing a child apparently changes the position of the removed child
					Vector2 savePos = item.GlobalPosition;
					tile.RemoveChild( item );
					item.GlobalPosition = savePos;
				}
				
				tile.Item = null;
			}
		}
    }
	
	public void ReturnItem( Item item )
	{
        Array<Node> tiles = Grid.GetChildren();
		Array<InventoryTile> tilesContainingItem = new Array<InventoryTile>();
        foreach( InventoryTile tile in tiles )
            if( tile.Item == item )
                tilesContainingItem.Add( tile );

		InventoryTile topLeft = null;
		foreach( InventoryTile tile in tilesContainingItem )
			if( topLeft == null || tile.GlobalPosition < topLeft.GlobalPosition )
				topLeft = tile;

		item.GlobalPosition = topLeft.GlobalPosition;
		item.ZIndex = ZIndex + 2;
    } 

	/// <summary>
	/// Adds a new item to the inventory
	/// </summary>
	/// <typeparam name="ItemType"> The type of item to add </typeparam>
	/// <param name="tileIndex"> The index of the top left tile that the item should be placed at </param>
	public ItemType TryAddItem<ItemType>( Vector2I tileIndex ) where ItemType : Item
	{
        Item item = (ItemType)Activator.CreateInstance( typeof( ItemType ) );
		item.InventoryStyle = (InventoryStyle)Style;
		item.SetTileSize( item.ItemSize );
		item.ZIndex = ZIndex + 2;

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
					return null;
				}
			}
		}

        foreach (var tile in tilesToOccupy)
			tile.Item = item;

        tilesToOccupy[0].AddChild( item );
        item.Owner = this;

        item.GlobalPosition = tilesToOccupy[0].GlobalPosition;

        GD.Print( Name, ": Added item (", item.GetType(),")" );

        return (ItemType)item;
    }

	public ItemType TryAddItem<ItemType>( Vector2I tileIndex, int amount ) where ItemType : StackableItem
	{
		StackableItem item = TryAddItem<ItemType>( tileIndex );

		if( item != null )
			item.SetAmount( amount );

		return (ItemType)item;
	}
}