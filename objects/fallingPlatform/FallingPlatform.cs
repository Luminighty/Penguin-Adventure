using Godot;
using System;
using System.Collections.Generic;

[Tool]
public class FallingPlatform : StaticBody2D, ICollisionDetector
{
    [Export]
    public int length;

    [Export]
    public float maxAnimDelay;
    [Export]
    public float maxFallDelay;
    [Export]
    public PackedScene destroyParticles;

    private Bound<float> fallDelay;
    private Bound<float> animDelay;

    private Sprite sprite;
    private List<Sprite> sprites;
    private CollisionShape2D collision;
    private RectangleShape2D shape;
    private int lastSprite = 1;


    public override void _Ready() {
        fallDelay = new Bound<float>(maxFallDelay, 0, maxFallDelay);
        animDelay = new Bound<float>(maxAnimDelay, 0, maxAnimDelay);
        sprite = GetNode<Sprite>("Sprite");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        shape = collision.Shape as RectangleShape2D;
        sprites = new List<Sprite>();
        foreach (Node node in GetChildren())
            if (node is Sprite && node != sprite)
                sprites.Add(node as Sprite);
        _processTool();
    }

    public override void _Process(float delta) {
        if (Engine.EditorHint) {
            _processTool(); 
        } else {
            _ProcessPlatform(delta);
        }

    }

    public void _processTool() {
        if (sprites.Count == length - 1 || length < 1)
            return;
        shape.Extents = new Vector2(8 * length, shape.Extents.y);
        collision.Position = shape.Extents;
        GD.Print("Was: ", sprites.Count);
        while (sprites.Count > length - 1) {
            Sprite spr = sprites[sprites.Count - 1];
            sprites.RemoveAt(sprites.Count - 1);
            spr.QueueFree();
        }
        while (sprites.Count < length - 1) {
            Sprite spr = sprite.Duplicate(0) as Sprite;
            AddChild(spr, true);
            sprites.Add(spr);
            spr.Position = Vector2.Right * 16 * sprites.Count;
        }
        for(int i = 0; i < sprites.Count; i++) {
            if (i == sprites.Count - 1) {
                sprites[i].FrameCoords = Vector2.Right * (sprite.Hframes - 2);
            } else {
                sprites[i].FrameCoords = Vector2.Right * ((i % (sprite.Hframes - 3)) + 1);
            }
        }
        GD.Print(sprites.Count);

        sprite.FrameCoords = Vector2.Right * ((sprites.Count == 0) ? sprite.Hframes - 1 : 0);
    }

    public void setSprites(int yCoord) {
        yCoord++;
        sprite.FrameCoords = new Vector2(sprite.FrameCoords.x, yCoord);
        foreach (var sprite in sprites)
            sprite.FrameCoords = new Vector2(sprite.FrameCoords.x, yCoord);
    }

    public void _ProcessPlatform(float delta) {
        if (!fallDelay.isMax) {
            fallDelay += delta;
            animDelay += delta;
            if (animDelay.isMax) {
                lastSprite++;
                lastSprite %= sprite.Vframes - 1;
                setSprites(lastSprite);
                animDelay.SetToMin();
            }
            if (fallDelay.isMax)
                Destroy();
        }
    }

    public void Destroy() {
        for(int i = 0; i < length; i++) {
            Particles2D instance = destroyParticles.Instance<Particles2D>();
            GetParent().AddChild(instance);
            instance.GlobalPosition = GlobalPosition + new Vector2(8 + i * 16, 4);
            instance.Emitting = true;
        }
        QueueFree();
    }

	public void OnCollision(Node node) {
        Player player = node as Player;
        if (player != null && fallDelay.isMax) {
            fallDelay.SetToMin();
            setSprites(0);
        }
	}
}
