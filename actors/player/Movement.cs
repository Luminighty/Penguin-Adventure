using Godot;
using System;
using System.Collections.Generic;

public class Movement : Node
{
    [Export]
    public int speed = 150;
    [Export]
    public int accSpeed = 10;
    [Export]
    public int brakeSpeed = 15;

    [Export]
    public int maxJumps = 2;
    [Export]
    public int jumpSize = 300;
    [Export]
    public float FallMod = 1.05f;

    [Export]
    public float LowJumpMod = 0.5f;

    public Bound<int> jumps;

    [Export]
    public bool isJumpEnabled = false;

    private List<Force> forces;
    
    public float MoveDirection {
        get {
            return Input.GetActionStrength(Keys.MoveRight) - Input.GetActionStrength(Keys.MoveLeft);
        }
    }
    private bool canJump {
        get {
            return isJumpEnabled && jumps != null && !jumps.isMax;
        }
    }

    public override void _Ready() {
        jumps = new Bound<int>(0, maxJumps);
        forces = new List<Force>();
    }

    public void applyVelocity(ref Vector2 velocity, float deltaTime) {
        move(ref velocity);
        jump(ref velocity);
        applyForces(ref velocity, deltaTime);
    }


    private void move(ref Vector2 velocity) {
        float targetSpeed = MoveDirection * speed;
        if (targetSpeed * velocity.x < 0)
            velocity.x = 0;
        float delta = (Math.Abs(targetSpeed) > Math.Abs(velocity.x)) ? accSpeed : brakeSpeed;
        velocity = velocity.MoveToward(new Vector2(targetSpeed, velocity.y), delta);
    }


    private void jump(ref Vector2 velocity) {
        if (canJump && Input.IsActionJustPressed(Keys.Jump)) {
            velocity.y = -jumpSize;
            jumps += 1;
        }

        if (velocity.y > 0) {
		    velocity.y *= FallMod;
        } else if (!Input.IsActionPressed(Keys.Jump)) {
            velocity.y *= LowJumpMod;
        }
    }


    private void applyForces(ref Vector2 velocity, float deltaTime) {
        foreach (var force in forces)
            force.applyForce(ref velocity, deltaTime);
        forces.RemoveAll((force) => !force.isAlive);
    }

    public void addForce(Vector2 direction, float weight, float lifetime) {
        forces.Add(new Force(direction, weight, lifetime));
    }
    public void addForce(Force force) {
        forces.Add(force);
    }


    public void _onGroundChanged(bool onGround) {
        if (onGround)
            jumps.value = 0;
    }

    public void _onAbilityGained(Ability ability) {
        isJumpEnabled |= ability == Ability.Jump;
    }
}
