extends Area2D

var target

func _ready():
	target = $Target

func _on_TeleportField_body_entered(body):
	body.position += target.position
