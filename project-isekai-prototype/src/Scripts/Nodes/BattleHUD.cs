using Godot;

public partial class BattleHUD : Control
{
    [Export] public AbilityButton[] AbilityButtons { get; set; }

    public void AssignCharacter(Character character)
    {
        if (AbilityButtons == null)
            return;

        foreach (var button in AbilityButtons)
        {
            button?.SetLinkedCharacter(character);
        }
    }
}