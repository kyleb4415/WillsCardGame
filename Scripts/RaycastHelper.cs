using Godot;
using Godot.Collections;
using System;

public partial class RaycastHelper : Node
{ 
    public static Dictionary GetCollisionPoint(Camera3D camera, Vector2 mousePosition, float castDistance)
    {
        var worldspace = camera.GetWorld3D().DirectSpaceState;
        var start = camera.ProjectRayOrigin(mousePosition);
        var end = camera.ProjectPosition(mousePosition, castDistance); // Specify cast distance
        return worldspace.IntersectRay(PhysicsRayQueryParameters3D.Create(start, end));
    }
}
