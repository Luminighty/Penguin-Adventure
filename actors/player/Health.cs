using Godot;

public class Health : Node {
	[Export]
	public int maxHp;
	[Export]
	public float iFrameLength;

	public Bound<int> hp;
	public Bound<float> iframes;

	public bool isInvincible {
		get { return iframes.value > 0; }
	}

	public bool isAlive {
		get { return !hp.isMin; }
	}

	public override void _Ready() {
		hp = new Bound<int>(maxHp, 0, maxHp);
		iframes = new Bound<float>(0, 0, iFrameLength);
	}

	public override void _PhysicsProcess(float delta) {
		bool lastInvincible = isInvincible;
		iframes -= delta;
		
		if (lastInvincible != isInvincible)
			EmitSignal(nameof(InvincibilityChanged), false);
	}

	public void Hurt(int damage) {
		hp -= damage;
		iframes.SetToMax();
		EmitSignal(nameof(InvincibilityChanged), true);
		GD.Print(hp);
	}

	public void Heal(int heal) {
		hp += heal;
	}

	public void FullHeal() => hp.SetToMax();

    [Signal]
    public delegate void InvincibilityChanged(bool isInvincible);
}