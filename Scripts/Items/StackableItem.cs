using Godot;
using System;
using System.Diagnostics;

public partial class StackableItem : Item
{

    public int ItemAmount { get; private set; }
    public int MaxItemAmount { get; protected set; }

	protected RichTextLabel AmountText { get; set; }

    public StackableItem()
	: base()
	{
	}

	~StackableItem()
	{

	}

    protected override void InitInternal()
    {
        base.InitInternal();

		AmountText = new RichTextLabel();

		AddChild( AmountText );
        AmountText.Owner = this;

		AmountText.ClipContents = false;
		AmountText.AutowrapMode = TextServer.AutowrapMode.Off;
		AmountText.ScrollActive = false;
		AmountText.AddThemeFontSizeOverride( "normal_font_size", 12 );
    }

    public override void SetTileSize( Vector2I size )
    {
        base.SetTileSize( size );

		AmountText.SetSize( new Vector2( 0, 15 ) );
		AmountText.Position = new Vector2( 3, Size.X - AmountText.Size.Y - 3  );
    }

    public override void Rotate()
    {
        base.Rotate();

        AmountText.SetSize( new Vector2( 0, 15 ) );
        AmountText.Position = new Vector2( 0, Size.X - 15 );
    }

    /// <summary>
    /// Adds an amount of items.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>
	/// <b>Positive</b> the value has been added and the ramainder is returned. <br/>
	/// <b>Negative</b> the value has <b>NOT</b> been added and the amount needed for it to not go negative is returned.
	/// </returns>
    public int AddAmount( int amount ) 
	{ 
		Debug.Assert( amount >= 0, "StackableItem: amount must be 0 or higher, use Remove amount instead" );

		int amountAfter = ItemAmount + amount;

		if( amountAfter > MaxItemAmount )
		{
			SetAmount( MaxItemAmount );
			return amountAfter - MaxItemAmount;
		}
		else if ( amountAfter < 0 )
		{
			return amountAfter;
		}
		else
		{
			SetAmount( amountAfter );
			return 0;
		}
	}

	public void SetAmount( int amount )
	{
		ItemAmount = amount;
		AmountText.Text = FormatInt( amount );
	}

	private string FormatInt( int amount )
	{
		if( amount <= 999 )
			return amount.ToString();

		string format = amount.ToString();

		return format.Insert( format.Length - 3, " " );
	}
}
