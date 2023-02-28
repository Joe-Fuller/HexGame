using Godot;
using System;

public class CameraController : Camera2D
{
    [Signal] private delegate void ReachedLocation();
    private float TargetXPos = 0;
    private float SmoothSpeed = 2.5f;

    public override void _Process(float delta)
    {
        base._Process(delta);
        float PosDiff = TargetXPos - Position.x;
        Vector2 SmoothedVelocity = new Vector2(PosDiff * SmoothSpeed * delta, 0);
        Position += SmoothedVelocity;
        if (Math.Abs(Position.x - TargetXPos) < 2)
        {
            EmitSignal("ReachedLocation");
        }
    }

    public void SetTargetXPos(float NewXPos)
    {
        TargetXPos = NewXPos;
    }
}
