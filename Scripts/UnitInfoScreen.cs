using Godot;
using System;

public class UnitInfoScreen : CanvasLayer
{
    Panel Panel;
    Label NameText;
    Label IconText;
    Label HealthText;
    Label DamageText;
    Label DescriptionText;

    GameManager GameManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visible = false;
        Panel = GetNode<Panel>("./Panel");
        NameText = GetNode<Label>("./Panel/NameText");
        IconText = GetNode<Label>("./Panel/IconText");
        HealthText = GetNode<Label>("./Panel/HealthText");
        DamageText = GetNode<Label>("./Panel/DamageText");
        DescriptionText = GetNode<Label>("./Panel/DescriptionText");
        GameManager = GetNode<GameManager>("..");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (Visible)
        {
            // update position and info
            Vector2 MousePos = GameManager.Tilemap.GetGlobalMousePosition();
            Vector2 TileMousedOver = GameManager.Tilemap.WorldToMap(MousePos * 2);
            Unit MousedOverUnit = GameManager.GetUnitOnTile(TileMousedOver);

            if (MousedOverUnit != null)
            {
                SetText(MousedOverUnit);
            }

            // Addition by Camera Pos is because I'm bad I think
            Panel.SetPosition(MousePos - GameManager.CameraController.Position);

            // Stop Panel Moving Off Screen
            while (Panel.GetRect().End.x > 1920)
            {
                Panel.SetPosition(Panel.RectPosition + new Vector2(-1, 0));
            }
            while (Panel.GetRect().End.y > 1080)
            {
                Panel.SetPosition(Panel.RectPosition + new Vector2(0, -1));
            }
        }
    }

    public void SetText(Unit Unit)
    {
        NameText.Text = Unit.UnitName;
        IconText.Text = Unit.UnitIcon;
        HealthText.Text = Unit.Health.ToString() + " Health";
        DamageText.Text = Unit.Damage.ToString() + " Damage";
        DescriptionText.Text = Unit.Description;

        // Set Size based on DescriptionText size
        int Length = (int)DescriptionText.GetRect().End.y;

        Panel.RectSize = new Vector2(320, Length + 16);
    }
}
