using Godot;
using System;

public class Actor : KinematicBody2D
{
	public static float MAXFALLSPEED = 500;
	public static Vector2 GRAVITY = new Vector2(0, 800);
	public Vector2 gravity = GRAVITY;
	public Vector2 velocity;

	public override void _PhysicsProcess(float delta) {
		velocity += gravity * delta;
		_computeVelocity(delta);

		velocity.y = Math.Min(velocity.y, MAXFALLSPEED);
		velocity = MoveAndSlide(velocity, Vector2.Up);

		for(int i = 0; i < GetSlideCount(); i++) {
			KinematicCollision2D collision = GetSlideCollision(i);
			ICollisionDetector detector = collision.Collider as ICollisionDetector;
			if (detector != null)
				detector.OnCollision(this);
		}
		
		_animate(delta);
		_postComputeVelocity(delta);
	}

	public virtual void _postComputeVelocity(float delta) {}
	public virtual void _animate(float delta) {}
	public virtual void _computeVelocity(float delta) {}
}
