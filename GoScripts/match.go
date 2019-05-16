package main

import (
	"context"
	"math"
	"database/sql"
	"github.com/heroiclabs/nakama/runtime"
	"github.com/golang/protobuf/proto"
	"fmt"
	"strconv"
)

type MatchState struct {
	PublicMatchState    PublicMatchState
	EmptyCounter        int
	Debug               bool

	InternalPlayer      map[string]*InternalPlayer

	//OldMatchState     map[int64]PublicMatchState

	ProjectileCounter	int64
	NpcCounter			int64
	
	GameDB				*GameDB
}  
  
type InternalPlayer struct {
	Presence                runtime.Presence
	Id                      string

	LastMessage             runtime.MatchData
	LastMessageServerTick   int64
	LastMessageClientTick   int64
	MissingCount			int
	MessageCountThisFrame   int
}

type Match struct{

}


func (m *Match) MatchInit(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {
	logger.Print(" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>> MatchInit <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<")
	for _, entry := range params { 
		logger.Printf("%+v\n", entry)
	}
	/*
	if d, ok := params["debug"]; ok {
		if dv, ok := d.(bool); ok {
			debugFlag = dv
		}
	}*/
	state := &MatchState{
		Debug: false,
		EmptyCounter : 0,
		PublicMatchState : PublicMatchState{
			Interactable: make(map[string]*PublicMatchState_Interactable),
			Projectile: make(map[string]*PublicMatchState_Projectile),
			Stopwatch: []int64{0, 0, 0, 0, 0},
		},
		InternalPlayer: make(map[string]*InternalPlayer),
		//OldMatchState: make(map[int64]PublicMatchState),
	}
	
	//create spellbook
	state.GameDB = init_db()

	//create map npcs:
	enemy := &PublicMatchState_Interactable{
		Id: "npc_" + strconv.FormatInt(state.NpcCounter, 16),
		Type: PublicMatchState_Interactable_NPC,
		CharacterId: 2,
		//Position: currentPlayerPublic.Position,
		Position: &PublicMatchState_Vector2Df {
			X: 15,
			Y: 15,
		},
		CurrentHealth: 100,
		CurrentPower: 0,
		MaxHealth: 100,
		MaxPower: 0,
	}
	state.PublicMatchState.Interactable[enemy.Id] = enemy
	state.NpcCounter++

	if state.Debug {
		logger.Printf("match init, starting with debug: %v", state.Debug)
	}
	tickRate := 10
	label := ""

	return state, tickRate, label
}

func (m *Match) MatchJoinAttempt(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presence runtime.Presence, metadata map[string]string) (interface{}, bool, string) {
	if state.(*MatchState).Debug {
		logger.Printf("match join attempt username %v user_id %v session_id %v node %v with metadata %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId(), metadata)
	}

	return state, true, ""
}

func (m *Match) MatchJoin(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	for _, presence := range presences {
		state.(*MatchState).PublicMatchState.Interactable[presence.GetUserId()] = &PublicMatchState_Interactable{
			Id: presence.GetUserId(),
			Type: PublicMatchState_Interactable_Player,
			Position: &PublicMatchState_Vector2Df {
				X: -10,
				Y: -5,
			},
			CurrentHealth: 100,
			CurrentPower: 100,
			MaxHealth: 100,
			MaxPower: 100,
		}
		
		state.(*MatchState).InternalPlayer[presence.GetUserId()] = &InternalPlayer{
			Id: presence.GetUserId(),
			Presence: presence,
		}
		
		logger.Printf("match join username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
	}

	return state
}

func (m *Match) MatchLeave(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	for _, presence := range presences {		
		state.(*MatchState).PublicMatchState.Interactable[presence.GetUserId()] = nil
		state.(*MatchState).InternalPlayer[presence.GetUserId()] = nil

		logger.Printf("match leave username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
	}

	return state
}

func PublicMatchState_Vector2Df_Rotate(v PublicMatchState_Vector2Df, degrees float32) PublicMatchState_Vector2Df {
	ca := float32(math.Cos(float64(360 - degrees) * 0.01745329251)); //0.01745329251
	sa := float32(math.Sin(float64(360 - degrees) * 0.01745329251));

	vec := PublicMatchState_Vector2Df {
		X: ca * v.X - sa * v.Y,
		Y: sa * v.X + ca * v.Y,
	}

	return vec
}


func PerformInputs(logger runtime.Logger, state interface{}, message runtime.MatchData, tickrate int) {
	BaseMovementSpeed := 20 / float32(tickrate)
	if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil || state.(*MatchState).PublicMatchState.Interactable[message.GetUserId()] == nil {
		return
	}
	currentPlayerInternal := state.(*MatchState).InternalPlayer[message.GetUserId()];
	currentPlayerPublic   := state.(*MatchState).PublicMatchState.Interactable[message.GetUserId()];
	
	msg := &Client_Character{}
	if err := proto.Unmarshal(message.GetData(), msg); err != nil {
		logger.Printf("Failed to parse incoming SendPackage Client_Character:", err)
	}

	//ClientState := state.(*MatchState).OldMatchState[msg.ServerTickPerformingOn]
	add := PublicMatchState_Vector2Df {
		X: msg.XAxis / float32(currentPlayerInternal.MessageCountThisFrame) * BaseMovementSpeed,
		Y: msg.YAxis / float32(currentPlayerInternal.MessageCountThisFrame) * BaseMovementSpeed,
	}
	rotated := PublicMatchState_Vector2Df_Rotate(add, msg.Rotation)
	currentPlayerPublic.Position.X += rotated.X;
	currentPlayerPublic.Position.Y += rotated.Y;
	currentPlayerPublic.Rotation = msg.Rotation;
	
	currentPlayerPublic.Target = msg.Target;

	//simple "wall"
	if currentPlayerPublic.Position.X < -25 { currentPlayerPublic.Position.X = -25 }
	if currentPlayerPublic.Position.X >  25 { currentPlayerPublic.Position.X =  25 }
	if currentPlayerPublic.Position.Y < -25 { currentPlayerPublic.Position.Y = -25 }
	if currentPlayerPublic.Position.Y >  25 { currentPlayerPublic.Position.Y =  25 }
	

	currentPlayerPublic.LastProcessedClientTick = msg.ClientTick
}

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	if state.(*MatchState).Debug {
		logger.Printf("match loop match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match loop match_id %v message count %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), len(messages))
	}
	fmt.Printf(" _ _ _ _ _ new tick %v _ _ _ _ _\n", tick)
	state.(*MatchState).PublicMatchState.Tick = tick	
	tickrate := ctx.Value(runtime.RUNTIME_CTX_MATCH_TICK_RATE).(int);

	//clear states
	for _, player := range state.(*MatchState).PublicMatchState.Interactable { 
		if player == nil || player.Type != PublicMatchState_Interactable_Player {
			continue
		}
		player.GlobalCooldown -= float32(1)/float32(ctx.Value(runtime.RUNTIME_CTX_MATCH_TICK_RATE).(int));
		player.Errors = make([]string, 0)
	}

	//get new input-counts
	for _, message := range messages { 
		if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil {
			continue
		}
		if(message.GetOpCode() == 0) {
			state.(*MatchState).InternalPlayer[message.GetUserId()].MessageCountThisFrame++
		}
	}
	//get new inputs
	for _, message := range messages { 
		//logger.Printf("message from %v with opcode %v", message.GetUserId(), message.GetOpCode())
		//entry.UserID, entry.SessionId, entry.Username, entry.Node, entry.OpCode, entry.Data, entry.ReceiveTime
		if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil {
			continue
		}
		currentPlayerInternal := state.(*MatchState).InternalPlayer[message.GetUserId()];
		currentPlayerPublic   := state.(*MatchState).PublicMatchState.Interactable[message.GetUserId()];

		if message.GetOpCode() == 0 {
			currentPlayerInternal.LastMessage = message
			currentPlayerInternal.LastMessageServerTick = tick
			currentPlayerInternal.MissingCount = 0
			
			PerformInputs(logger, state, currentPlayerInternal.LastMessage, tickrate)
		} else if message.GetOpCode() == 1 {
			msg := &Client_Cast{}
			if err := proto.Unmarshal(message.GetData(), msg); err != nil {
				logger.Printf("Failed to parse incoming SendPackage Client_Cast:", err)
			}

			if currentPlayerPublic.GlobalCooldown <= 0 {
				targetId := ""
				distance := float32(0)
				if state.(*MatchState).GameDB.Spells[msg.SpellId].Target != GameDB_Interrupts_None {
					if currentPlayerPublic.Target != "" {	
						targetId = currentPlayerPublic.Target
						target := state.(*MatchState).PublicMatchState.Interactable[targetId]
						distance = float32(math.Sqrt(math.Pow(float64(currentPlayerPublic.Position.X - target.Position.X), 2) + math.Pow(float64(currentPlayerPublic.Position.Y - target.Position.Y), 2)))	
					} else {
						currentPlayerPublic.Errors = append(currentPlayerPublic.Errors, "No Target!")
						continue
					}	
				}		
				
				if distance <= state.(*MatchState).GameDB.Spells[msg.SpellId].Range {
					fmt.Printf("cast spell: %v\n", msg.SpellId)
					currentPlayerPublic.GlobalCooldown = state.(*MatchState).GameDB.Spells[msg.SpellId].GlobalCooldown
					proj := &PublicMatchState_Projectile{
						Id: "p_" + strconv.FormatInt(state.(*MatchState).ProjectileCounter, 16),
						SpellId: msg.SpellId,
						Position: &PublicMatchState_Vector2Df {
							X: currentPlayerPublic.Position.X,
							Y: currentPlayerPublic.Position.Y,
						},
						Rotation: currentPlayerPublic.Rotation,
						CreatedAtTick: tick,
						Target: targetId,
						Speed: state.(*MatchState).GameDB.Spells[msg.SpellId].Speed,
					}
					state.(*MatchState).PublicMatchState.Projectile[proj.Id] = proj
					state.(*MatchState).ProjectileCounter++					
				} else {
					currentPlayerPublic.Errors = append(currentPlayerPublic.Errors, "Out of Range!")
				}	
			} else {
				currentPlayerPublic.Errors = append(currentPlayerPublic.Errors, "Cannot do that now!")
			}
		}
	}

	//did a player not send an package? then re-do his last
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
		if player.LastMessageServerTick != tick {
			player.MissingCount++
			if player.MissingCount > 1 && player.LastMessage != nil {
				player.MessageCountThisFrame = 1
				logger.Printf("2nd missing Package from player %v in a row, inserting last known package.", player.Id)
				PerformInputs(logger, state, player.LastMessage, tickrate)
			}
		}
	}
	
	//calculate game/npcs/objects
	for _, projectile := range state.(*MatchState).PublicMatchState.Projectile {		
		if projectile == nil || projectile.CreatedAtTick == tick {
			continue
		}
		fmt.Printf("calc proj %v\n", projectile)
		projectile.Run(state.(*MatchState), projectile, tickrate)		
	}

	for _, npc := range state.(*MatchState).PublicMatchState.Interactable {		
		if npc == nil || npc.Type == PublicMatchState_Interactable_Player {
			continue
		}

	}

	//send new game state (by creating protobuf message)
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
		player.MessageCountThisFrame = 0

		currentPlayerPublic := state.(*MatchState).PublicMatchState.Interactable[player.Id];

		fmt.Printf("%v @ %v | %v  GCD: %v\n", player.Id, currentPlayerPublic.Position.X, currentPlayerPublic.Position.Y, currentPlayerPublic.GlobalCooldown)

		out, err := proto.Marshal(&state.(*MatchState).PublicMatchState)
		if err != nil {
				logger.Printf("Failed to encode PublicMatchState:", err)
		}
		dispatcher.BroadcastMessage(1, out, []runtime.Presence { player.Presence }, nil)
	}	
	
	//save for history
	//historyCopy := state.(*MatchState).PublicMatchState
	//state.(*MatchState).OldMatchState[tick] = historyCopy



	//end if no ones sending smth (all dc'ed)
	if len(messages) == 0 {
		state.(*MatchState).EmptyCounter = state.(*MatchState).EmptyCounter + 1;
	} else {
		state.(*MatchState).EmptyCounter = 0
	}
	
	if state.(*MatchState).EmptyCounter == 20 {
		return nil
	}
	
	return state
}

func (m *Match) MatchTerminate(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, graceSeconds int) interface{} {
	if state.(*MatchState).Debug {
		logger.Printf("match terminate match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match terminate match_id %v grace seconds %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), graceSeconds)
	}

	return state
}

