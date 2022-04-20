using Godot;
using System;

public class Lock : Node
{
    [Export]
    public KeyType requiredKey;

    private Player overlappingPlayer = null;

    private Node door;
    private Sprite lockSprite;
    private Area2D area2D;

    public override void _Ready() {
        lockSprite = GetNode<Sprite>("Lock");
        area2D = GetNode<Area2D>("Lock/Area2D");
        door = GetNode("Door");
    }

    public override void _Input(InputEvent inputEvent) {
        if (inputEvent.IsActionPressed(Keys.Interact) && canOpen()) {
            open();
        }
    }

    private bool canOpen() {
        return overlappingPlayer != null && overlappingPlayer.keys[requiredKey] > 0;
    }

    private void open() {
        lockSprite.Frame += 1;
        overlappingPlayer.keys[requiredKey]--;
        door.QueueFree();
        area2D.QueueFree();
    }

    public void _on_body_entered(Node body) {
        overlappingPlayer = body as Player;
    }

    public void _on_body_exited(Node body) {
        if (body is Player)
            overlappingPlayer = null;
    }

}
