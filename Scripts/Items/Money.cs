using Godot;
using System;

public partial class Money : StackableItem
{
	public Money()
	{
		// Define your own item size
		ItemSize = new Vector2I( 1, 1 );
        // Define max item amount
        MaxItemAmount = 200_000;
    }

	public override void SetTextures()
	{
		// Define your own textures
		Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
		Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
		Icon.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/Money.png" );
	}
}
