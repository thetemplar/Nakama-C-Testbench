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
		projectile.Hit(state, target, projectile, spell)
		delete(state.PublicMatchState.Projectile, projectile.Id)
		projectile = nil
		return
	}

	projectile.Position.X = projectile.Position.X + (direction.X * (projectile.Speed / float32(tickrate)))
	projectile.Position.Y = projectile.Position.Y + (direction.Y * (projectile.Speed / float32(tickrate)))

	fmt.Printf("%v @ %v | %v\n", projectile.Id, projectile.Position.X, projectile.Position.Y)
}


func (p PublicMatchState_Projectile) Hit(state *MatchState, target *PublicMatchState_Interactable, projectile *PublicMatchState_Projectile, spell *GameDB_Spells) {
	for _, aura_spell := range spell.ApplySpell { 
		fmt.Printf("Apply Spell on Hit %v\n", aura_spell)
		aura := &PublicMatchState_Aura{
			CreatedAtTick: state.PublicMatchState.Tick,
			SpellId: aura_spell.Id,
			Creator: projectile.Creator,
		}
		target.Auras = append(target.Auras, aura)

		clEntry := &PublicMatchState_CombatLogEntry {
			Timestamp: state.PublicMatchState.Tick,
			SourceId: aura.Creator,
			DestinationId: target.Id,
			SourceSpellId: aura_spell.Id,
			Source: PublicMatchState_CombatLogEntry_Spell,
			Type: &PublicMatchState_CombatLogEntry_Aura{ &PublicMatchState_CombatLogEntry_CombatLogEntry_Aura{
				Event: PublicMatchState_CombatLogEntry_CombatLogEntry_Aura_Applied,
			}},
		}
		state.PublicMatchState.Combatlog = append(state.PublicMatchState.Combatlog, clEntry)
	}

	dmg := randomInt(spell.SpellDamageMin, spell.SpellDamageMax);
	target.CurrentHealth -= dmg;

	clEntry := &PublicMatchState_CombatLogEntry {
		Timestamp: state.PublicMatchState.Tick,
		SourceId: projectile.Creator,
		DestinationId: target.Id,
		SourceSpellId: spell.Id,
		Source: PublicMatchState_CombatLogEntry_Spell,
		Type: &PublicMatchState_CombatLogEntry_Damage{ &PublicMatchState_CombatLogEntry_CombatLogEntry_Damage{
			Amount: dmg,
		}},
	}
	state.PublicMatchState.Combatlog = append(state.PublicMatchState.Combatlog, clEntry)
}