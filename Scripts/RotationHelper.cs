using Godot;
using Godot.Collections;
using System;
using System.Linq;

public static class RotationHelper
{
    public static void RotateCard(Card c, Vector3 collisionPoint, SceneTree s)
    {
        Tween tween = s.CreateTween();
        Tween tween2 = s.CreateTween();
        c.GravityScale = 0;
        tween.TweenProperty(c, "position", new Vector3(c.PlacedPos.X, c.PlacedPos.Y + 0.1f, c.PlacedPos.Z), 0.1f).SetTrans(Tween.TransitionType.Quad);

        float z = (1.5f * c.Position.Z - collisionPoint.Z * 1.5f);
        float x = (1.5f * c.Position.X - collisionPoint.X * 1.5f);
        //leave y the same
        tween2.TweenProperty(c, "rotation", new Vector3(x, (c.Rotation.Y), z), 0.25f).SetTrans(Tween.TransitionType.Quad);
        (c.GetNode("SelectedLight") as OmniLight3D).SetLayerMaskValue(1, true);
    }

    public static void ResetRotation(Card c, SceneTree s)
    {
        if(c.Selected == false)
        {
            Tween tween = s.CreateTween();
            Tween tween2 = s.CreateTween();
            tween.TweenProperty(c, "position", new Vector3(c.PlacedPos.X, c.PlacedPos.Y, c.PlacedPos.Z), 0.1f);
            tween2.TweenProperty(c, "rotation", new Vector3(0f, 0f, 0f), 0.25f).SetTrans(Tween.TransitionType.Quad);
            tween.Finished += () => {
                c.GravityScale = 1f;
                c.Set("rotation", new Vector3(0f, 0f, 0f));
                float x = c.PlacedPos.X;
                float y = c.PlacedPos.Y;
                float z = c.PlacedPos.Z;
                c.Set("position", new Vector3(x, y, z));
                (c.GetNode("SelectedLight") as OmniLight3D).SetLayerMaskValue(1, false);
            };
        }
        else
        {
            Tween tween = s.CreateTween();
            tween.TweenProperty(c, "rotation", new Vector3(0f, 0f, 0f), 0.25f).SetTrans(Tween.TransitionType.Quad);
        }
    }
}
