using System.Linq;
using Godot;

public partial class AbilityButton : Control
{
    [Export] public Character LinkedCharacter { get; set; }
    [Export] public int AbilityIndex { get; set; }
    [Export] public Label labelText { get; set; }

    private TextureButton textureButton;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        textureButton = GetNode<TextureButton>("TextureButton");
        textureButton.MouseFilter = MouseFilterEnum.Stop;
        textureButton.Pressed += OnPressed;
        RefreshCharacterLabel();
    }

    private void OnPressed()
    {
        GD.Print("Pressed " + Name);
        LinkedCharacter?.CastAbility(AbilityIndex);
    }

    public void SetLinkedCharacter(Character character)
    {
        LinkedCharacter = character;
        RefreshCharacterLabel();
    }

    private void RefreshCharacterLabel()
    {
        if (labelText == null)
            return;

        if (LinkedCharacter?.BaseInfo?.Name == null || LinkedCharacter.BaseInfo.Name.Length == 0)
        {
            labelText.Text = "";
            return;
        }

        labelText.Text = LinkedCharacter.BaseInfo.Name.ElementAt(0).ToString();
        textureButton.Modulate = LinkedCharacter.Modulate;
    }
}
