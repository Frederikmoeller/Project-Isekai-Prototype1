using Godot;
using System;

[GlobalClass]
public partial class BaseInfo : Resource
{
    [Export] public string Name { get; set; }
    [Export] public int Health { get; set; }
    [Export] public int Armour { get; set; }
    [Export] public int Mana { get; set; }
    [Export] public int Speed { get; set; }
    [Export] public int Attack { get; set; }
}