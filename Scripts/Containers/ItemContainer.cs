using Godot;
using System;

[GlobalClass]
public partial class ItemContainer : Item
{
	public Inventory Inventory { get; set; }

	public ItemContainer()
	: base()
	{
	}
}
