using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CameraController : Camera2D
{
    [Export] public float FollowLerpSpeed { get; set; } = 6f;
    [Export] public float BattleLerpSpeed { get; set; } = 10f;
    [Export] public Vector2 ExplorationZoom { get; set; } = new Vector2(1.0f, 1.0f);
    [Export] public Vector2 BattleZoom { get; set; } = new Vector2(0.75f, 0.75f);
    [Export] public float ZoomLerpSpeed { get; set; } = 8f;

    private Node2D partyRoot;
    private readonly List<Node2D> battleTargets = new();
    private bool inBattle;

    public override void _Ready()
    {
        Zoom = ExplorationZoom;
        PositionSmoothingEnabled = false;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (inBattle)
        {
            var focus = GetBattleFocus();
            if (focus.HasValue)
                GlobalPosition = GlobalPosition.Lerp(focus.Value, Mathf.Clamp(BattleLerpSpeed * dt, 0f, 1f));

            Zoom = Zoom.Lerp(BattleZoom, Mathf.Clamp(ZoomLerpSpeed * dt, 0f, 1f));
        }
        else if (partyRoot != null && IsInstanceValid(partyRoot))
        {
            GlobalPosition = GlobalPosition.Lerp(partyRoot.GlobalPosition, Mathf.Clamp(FollowLerpSpeed * dt, 0f, 1f));
            Zoom = Zoom.Lerp(ExplorationZoom, Mathf.Clamp(ZoomLerpSpeed * dt, 0f, 1f));
        }
    }

    public void SetPartyRoot(Node2D root)
    {
        partyRoot = root;
    }

    public void EnterBattle(IEnumerable<Node2D> targets)
    {
        battleTargets.Clear();
        battleTargets.AddRange(targets.Where(t => t != null && IsInstanceValid(t)));
        inBattle = true;
    }

    public void ExitBattle()
    {
        battleTargets.Clear();
        inBattle = false;
    }

    private Vector2? GetBattleFocus()
    {
        if (battleTargets.Count == 0)
            return null;

        Vector2 sum = Vector2.Zero;
        int count = 0;

        foreach (var target in battleTargets)
        {
            if (target == null || !IsInstanceValid(target))
                continue;

            sum += target.GlobalPosition;
            count++;
        }

        if (count == 0)
            return null;

        return sum / count;
    }
}