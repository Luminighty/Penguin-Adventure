using Godot;
using System;

public class RespawnPoint : Area2D
{
    private static RespawnPoint lastRespawnPoint;
    private AnimationPlayer animation;
    private Position2D respawnPosition;
    
    public override void _Ready() {
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        animation.Play("Disable");
        animation.Seek(0.3f, true);
        respawnPosition = GetNode<Position2D>("RespawnPosition");
    }

    public void _on_body_entered(Node node) {
        Player player = node as Player;
        if (player == null)
            return;
        if (lastRespawnPoint == this)
            return;
        Activate(player);
    }

    public void Activate(Player player) {
        player.spawnPoint.SetSpawnPoint(respawnPosition.GlobalPosition);
        animation.Play("Enable");

        if (lastRespawnPoint != null && lastRespawnPoint.animation != null)
            lastRespawnPoint.animation.Play("Disable");
        lastRespawnPoint = this;
    }

	public override void _Notification(int what) {
        if (what == NotificationPredelete && lastRespawnPoint == this)
            lastRespawnPoint = null;
	}

	public override void _Process(float delta) {
        if (Input.IsKeyPressed((int)KeyList.Q))
            QueueFree();
    }

}
