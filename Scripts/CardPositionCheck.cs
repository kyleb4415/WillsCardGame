using Godot;
using System;
using System.Collections.Generic;

public static class CardPositionCheck
{
    private static Node3D GetPlayerSpace(BoardController b, Card c)
    {
        foreach (Node3D space in b.PlayerSpaces)
        {
            if(space.GetChild<Area3D>(0).GetOverlappingBodies().Count > 1)
            {
                if(space.GetChild<Area3D>(0).GetOverlappingBodies()[1] == c)
                {
                    return space.GetParent<Node3D>();
                    GD.Print("found it");
                }
            }
        }
        GD.Print("nah no space fam");
        return null;
    }
    public static bool PathClear(BoardController b, Card c)
    {
        List<Node3D> enemySpaces = b.Enemy.EnemySpaces;
        List<Node3D> playerSpaces = b.PlayerSpaces;
        Node3D n = GetPlayerSpace(b, c);
        switch(n.Name)
        {
            case ("Space"):
                GD.Print("enemy unreachable!");
                return false;
            case ("Space2"):
                if (enemySpaces[1].GetChildCount() > 0 || enemySpaces[7].GetChildCount() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case ("Space3"):
                if (enemySpaces[2].GetChildCount() > 0 || enemySpaces[6].GetChildCount() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case ("Space4"):
                if (enemySpaces[3].GetChildCount() > 0 || enemySpaces[5].GetChildCount() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            case ("Space5"):
                GD.Print("enemy unreachable!");
                return false;
            case ("Space6"):
                GD.Print("enemy unreachable!");
                return false;
            case ("Space7"):
                GD.Print("enemy unreachable!");
                return false;
            case ("Space8"):
                GD.Print("enemy unreachable!");
                return false;
        }
        return true;
    }
}
