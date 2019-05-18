package main

import (
	"fmt"
	"strconv"
	"math"
)

func (p PublicMatchState_Interactable) getInternalPlayer(state *MatchState) (*InternalPlayer) {
	return state.InternalPlayer[p.Id];
}


func (p PublicMatchState_Interactable) startCast(state *MatchState, spellId int64) {
	currentPlayerInternal := p.getInternalPlayer(state)

	if p.GlobalCooldown <= 0 && currentPlayerInternal.CastingSpellId <= 0 {
		targetId := ""
		distance := float32(0)
		if state.GameDB.Spells[spellId].Target != GameDB_Spell_Target_None {
			if p.Target != "" {	
				targetId = p.Target
				target := state.PublicMatchState.Interactable[targetId]
				distance = float32(math.Sqrt(math.Pow(float64(p.Position.X - target.Position.X), 2) + math.Pow(float64(p.Position.Y - target.Position.Y), 2)))	
				if distance <= state.GameDB.Spells[spellId].Range {
					if state.GameDB.Spells[spellId].CastTime > 0 {
						currentPlayerInternal.CastingSpellId = spellId
						currentPlayerInternal.CastingTickStarted = state.PublicMatchState.Tick
						if state.GameDB.Spells[spellId].Target != GameDB_Spell_Target_None {
							currentPlayerInternal.CastingTargeted = targetId
						} else {
							currentPlayerInternal.CastingTargeted = ""
						}
					} else {
						p.finishCast(state, spellId, targetId)
					}
				} else {
					clEntry := &PublicMatchState_CombatLogEntry {
						Timestamp: state.PublicMatchState.Tick,
						SourceId: p.Id,
						SourceSpellEffectId: &PublicMatchState_CombatLogEntry_SourceSpellId{spellId},
						Source: PublicMatchState_CombatLogEntry_Spell,
						Type: &PublicMatchState_CombatLogEntry_Cast{ &PublicMatchState_CombatLogEntry_CombatLogEntry_Cast{
							Event: PublicMatchState_CombatLogEntry_CombatLogEntry_Cast_Failed,
							FailedMessage: "Out of Range!",
						}},
					}
					state.PublicMatchState.Combatlog = append(state.PublicMatchState.Combatlog, clEntry)
				}	
			} else {
				clEntry := &PublicMatchState_CombatLogEntry {
					Timestamp: state.PublicMatchState.Tick,
					SourceId: p.Id,
					SourceSpellEffectId: &PublicMatchState_CombatLogEntry_SourceSpellId{spellId},
					Source: PublicMatchState_CombatLogEntry_Spell,
					Type: &PublicMatchState_CombatLogEntry_Cast{ &PublicMatchState_CombatLogEntry_CombatLogEntry_Cast{
						Event: PublicMatchState_CombatLogEntry_CombatLogEntry_Cast_Failed,
						FailedMessage: "No Target Selected",
					}},
				}
				state.PublicMatchState.Combatlog = append(state.PublicMatchState.Combatlog, clEntry)
			}	
		}	
	} else {
		clEntry := &PublicMatchState_CombatLogEntry {
			Timestamp: state.PublicMatchState.Tick,
			SourceId: p.Id,
			SourceSpellEffectId: &PublicMatchState_CombatLogEntry_SourceSpellId{spellId},
			Source: PublicMatchState_CombatLogEntry_Spell,
			Type: &PublicMatchState_CombatLogEntry_Cast{ &PublicMatchState_CombatLogEntry_CombatLogEntry_Cast{
				Event: PublicMatchState_CombatLogEntry_CombatLogEntry_Cast_Failed,
				FailedMessage: "Cannot do that now!",
			}},
		}
		state.PublicMatchState.Combatlog = append(state.PublicMatchState.Combatlog, clEntry)
	}
}


func (p PublicMatchState_Interactable) finishCast(state *MatchState, spellId int64, targetId string) {
	fmt.Printf("cast spell: %v\n", spellId)
	p.GlobalCooldown = state.GameDB.Spells[spellId].GlobalCooldown
	proj := &PublicMatchState_Projectile{
		Id: "p_" + strconv.FormatInt(state.ProjectileCounter, 16),
		SpellId: spellId,
		Position: &PublicMatchState_Vector2Df {
			X: p.Position.X,
			Y: p.Position.Y,
		},
		Rotation: p.Rotation,
		CreatedAtTick: state.PublicMatchState.Tick,
		Target: targetId,
		Speed: state.GameDB.Spells[spellId].Speed,
	}
	state.PublicMatchState.Projectile[proj.Id] = proj
	state.ProjectileCounter++			
}

func (p PublicMatchState_Interactable) recalcStats(state *MatchState) {
	p.getInternalPlayer(state).StatModifiers = PlayerStats {}
	for _, aura := range p.Auras {
		effect := state.GameDB.Effects[aura.EffectId]
		
		switch effect.Type.(type) {
		case *GameDB_Effect_Apply_Aura_Mod:
			if effect.Type.(*GameDB_Effect_Apply_Aura_Mod).Stat == GameDB_Stat_Speed && effect.Type.(*GameDB_Effect_Apply_Aura_Mod).Value > p.getInternalPlayer(state).StatModifiers.MovementSpeed {
				p.getInternalPlayer(state).StatModifiers.MovementSpeed = effect.Type.(*GameDB_Effect_Apply_Aura_Mod).Value
			}
		}
	}
}