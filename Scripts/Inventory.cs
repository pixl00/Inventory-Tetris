using Godot;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Diagnostics;

[Tool]
public partial class Inventory : Control
{
	[Export]
	Vector2I GridSize { get; set; } = Vector2I.One;
	[Export( PropertyHint.Range, "0, 20" )] 
	int TilePadding { get; set; }
	[Export( PropertyHint.Range, "0, 20" )] 
	int GridPadding { get; set; }


	[ExportGroup( "Paths" )]
	[Export] 
	Texture2D TileTexture { get; set; } = (Texture2D)GD.Load( "res://Resources/KenneyAssets/ItemBorder.png" );

	[Description( "Yes" )]
    [ExportGroup( "Nodes" )]
    [Export]
    GridContainer Grid { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if(Engine.IsEditorHint())
            ResizeGrid();
	}
	
	private void ResizeGrid()
	{
		Godot.Collections.Array<Node> tiles = Grid.GetChildren();
		if( tiles.Count != GridSize.X * GridSize.Y )
		{
			// Remove all tiles
			foreach( Node tile in tiles )
				Grid.RemoveChild( tile );

			// Add new tiles
			for( int i = 0; i < GridSize.X * GridSize.Y; i++ )
			{
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = TileTexture;
				Grid.AddChild( textureRect );
			}
		}


        Grid.Columns = GridSize.X;
		Grid.Size = Vector2.Zero;
		//Position = Vector2.Zero;

		// Set padding between tiles
        Grid.AddThemeConstantOverride( "h_separation", TilePadding );
        Grid.AddThemeConstantOverride( "v_separation", TilePadding );

		Vector2 NewSize = new Vector2(
			TileTexture.GetWidth() * GridSize.X + (GridSize.X - 1) * TilePadding + GridPadding * 2,
			TileTexture.GetHeight() * GridSize.Y + (GridSize.Y - 1) * TilePadding + GridPadding * 2 );

        Grid.Position = Vector2.One * GridPadding;

        Size = NewSize;
    }
}
