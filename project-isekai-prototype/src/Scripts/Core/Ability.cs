using Godot;

public enum AbilityTargetType
{
    Self,
    SingleTarget,
    Area
}

[GlobalClass]
public partial class Ability : Resource
{
    [Export] public string AbilityName { get; set; }
    [Export] public int ManaCost { get; set; } = 10;
    [Export] public AbilityTargetType TargetType { get; set; } = AbilityTargetType.SingleTarget;
    [Export] public float Range { get; set; } = 200f;
    [Export] public float Radius { get; set; } = 64f;

    [Export] public int Damage { get; set; }
    [Export] public int HealAmount { get; set; }
    [Export] public int BuffAmount { get; set; }
    [Export] public float BuffDuration { get; set; }
    [Export] public float KnockbackForce { get; set; }
    [Export] public float TauntDuration { get; set; }

    [Export] public bool DealsDamage { get; set; }
    [Export] public bool Heals { get; set; }
    [Export] public bool AppliesBuff { get; set; }
    [Export] public bool AppliesKnockback { get; set; }
    [Export] public bool AppliesTaunt { get; set; }
}