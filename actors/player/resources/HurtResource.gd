extends Resource
class_name HurtResource

export var strength: Vector2
export(float, 0, 1) var weight
export(float) var lifeTime

func calculateForceDirection(player, other):
	var delta = player - other
	var forceDirection = Vector2(sign(delta.x) * strength.x, strength.y)
	return forceDirection
