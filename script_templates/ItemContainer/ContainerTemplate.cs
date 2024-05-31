// meta-name: Default
// meta-description: A default template for a item container
// meta-default: true

using Godot;
using System;

public partial class _CLASS_ : ItemContainer
{
    public _CLASS_()
    : base()
    {
        // Define your own item size
        ItemSize = new Vector2I( 1, 1 );
    }

    protected override void InitInternal()
    {
        base.InitInternal();

        // Set inventory size
        Inventory = new Inventory( new Vector2I( 1, 1 ) );

        Inventory.Visible = false;
    }

    public override void SetTextures()
    {
        // Define your own textures
        Background.Texture = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );
        Border.Texture = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );
        Icon.Texture = (Texture2D)GD.Load( "" );
    }
}