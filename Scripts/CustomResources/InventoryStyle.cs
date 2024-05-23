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
    public Texture2D ItemBorder { get; private set; } = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );

    [Export]
    public int TilePadding { get; set; }

    [Export]
    public int GridPadding { get; set; }
}
