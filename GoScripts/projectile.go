package main

import (
	"math"
	"fmt"
	"math/rand"
)

func randomInt(min, max int32) int32 {
    return min + rand.Int31n(max-min)
}

func (p PublicMatchState_Projectile) Run(state *MatchState, projectile *PublicMatchState_Projectile, tickrate int) {
	target := state.PublicMatchState.Interactable[projectile.Target]					
	distance := float32(math.Sqrt(math.Pow(float64(projectile.Position.X - target.Position.X), 2) + math.Pow(float64(projectile.Position.Y - target.Position.Y), 2)))	
	direction := PublicMatchState_Vector2Df {
		X: target.Position.X - projectile.Position.X,
		Y: target.Position.Y - projectile.Position.Y,
	}
	direction.X /= distance
	direction.Y /= distance

	if distance <= (projectile.Speed / float32(tickrate)) {
		//impact
		fmt.Printf("%v impact\n", projectile.Id)
		spell := state.GameDB.Spells[projectile.SpellId]	
		projectile.Hit(target, projectile, spell)
		delete(state.PublicMatchState.Projectile, projectile.Id)
		projectile = nil
		return
	}

	projectile.Position.X = projectile.Position.X + (direction.X * (projectile.Speed / float32(tickrate)))
	projectile.Position.Y = projectile.Position.Y + (direction.Y * (projectile.Speed / float32(tickrate)))

	fmt.Printf("%v @ %v | %v\n", projectile.Id, projectile.Position.X, projectile.Position.Y)
}


func (p PublicMatchState_Projectile) Hit(target *PublicMatchState_Interactable, projectile *PublicMatchState_Projectile, spell *GameDB_Spells) {
	target.CurrentHealth -= randomInt(spell.SpellDamageMin, spell.SpellDamageMax)
}