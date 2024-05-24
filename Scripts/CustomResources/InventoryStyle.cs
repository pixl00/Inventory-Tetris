using Godot;
using System;

[GlobalClass]
[Tool]
public partial class InventoryStyle : Resource
{
    public InventoryStyle(){}

    [Export]
    public Texture2D TileTexture { get; private set; } = (Texture2D)GD.Load( "res://Resources/KenneyAssets/InventoryTile.png" );

    [Export]
    public Texture2D Background { get; private set; } = (Texture2D)GD.Load( "res://Resources/DarkBlueGradiant.tres" );

    [Export]
    public Texture2D ItemBorder { get; private set; } = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );

    [Export]
    public Texture2D ItemBackground { get; private set; } = (Texture2D)GD.Load( "res://Resources/BlackTranslucent.png" );

    // Add later maybe
    //[Export] 
    //public int TileSize { get; private set; } = 30;

    [Export]
    public int TilePadding { get; private set; }

    [Export]
    public int GridPadding { get; private set; }
}
