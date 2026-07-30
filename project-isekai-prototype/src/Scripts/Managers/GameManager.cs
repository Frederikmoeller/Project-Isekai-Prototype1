using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    private Node2D levelRoot;
    private Node2D entitiesRoot;
    private CameraController cameraController;

    private PackedScene partyControllerScene;
    private PackedScene enemyScene;

    private Node2D activeLevel;
    private PartyController activeParty;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Initialize(
        Node2D levelRootNode,
        Node2D entitiesRootNode,
        CameraController camera,
        PackedScene partyScene,
        PackedScene enemySceneScene)
    {
        levelRoot = levelRootNode;
        entitiesRoot = entitiesRootNode;
        cameraController = camera;
        partyControllerScene = partyScene;
        enemyScene = enemySceneScene;
    }

    public void LoadLevel(PackedScene levelScene)
    {
        if (levelRoot == null || entitiesRoot == null)
        {
            GD.PushError("GameManager not initialized.");
            return;
        }

        if (activeLevel != null && IsInstanceValid(activeLevel))
        {
            activeLevel.QueueFree();
            activeLevel = null;
        }

        if (activeParty != null && IsInstanceValid(activeParty))
        {
            activeParty.QueueFree();
            activeParty = null;
        }

        activeLevel = levelScene.Instantiate<Node2D>();
        levelRoot.AddChild(activeLevel);

        SpawnPartyAndEnemies(activeLevel);
    }

    public void StartBattle(IEnumerable<Node2D> enemyTargets)
    {
        if (activeParty == null)
            return;

        activeParty.EnterBattle();

        var targets = new List<Node2D>();
        targets.AddRange(activeParty.GetCombatTargets());

        if (enemyTargets != null)
            targets.AddRange(enemyTargets.Where(t => t != null && IsInstanceValid(t)));

        cameraController?.EnterBattle(targets);
    }

    public void EndBattle()
    {
        cameraController?.ExitBattle();
        activeParty?.ExitBattle();
    }

    private void SpawnPartyAndEnemies(Node2D level)
    {
        if (partyControllerScene == null || enemyScene == null)
        {
            GD.PushError("GameManager scenes not assigned.");
            return;
        }

        var playerSpawnParent = FindSpawnContainer(level, "PlayerSpawn", "CharacterSpawn");
        var enemySpawnParent = level.GetNodeOrNull<Node>("EnemySpawns");

        var partySpawnPos = GetFirstMarkerPosition(playerSpawnParent);

        activeParty = partyControllerScene.Instantiate<PartyController>();
        entitiesRoot.AddChild(activeParty);
        activeParty.GlobalPosition = partySpawnPos;
        activeParty.SpawnParty();

        cameraController?.SetPartyRoot(activeParty);

        if (enemySpawnParent == null)
            return;

        foreach (var marker in enemySpawnParent.GetChildren().OfType<Marker2D>())
        {
            var enemy = enemyScene.Instantiate<Node2D>();
            entitiesRoot.AddChild(enemy);
            enemy.GlobalPosition = marker.GlobalPosition;
        }
    }

    private static Node FindSpawnContainer(Node level, params string[] names)
    {
        foreach (var name in names)
        {
            var node = level.GetNodeOrNull<Node>(name);
            if (node != null)
                return node;
        }

        return null;
    }

    private static Vector2 GetFirstMarkerPosition(Node parent)
    {
        if (parent == null)
            return Vector2.Zero;

        var marker = parent.GetChildren().OfType<Marker2D>().FirstOrDefault();
        return marker?.GlobalPosition ?? Vector2.Zero;
    }
}