// meta-name: Stackable
// meta-description: A template for a stackable item
// meta-default: true

using Godot;
using System;

public partial class _CLASS_ : StackableItem
{
    public _CLASS_()
    : base()
    {
        // Define your own item size
        ItemSize = new Vector2I( 1, 1 );
        // Define max item amount
        MaxItemAmount = 1;
    }

    public override void SetTextures()
    {
        // Define your own textures
        Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
        Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
        Icon.Texture = (Texture2D)GD.Load( "" );
    }
}