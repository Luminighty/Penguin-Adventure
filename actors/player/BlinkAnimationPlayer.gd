extends AnimationPlayer


func _onInvincibilityChanged(isInvincible):
	if isInvincible:
		self.play("Start")
	else:
		self.play("Stop")
