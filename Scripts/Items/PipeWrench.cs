using Godot;
using System;

public partial class PipeWrench : Item
{
    public PipeWrench()
	: base()
	{
		// Define your own item size
		ItemSize = new Vector2I( 1, 2 );
	}

	public override void SetTextures()
	{
		// Define your own textures
		Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
		Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
		Icon.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/PipeWrench.png" );
	}
}
