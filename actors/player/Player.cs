using Godot;
using System.Collections.Generic;

public class Player : Actor
{
    Sprite sprite;
    RayCast2D groundCast;
    RayCast2D groundCast2;
    AnimationTree animation;
    AnimationNodeStateMachinePlayback animationStateMachine;
    AnimationPlayer blinkAnimationPlayer;

    bool isOnGround = false;
    
    public Movement movement;
    public Health health;
    public SpawnPoint spawnPoint;

    public Dictionary<KeyType, int> keys;
    public Camera2D camera;

    public HashSet<Ability> abilities;

    [Export]
    public Resource hurtResource;

    [Signal]
    delegate void GroundChanged(bool isOnGround);
    [Signal]
    delegate void FacingChanged(bool isRight);
    [Signal]
    delegate void AbilityGained(Ability ability);

    public override void _Ready() {
        initKeys();
        spawnPoint = new SpawnPoint(this);
        movement = GetNode<Movement>("Movement");
        health = GetNode<Health>("Health");

        groundCast = GetNode<RayCast2D>("GroundCast");
        groundCast2 = GetNode<RayCast2D>("GroundCast2");
        camera = GetNode<Camera2D>("Camera2D");
        sprite = GetNode<Sprite>("Sprite");
        animation = GetNode<AnimationTree>("AnimationTree");
        animationStateMachine = animation.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
        blinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
        abilities = new HashSet<Ability>();
    }

    public void addAbility(Ability ability) {
        abilities.Add(ability);
        EmitSignal(nameof(AbilityGained), ability);
    }

    private void initKeys() {
        keys = new Dictionary<KeyType, int>();
        foreach(var key in System.Enum.GetValues(typeof(KeyType)))
            keys[(KeyType)key] = 0;
    }

	public override void _computeVelocity(float deltaTime) {
        movement.applyVelocity(ref velocity, deltaTime);
        computeGround(deltaTime);
        oneWay();
    }

    public void Hurt(Node body, int dmg) {
        if (health.isInvincible)
            return;
        health.Hurt(dmg);
        

        if (!health.isAlive) {
            Die();
            return;
        }
        
        // If player didn't die

        Node2D other = body as Node2D;
        if (other != null) {
            Vector2 direction = (Vector2)hurtResource.Call("calculateForceDirection", Position, other.Position);
            float weight = (float)hurtResource.Get("weight");
            float lifeTime = (float)hurtResource.Get("lifeTime");
            movement.addForce(direction, weight, lifeTime);
        }
    }

    public void Die() {
        spawnPoint.Respawn();
        health.FullHeal();
    }

    private void oneWay() {
        var value = !Input.IsActionPressed("ui_down");
        SetCollisionMaskBit(4, value);
    }

    private new void SetCollisionMaskBit(int bit, bool value) {
        base.SetCollisionMaskBit(bit, value);
        groundCast.SetCollisionMaskBit(bit, value);
        groundCast2.SetCollisionMaskBit(bit, value);
    }

    public override void _animate(float deltaTime) {
        int rotation = sprite.FlipH ? -1 : 1;
        if (rotation * Mathf.Stepify(velocity.x, 0.01f) < 0) {
            sprite.FlipH = velocity.x < 0;
            EmitSignal(nameof(FacingChanged), velocity.x > 0);
        }

        animation.Set("parameters/Movement/blend_position", velocity.x);
        animation.Set("parameters/Jump/blend_position", velocity.y);

        animation.Set("parameters/conditions/is_jumping", !isOnGround);
        animation.Set("parameters/conditions/is_walking", isOnGround);
    }

    private void computeGround(float deltaTime) {
        var onGround = groundCast.IsColliding() || groundCast2.IsColliding();
        if (onGround && velocity.y < 0)
            return;
        if (onGround != isOnGround) {
            isOnGround = onGround;
            EmitSignal(nameof(GroundChanged), onGround);
        }
    }

    public void _onPlayAnimation(string anim) {
        animationStateMachine.Start(anim);
    }
}
