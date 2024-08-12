using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection.Metadata;

public partial class CardNode : Node
{
    private SQLiteConnection db;
    private string dbName = "DataStore/CardData.db";
    private string cardName;

    public override void _Ready()
    {
        db = new SQLiteConnection($"Data Source={dbName};Version=3;");
    }

    public void SetCardName(string name)
    {
        cardName = name;
        LoadImages();
    }

    private void LoadImages()
    {
        string characterImageQuery = $"SELECT Images FROM Card WHERE Name = '{cardName}'";
        string characterImageCheck = ExecuteScalarQuery(characterImageQuery);

        //string borderQuery = $"SELECT Race.Border FROM Card JOIN Race ON Card.Race_ID = Race.ID WHERE Card.Name = '{cardName}'";
        string borderQuery = $"SELECT Race.Border FROM Card JOIN Race ON Card.Race_ID = Race.ID WHERE Card.Name = '{cardName}'";
        string borderImageCheck = ExecuteScalarQuery(borderQuery);

        //string typeQuery = $"SELECT Type.Image FROM Card JOIN Type ON Card.Type_ID = Type.ID WHERE Card.Name = '{cardName}'";
        string typeQuery = $"SELECT Type.Image FROM Card JOIN Type ON Card.Type_ID = Type.ID WHERE Card.Name = '{cardName}'";
        string typeImageCheck = ExecuteScalarQuery(typeQuery);

        if (!string.IsNullOrEmpty(characterImageCheck))
        {
            byte[] characterBlob = ExecuteScalarQueryBlob(characterImageQuery);
            ImageTexture img = LoadPngFromBlob(characterBlob);

            TextureRect rect = GetNode<TextureRect>("CardViewport/CardImage");
            rect.StretchMode = TextureRect.StretchModeEnum.Scale;
            rect.Texture = (Texture2D)img;
        }
        else
        {
            GD.Print("null character image check");
        }

        if (!string.IsNullOrEmpty(borderImageCheck))
        {
            byte[] borderBlob = ExecuteScalarQueryBlob(borderQuery);
            ImageTexture img = LoadPngFromBlob(borderBlob);
            GetNode<Sprite2D>("CardViewport/Border").Texture = (Texture2D)img;
        }

        if (!string.IsNullOrEmpty(typeImageCheck))
        {
            byte[] typeBlob = ExecuteScalarQueryBlob(typeQuery);
            ImageTexture img = LoadPngFromBlob(typeBlob);
            TextureRect rect = GetNode<TextureRect>("CardViewport/TypesContainer/Type");
            rect.StretchMode = TextureRect.StretchModeEnum.KeepAspect;
            img.SetSizeOverride((Vector2I)rect.Size);
            rect.Texture = (Texture2D)img;
        }
    }

    private string ExecuteScalarQuery(string query)
    {
        db = new SQLiteConnection($"Data Source={dbName};Version=3;");
        db.Open();
        using (var command = new SQLiteCommand(query, db))
        {
            var result = command.ExecuteScalar();
            return result != null ? result.ToString() : string.Empty;
        }
    }


    private byte[] ExecuteScalarQueryBlob(string query)
    {
        db = new SQLiteConnection($"Data Source={dbName};Version=3;");
        db.Open();
        using (var command = new SQLiteCommand(query, db))
        {
            var result = command.ExecuteScalar();
            return result != null ? (byte[])result : null;
        }
    }

    private ImageTexture LoadPngFromBlob(byte[] blob)
    {
        Image img = new Image();
        //byte[] bytes = new byte[blob.GetCount()];
        //GD.Print(bytes.Length);
        //blob.Read(bytes, blob.GetCount(), 0);
        img.LoadPngFromBuffer(blob);
        ImageTexture imgTexture = ImageTexture.CreateFromImage(img);
        GD.Print("creating image texture");
        return imgTexture;

    }

}
