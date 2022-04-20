using Godot;
using System;

public class SpawnPoint
{
    private Actor parent;
    private Vector2 position;

    public SpawnPoint(Actor parent) {
        this.parent = parent;
        position = parent.Position;
    }

    public void Respawn() {
        parent.Position = position;
    }

    public void SetSpawnPoint(Vector2 position) {
        this.position = position;
    }
}
