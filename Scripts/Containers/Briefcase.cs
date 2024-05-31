using Godot;
using System;

public partial class Briefcase : ItemContainer
{
	public Briefcase()
	: base()
	{
		// Define your own item size
		ItemSize = new Vector2I( 2, 2 );
	}

	protected override void InitInternal()
	{
		base.InitInternal();

		// Set inventory size
		Inventory = new Inventory( new Vector2I( 4, 4 ) );

		Inventory.Visible = false;
	}

	public override void SetTextures()
	{
		// Define your own textures
		Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
		Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
		Icon.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/Briefcase.png" );
	}
}
