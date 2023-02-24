using Godot;
using System;

public class CombatUI : Control
{
    public CombatManager CombatManager;
    private GameManager GameManager;
    public override void _Ready()
    {
        CombatManager = GetParent<CombatManager>();
        GameManager = GetNode<GameManager>("../..");
    }

    public void OnPauseButtonPressed()
    {
        if (CombatManager.IsPaused)
        {
            GD.Print("Unpausing");
            CombatManager.IsPaused = false;
            CombatManager.Unpause();
        }
        else
        {
            GD.Print("Pausing");

            CombatManager.IsPaused = true;
        }
    }

    public void OnFasterButtonPressed()
    {
        CombatManager.TurnTime /= 2;
        if (CombatManager.TurnTime < 0.25)
        {
            CombatManager.TurnTime = 0.25f;
        }
    }

    public void OnSlowerButtonPressed()
    {
        CombatManager.TurnTime *= 2;
        if (CombatManager.TurnTime > 4)
        {
            CombatManager.TurnTime = 4;
        }
    }

    public void OnStartCombatButtonPressed()
    {
        GameManager.CameraController.SetTargetXPos(0);
        CombatManager.StartCombat();
    }
}
