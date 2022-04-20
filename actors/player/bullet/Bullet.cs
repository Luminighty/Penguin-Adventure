using Godot;
using System;

public class Bullet : KinematicBody2D
{
    public enum Kind { Small, Big }
    public Vector2 velocity;
    public Kind kind;
    public float lifeTime;


    public override void _Ready() {
        var sprite = GetNode<Sprite>("Sprite");
        var collision = GetNode<CollisionShape2D>("CollisionShape2D");
        var shape = collision.Shape as RectangleShape2D;
        sprite.Frame = (int)kind;
        switch (kind) {
            case Kind.Small:
                shape.Extents = Vector2.One * 1;
                break;
            case Kind.Big:
                shape.Extents = Vector2.One * 3;
                break;
        }
    }

    public override void _Process(float delta) {
        lifeTime -= delta;
        if (lifeTime < 0)
            Destroy();
        
        var collision = MoveAndCollide(velocity * delta);
        if (collision != null)
            Destroy();
    }

    private void Destroy() {
        QueueFree();
    }
}
