using Godot;
using System;

public class AbilityFish : Area2D
{
    [Export]
    public Ability ability;

    public void _on_body_entered(Node node) {
        Player player = node as Player;
        if (player == null)
            return;
        player.addAbility(ability);
        QueueFree();
    }
}
