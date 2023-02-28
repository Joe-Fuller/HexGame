using Godot;

public class RoundOverScreen : CanvasLayer
{
    Label Title;
    Button Button;
    private GameManager GameManager;
    public override void _Ready()
    {
        Title = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/Title");
        Button = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/ButtonContainer/ResetButton");
        GameManager = GetNode<GameManager>("..");
    }

    private void OnResetButtonPressed()
    {
        // GetTree().ChangeScene("res://Scenes/GameManager.tscn");
        GameManager.Shop.EnterShopMode();
        this.Visible = false;
    }

    public void SetTitle(bool PlayerWon)
    {
        Title.Text = (PlayerWon ? "You Won!" : "You Lost!");
        Button.Text = (PlayerWon ? "Next Round" : "Reset");
    }
}
