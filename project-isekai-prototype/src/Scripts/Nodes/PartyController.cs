using System.Collections.Generic;
using Godot;

public partial class PartyController : Node2D
{
    [Export] public PackedScene CharacterScene { get; set; }

    [Export] public Godot.Collections.Array<Vector2> FormationOffsets { get; set; } = new()
    {
        new Vector2(0, 0),
        new Vector2(-64, 48),
        new Vector2(64, 48),
        new Vector2(-128, 48),
        new Vector2(128, 48)
    };

    [Export] public float ReturnToFormationSpeed { get; set; } = 6f;

    private readonly List<Character> members = new();
    private readonly Dictionary<Character, Vector2> savedOffsets = new();
    private bool inBattle;
    private bool spawned;

    public override void _Ready()
    {
        SpawnParty();
    }

    public override void _Process(double delta)
    {
        if (inBattle)
            return;

        float dt = (float)delta;

        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            if (member == null || !IsInstanceValid(member))
                continue;

            Vector2 targetLocal = savedOffsets.TryGetValue(member, out var offset)
                ? offset
                : GetDefaultOffset(i);

            member.GlobalPosition = member.GlobalPosition.Lerp(GlobalPosition + targetLocal, ReturnToFormationSpeed * dt);
        }
    }

    public void SpawnParty()
    {
        if (spawned)
            return;

        if (CharacterScene == null)
        {
            GD.PushError("PartyController CharacterScene not assigned.");
            return;
        }

        members.Clear();
        savedOffsets.Clear();

        for (int i = 0; i < FormationOffsets.Count; i++)
        {
            var member = CharacterScene.Instantiate<Character>();
            member.Position = FormationOffsets[i];
            AddChild(member);
            members.Add(member);
        }

        spawned = true;
        SaveCurrentFormation();
    }

    public void BindMembers(IEnumerable<Character> newMembers)
    {
        members.Clear();
        members.AddRange(newMembers);
        SaveCurrentFormation();
    }

    public void EnterBattle()
    {
        inBattle = true;
        SaveCurrentFormation();
    }

    public void ExitBattle()
    {
        inBattle = false;
    }

    public IEnumerable<Node2D> GetCombatTargets()
    {
        foreach (var member in members)
        {
            if (member != null && IsInstanceValid(member))
                yield return member;
        }
    }

    private void SaveCurrentFormation()
    {
        savedOffsets.Clear();

        foreach (var member in members)
        {
            if (member == null || !IsInstanceValid(member))
                continue;

            savedOffsets[member] = member.GlobalPosition - GlobalPosition;
        }
    }

    private Vector2 GetDefaultOffset(int index)
    {
        if (index < FormationOffsets.Count)
            return FormationOffsets[index];

        return new Vector2(index * 48, 0);
    }
}