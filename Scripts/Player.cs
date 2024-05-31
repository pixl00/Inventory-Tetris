using Godot;
using System;

public partial class Player : Node3D
{
	[Export]
	InventoryManager InventoryManager { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if( InventoryManager != null )
		{
			Inventory mainInv = InventoryManager.Inventories[0];

			mainInv.TryAddItem<Briefcase>( new Vector2I( 0, 0 ) );
			mainInv.TryAddItem<Money>( new Vector2I( 2, 0 ), 100_000 );
			mainInv.TryAddItem<Money>( new Vector2I( 3, 0 ), 10_000 );
			mainInv.TryAddItem<Drill>( new Vector2I( 0, 2 ) );
			mainInv.TryAddItem<Computer>( new Vector2I( 0, 4 ) );

		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
