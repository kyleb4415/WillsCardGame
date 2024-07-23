using Godot;
using System;

public static class PngImageLoader
{
    public static void LoadPngFromDatabase(Card c, int width, int height)
    {
        ImageTexture imgTexture = new ImageTexture();

        try
        {
            Image img = new Image();
            byte[] bytes = new byte[c.CardImage.GetCount()];
            GD.Print(bytes.Length);
            c.CardImage.Read(bytes, c.CardImage.GetCount(), 0);
            img.LoadPngFromBuffer(bytes);
            Mesh mesh = (Mesh)c.GetNode("CardMesh").Get("mesh");
            StandardMaterial3D material3D = (StandardMaterial3D)mesh.Get("material");
            imgTexture = ImageTexture.CreateFromImage(img);
            imgTexture.SetSizeOverride(new Vector2I(width, height));
            material3D.Set("albedo_texture", ImageTexture.CreateFromImage(img));
        }
        catch (Exception ex)
        {
            GD.Print(ex.Message);
        }
    }
}
