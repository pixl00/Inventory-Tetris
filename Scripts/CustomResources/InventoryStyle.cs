using Godot;
using System;
using System.ComponentModel;

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
    public int GridPadding { get; private set; } = 5;

    [Export]
    public int TileSize { get; private set; } = 50;
}
