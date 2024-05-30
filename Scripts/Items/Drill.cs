using Godot;
using System;

public partial class Drill : Item
{
	public Drill()
	: base()
	{
		// Define your own item size
		ItemSize = new Vector2I( 3, 2 );
	}

	public override void SetTextures()
	{
		// Define your own textures
		Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
		Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
		Icon.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/Drill.png" );
	}
}
