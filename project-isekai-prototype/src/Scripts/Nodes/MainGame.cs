using Godot;

public partial class MainGame : Node
{
    [Export] public NodePath LevelRootPath { get; set; } = "World/LevelRoot";
    [Export] public NodePath EntitiesRootPath { get; set; } = "World/EntitiesRoot";
    [Export] public NodePath CameraPath { get; set; } = "World/CameraController";

    [Export] public PackedScene PartyControllerScene { get; set; }
    [Export] public PackedScene EnemyScene { get; set; }
    [Export] public PackedScene InitialLevelScene { get; set; }

    public override void _Ready()
    {
        var levelRoot = GetNode<Node2D>(LevelRootPath);
        var entitiesRoot = GetNode<Node2D>(EntitiesRootPath);
        var camera = GetNode<CameraController>(CameraPath);

        GameManager.Instance.Initialize(
            levelRoot,
            entitiesRoot,
            camera,
            PartyControllerScene,
            EnemyScene);

        if (InitialLevelScene != null)
            GameManager.Instance.LoadLevel(InitialLevelScene);
    }
}