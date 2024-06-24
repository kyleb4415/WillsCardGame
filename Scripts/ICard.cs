using Godot;
using System;
using System.Reflection.Metadata;

public interface ICard
{
	string Name { get; set; }

#nullable enable
	System.Data.SQLite.SQLiteBlob CardImage { get; set; }
	System.Data.SQLite.SQLiteBlob TypeImage { get; set; }

#nullable disable
	string Type { get; set; }
	int ManaCost { get; set; }
	int UnlockedFlag { get; set; }
	string Description { get; set; }
}
