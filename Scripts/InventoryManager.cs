using Godot;
using System;
using Godot.Collections;

public partial class InventoryManager : Control
{
	[Export]
	public Array<Inventory> Inventories { get; set; }

	public Item HeldItem { get; set; }

	/// <summary>
	/// The inventory that the HeldItem is from
	/// </summary>
	private Inventory HeldItemInventory { get; set; }
    /// <summary>
    /// What rotation the item had when it was picked up
    /// </summary>
    private bool HeldItemRotation { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        foreach (var inventory in Inventories)
            inventory.ClearDisplayItemPlacement();

        if ( HeldItem != null )
		{
			Array<Vector2> positions = HeldItem.GetMiddlePositions();

            foreach (var pos in positions)
            {
                Inventory inv = GetHoveredInventory( pos );
                if( inv != null )
                {
                    inv.DisplayItemPlacement( HeldItem );
                }
            }
		}
    }

    public override void _GuiInput( InputEvent @event )
    {
		if( @event is InputEventMouseButton click )
		{
            if( click.ButtonIndex == MouseButton.Left && click.IsPressed() )
			{
				TryGrabItem( click.GlobalPosition );
			}
            if( click.ButtonIndex == MouseButton.Left && click.IsReleased() )
            {
				TryDropHeldItem();
            }
            if( click.ButtonIndex == MouseButton.Right && click.IsPressed() )
            {
                //TestTile( click.GlobalPosition );
                Inventory inv = GetHoveredInventory( click.GlobalPosition );
                GD.Print( inv.CheckTile( inv.GetClosestTile( click.GlobalPosition ) ), HeldItem );
            }
        }

		if( @event is InputEventMouseMotion motion )
		{
			if( HeldItem != null )
			{
				HeldItem.Position += motion.Relative;
			}
		}
    }

    public override void _Input( InputEvent @event )
    {
        if( @event.IsActionPressed( "Rotate_Item" ) && HeldItem != null )
        {
            HeldItem.Rotate();
        }
    }

    public override void _Draw()
    {
    }

    /// <summary>
    /// Gets the currently hovered inventory
    /// </summary>
    /// <param name="pos"> Position in CanvasLayer </param>
    /// <returns></returns>
    private Inventory GetHoveredInventory( Vector2 pos )
	{
		Inventory hoveredInventory = null;

        foreach (var inventory in Inventories)
        {
			if( pos < inventory.GlobalPosition || pos > ( inventory.GlobalPosition + inventory.Size) )
				continue;

			if( hoveredInventory == null || inventory.ZIndex > hoveredInventory.ZIndex )
			{
				hoveredInventory = inventory;
				continue;
			}
        }

		return hoveredInventory;
    }

    /// <summary>
    /// Tries to grab the item at the position
    /// </summary>
    /// <param name="pos"> Position in CanvasLayer </param>
    private void TryGrabItem( Vector2 pos )
	{
        Inventory hoveredInventory = GetHoveredInventory( pos );
        if( hoveredInventory == null )
			return;

        InventoryTile tile = hoveredInventory.GetClosestTile( pos );
		if( tile == null || tile.Item == null ) 
			return;

		HeldItem = tile.Item;
		HeldItemInventory = hoveredInventory;
        HeldItemRotation = tile.Item.Rotated;
		HeldItem.ZIndex = (int)RenderingServer.CanvasItemZMax;
    }

	private void TestTile( Vector2 pos )
	{
        Inventory hoveredInventory = GetHoveredInventory( pos );
        if( hoveredInventory == null )
		{
            GD.Print( "Inventory: No inventory\n" );
			return;
		}
        GD.Print( "Inventory: ", hoveredInventory.Name );

        InventoryTile tile = hoveredInventory.GetClosestTile( pos );
        if( tile == null )
		{
            GD.Print( "Tile: No tile\n" );
            return;
        }
        GD.Print( "Tile: ", tile.Name );

        if( tile.Item == null )
		{
            GD.Print( "Item: No item\n" );
            return;
        }
		GD.Print( "Item: ", tile.Item.GetType(), "\n" );
    }

    /// <summary>
    /// Tries to drop the he item
    /// </summary>
    /// <param name="pos"> Position in CanvasLayer </param>
    private void TryDropHeldItem()
	{
		if( HeldItem == null ) 
			return;

        Array<Vector2> positions = HeldItem.GetMiddlePositions();
		// Inventory at items top left position
        Inventory hoveredInventory = GetHoveredInventory( positions[0] );
		// Inventory at items bottom right position
        Inventory hoveredInventory2 = GetHoveredInventory( positions[positions.Count - 1] );

        if( hoveredInventory == null || hoveredInventory2 == null || (hoveredInventory != hoveredInventory2) )
        {
            if( HeldItemRotation != HeldItem.Rotated )
                HeldItem.Rotate();
            HeldItemInventory.ReturnItem( HeldItem );
            GD.Print( "Held item returned to old inventory" );
            HeldItem = null;
            HeldItemInventory = null;
            return;
        }

        bool canPlace = hoveredInventory.TestItemPlacement( HeldItem );
        
        InventoryTile hoveredTile = hoveredInventory.GetClosestTile( HeldItem.GetTopLeftMiddlePos() );

        bool stack = HeldItem is StackableItem && hoveredTile.Item is StackableItem && HeldItem != hoveredTile.Item;

        if( !canPlace )
		{
            if( HeldItemRotation != HeldItem.Rotated ) 
                HeldItem.Rotate();
			HeldItemInventory.ReturnItem( HeldItem );
			GD.Print( "Held item returned to old inventory" );
		}
        else if( canPlace && stack )
        {
            int remainder = hoveredInventory.StackItem( (StackableItem)HeldItem );
            if( remainder > 0 )
            {
                GD.Print( remainder );
                ((StackableItem)HeldItem).SetAmount( remainder );
                HeldItemInventory.ReturnItem( HeldItem );
            }
            else
            {
                GD.Print( remainder );
                HeldItemInventory.RemoveItem( HeldItem );
                HeldItem.QueueFree();
            }

        }
        else if( canPlace )
        {
            HeldItemInventory.RemoveItem( HeldItem );
            hoveredInventory.PlaceItem( HeldItem );
            GD.Print( "Held item moved" );
        }
		
		HeldItem = null;
		HeldItemInventory = null;
        GD.Print( "" );
    }

}
