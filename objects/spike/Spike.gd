extends Area2D

export var damage = 1

func _on_Spike_body_entered(body):
	if funcref(body, "Hurt"):
		body.Hurt(self, damage)
