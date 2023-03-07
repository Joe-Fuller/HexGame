using Godot;
using System;

public partial class CameraController : Camera2D
{
    [Signal] public delegate void ReachedLocationEventHandler();
    private float TargetXPos = 0;
    private float SmoothSpeed = 2.5f;

    public override void _Process(double delta)
    {
        base._Process(delta);
        float PosDiff = TargetXPos - Position.X;
        Vector2 SmoothedVelocity = new Vector2(PosDiff * SmoothSpeed * (float)delta, 0);
        Position += SmoothedVelocity;
        if (Math.Abs(Position.X - TargetXPos) < 2)
        {
            EmitSignal("ReachedLocation");
        }
    }

    public void SetTargetXPos(float NewXPos)
    {
        TargetXPos = NewXPos;
    }
}
