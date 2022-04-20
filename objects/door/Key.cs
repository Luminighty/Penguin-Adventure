using Godot;
using System;

[Tool]
public class Key : Area2D
{
    private KeyType _key;
    [Export]
    public KeyType key {
        get {  return _key; }
        set {
            _key = value;
            #if DEBUG
                var sprite = GetNode<Sprite>("Sprite");
                var coords = sprite.FrameCoords;
                coords.y = (int)value;
                sprite.FrameCoords = coords;
            #endif
        }
    }

    public void _on_body_entered(Node body) {
        Player player = body as Player;
        if (player == null)
            return;
        player.keys[key]++;
        foreach (var item in player.keys)
            GD.Print($"{item.Key} : {item.Value}");
        QueueFree();
    }
}
