using Godot;
using System;
using Godot.Collections;

public partial class InventoryManager : Control
{
	[Export]
	public Array<Inventory> Inventories { get; set; }

	public Item HeldItem { get; set; }

	/// <summary>
	/// The inventory that the HeldItem is in
	/// </summary>
	private Inventory HeldItemInventory {  get; set; }

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
            if( click.ButtonIndex == MouseButton.Left && click.IsPressed() )
			{
				TryGrabItem( click.GlobalPosition );
				GD.Print( "click" );
			}
            if( click.ButtonIndex == MouseButton.Left && click.IsReleased() )
            {
				TryDropHeldItem();
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
		if( hoveredInventory != null )
			GD.Print( hoveredInventory.Name );

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
		HeldItem.ZIndex = (int)RenderingServer.CanvasItemZMax;

		GD.Print( "Grabbed item" );
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
			HeldItemInventory.ReturnItem( HeldItem );
            HeldItem = null;
            HeldItemInventory = null;
            return;
		}

        bool canPlace = hoveredInventory.TestItemPlacement( HeldItem );

		if( canPlace )
		{
			HeldItemInventory.RemoveItem( HeldItem );
			hoveredInventory.PlaceItem( HeldItem );
			GD.Print( "Held item moved" );
		}
		else
		{
			HeldItemInventory.ReturnItem( HeldItem );
			GD.Print( "Held item returned to old inventory" );
		}
		
		HeldItem = null;
		HeldItemInventory = null;
    }

}
