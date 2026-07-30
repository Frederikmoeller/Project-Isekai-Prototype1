using Godot;

public partial class Character : CharacterBody2D
{
    [Export] public BaseInfo BaseInfo {get; set; }
    [Export] public int CurrentMana { get; set; }
    [Export] public Godot.Collections.Array<Ability> Abilities { get; set; } = new();
    public override void _Ready()
    {
        if (BaseInfo != null && CurrentMana <= 0)
        {            
            CurrentMana = BaseInfo.Mana;
            Modulate = BaseInfo.CharacterColor;;
        }
    }

    public override void _Process(double delta)
    {
        
    }

    public bool CanCast(Ability ability)
    {
        return ability != null && CurrentMana >= ability.ManaCost;
    }

    public bool CastAbility(int index, Node2D target = null)
    {
        if (index < 0 || index >= Abilities.Count)
            return false;

        var ability = Abilities[index];
        if (!CanCast(ability))
            return false;

        if (ability.TargetType == AbilityTargetType.SingleTarget && target == null)
            return false;

        CurrentMana -= ability.ManaCost;

        ApplyAbility(ability, target);
        return true;
    }

    private void ApplyAbility(Ability ability, Node2D target)
    {
        if (ability.TargetType == AbilityTargetType.Self)
            target = this;

        if (ability.DealsDamage && target != null)
            GD.Print($"{Name} deals {ability.Damage} damage to {target.Name}");

        if (ability.Heals && target != null)
            GD.Print($"{Name} heals {target.Name} for {ability.HealAmount}");

        if (ability.AppliesBuff && target != null)
            GD.Print($"{Name} buffs {target.Name} by {ability.BuffAmount} for {ability.BuffDuration}s");

        if (ability.AppliesKnockback && target != null)
            GD.Print($"{Name} knocks back {target.Name} with force {ability.KnockbackForce}");

        if (ability.AppliesTaunt && target != null)
            GD.Print($"{Name} taunts {target.Name} for {ability.TauntDuration}s");
    }
}