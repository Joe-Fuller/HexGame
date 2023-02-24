using Godot;
using System;

public class CameraController : Camera2D
{
    private float TargetXPos = 0;
    private float SmoothSpeed = 1.5f;

    public override void _Process(float delta)
    {
        base._Process(delta);
        float PosDiff = TargetXPos - Position.x;
        Vector2 SmoothedVelocity = new Vector2(PosDiff * SmoothSpeed * delta, 0);
        Position += SmoothedVelocity;
    }

    public void SetTargetXPos(float NewXPos)
    {
        TargetXPos = NewXPos;
    }
    public void MoveToShopPosition()
    {
        Position -= new Vector2(1000, 0);
    }

    public void MoveToCombatPosition()
    {
        Position += new Vector2(1000, 0);
    }
}
