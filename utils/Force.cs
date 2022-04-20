using Godot;

public class Force {

	private Vector2 direction;
	private float weight;
	private float lifetime;

	public bool isAlive {
		get { return lifetime > 0.0f; }
	}

	public Force(Vector2 direction, float strength, float lifetime) {
		this.direction = direction;
		this.weight = strength;
		this.lifetime = lifetime;
	}

	public void Tick(float delta) {
		this.lifetime -= delta;
	}

	public void applyForce(ref Vector2 velocity, float delta = 0) {
		velocity = velocity.LinearInterpolate(direction, weight);
		Tick(delta);
	}
}