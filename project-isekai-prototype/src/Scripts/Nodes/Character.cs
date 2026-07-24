using Godot;

public partial class Character : CharacterBody2D
{
    [Export] public BaseInfo BaseInfo {get; set; }
    public override void _Ready()
    {
        GD.Print($"Player layer: {CollisionLayer}");
    }

    public override void _Process(double delta)
    {
        
    }
}