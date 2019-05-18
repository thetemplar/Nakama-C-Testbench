package main

import (
	"math"
)

func (v PublicMatchState_Vector2Df) rotate(degrees float32) PublicMatchState_Vector2Df {
	ca := float32(math.Cos(float64(360 - degrees) * 0.01745329251)); //0.01745329251
	sa := float32(math.Sin(float64(360 - degrees) * 0.01745329251));

	vec := PublicMatchState_Vector2Df {
		X: ca * v.X - sa * v.Y,
		Y: sa * v.X + ca * v.Y,
	}

	return vec
}