using Godot;
using System;

public class Shooting : Position2D
{

    [Export]
    public Vector2 bulletVelocity;
    [Export]
    public float lifeTime;
    [Export]
    public float cooldown;
    public Bound<float> cd;

    private PackedScene bullet;
    private bool isCharging = false;
    private float charge = 0.0f;
    private int isRight = 1;

    [Export]
    public bool isShootingEnabled;
    [Export]
    public bool isChargedShotEnabled;

    private bool canShoot {
        get { return isShootingEnabled && cd.isMin; }
    }
    private bool canCharge {
        get { return isChargedShotEnabled; }
    }

    [Signal]
    delegate void Shoot();

    public override void _Ready() {
        bullet = GD.Load<PackedScene>("res://actors/player/bullet/Bullet.tscn");
        cd = new Bound<float>(0, 0, cooldown); 
    }

    public override void _Input(InputEvent inputEvent) {
        if (inputEvent.IsActionPressed(Keys.Shoot))
            isCharging = canCharge;
        if (inputEvent.IsActionReleased(Keys.Shoot))
            ShootProjectile();
    }

    private void ShootProjectile() {
        if (!canShoot) {
            Reset();
            return;
        }
        charge = Mathf.Min(charge, 1.0f);
        cd.SetToMax();

        Bullet instance = bullet.Instance<Bullet>();
        instance.kind = charge < 0.9f ? Bullet.Kind.Small : Bullet.Kind.Big;
        instance.lifeTime = lifeTime;
        instance.velocity = bulletVelocity * isRight;
        instance.Position = GlobalPosition;
        GetNode("/root/").AddChild(instance);
        EmitSignal(nameof(Shoot));

        Reset();
    }

    private void Reset() {
        isCharging = false;
        charge = 0;
    }
    

    public override void _Process(float delta) {
        if (isCharging && cd.isMin)
            charge += delta;
        cd.value -= delta;
    }

    public void _on_FacingChanged(bool isRight) {
        this.isRight = isRight ? 1 : -1;
        var pos = Position;
        pos.x = Math.Abs(pos.x) * this.isRight;
        Position = pos;
    }

    public void _onAbilityGained(Ability ability) {
        isChargedShotEnabled |= ability == Ability.ChargeShot;
        isShootingEnabled |= ability == Ability.Shoot;
    }
}
