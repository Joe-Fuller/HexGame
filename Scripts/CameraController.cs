using Godot;
using System;

public class CameraController : Camera2D
{
    public void MoveToShopPosition()
    {
        Position -= new Vector2(1000, 0);
    }

    public void MoveToCombatPosition()
    {
        Position += new Vector2(1000, 0);
    }
}
