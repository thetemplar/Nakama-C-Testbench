package main

type Line struct {
	slope float32
	yint float32
}
 
func CreateLine (a, b PublicMatchState_Vector2Df) Line {
	slope := (b.Y-a.Y) / (b.X-a.X)
	yint := a.Y - slope*a.X
	return Line{slope, yint}
} 
 
func EvalX (l Line, x float32) float32 {
	return l.slope*x + l.yint
}
 
func Intersection (l1, l2 Line) (bool, PublicMatchState_Vector2Df) {
	if l1.slope == l2.slope {
		return false, PublicMatchState_Vector2Df{}
	}
	x := (l2.yint-l1.yint) / (l1.slope-l2.slope)
	y := EvalX(l1, x)
	return true, PublicMatchState_Vector2Df{X:x, Y:y}
}
