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

	//OldMatchState       map[int64]PublicMatchState

	ProjectileCounter		int64
	NpcCounter		int64
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
			Player: make(map[string]*PublicMatchState_Player),
			Projectile: make(map[string]*PublicMatchState_Projectile),
			Npc: make(map[string]*PublicMatchState_NPC),
			Stopwatch: []int64{0, 0, 0, 0, 0},
		},
		InternalPlayer: make(map[string]*InternalPlayer),
		//OldMatchState: make(map[int64]PublicMatchState),
	}

	//create map npcs:
	enemy := &PublicMatchState_NPC{
		Id: "npc_" + strconv.FormatInt(state.NpcCounter, 16),
		Type: PublicMatchState_NPC_TRAININGBALL,
		//Position: currentPlayerPublic.Position,
		Position: &PublicMatchState_Vector2Df {
			X: 15,
			Y: 15,
		},
		Health: 100,
	}
	state.PublicMatchState.Npc[enemy.Id] = enemy
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
		state.(*MatchState).PublicMatchState.Player[presence.GetUserId()] = &PublicMatchState_Player{
			Id: presence.GetUserId(),
			Position: &PublicMatchState_Vector2Df {
				X: 0,
				Y: 0,
			},
			Health: 100,
			Power: 100,
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
		state.(*MatchState).PublicMatchState.Player[presence.GetUserId()] = nil
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


func PerformInputs(logger runtime.Logger, state interface{}, message runtime.MatchData) {
	const BaseMovementSpeed = 2
	if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil || state.(*MatchState).PublicMatchState.Player[message.GetUserId()] == nil {
		return
	}
	currentPlayerInternal := state.(*MatchState).InternalPlayer[message.GetUserId()];
	currentPlayerPublic   := state.(*MatchState).PublicMatchState.Player[message.GetUserId()];
	
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
	//clear states
	for _, player := range state.(*MatchState).PublicMatchState.Player { 
		if state.(*MatchState).InternalPlayer[player.Id] == nil {
			continue
		}
		player.Errors = make([]string, 0)
	}

	state.(*MatchState).PublicMatchState.Tick = tick
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
		currentPlayerPublic   := state.(*MatchState).PublicMatchState.Player[message.GetUserId()];

		if message.GetOpCode() == 0 {
			currentPlayerInternal.LastMessage = message
			currentPlayerInternal.LastMessageServerTick = tick
			currentPlayerInternal.MissingCount = 0
			
			PerformInputs(logger, state, currentPlayerInternal.LastMessage)
		} else if message.GetOpCode() == 1 {
			msg := &Client_Cast{}
			if err := proto.Unmarshal(message.GetData(), msg); err != nil {
				logger.Printf("Failed to parse incoming SendPackage Client_Cast:", err)
			}

			if currentPlayerPublic.GlobalCooldown <= 0 {
				fmt.Printf("cast spell: %v\n", msg.Spellname)
				switch msg.Spellname {
				case "fireball":
					fmt.Printf("Target: %v\n", currentPlayerPublic.Target)
					if currentPlayerPublic.Target != "" {
						currentPlayerPublic.GlobalCooldown = 1.5
						proj := &PublicMatchState_Projectile{
							Id: "p_" + strconv.FormatInt(state.(*MatchState).ProjectileCounter, 16),
							Type: PublicMatchState_Projectile_FIREBALL,
							//Position: currentPlayerPublic.Position,
							Position: &PublicMatchState_Vector2Df {
								X: 10,
								Y: 10,
							},
							Rotation: currentPlayerPublic.Rotation,
							Target: currentPlayerPublic.Target,
							Speed: 2,
							Damage: 10,
						}
						state.(*MatchState).PublicMatchState.Projectile[proj.Id] = proj
						fmt.Printf("proj: %v\n\n", proj)

						state.(*MatchState).ProjectileCounter++
					} else {
						currentPlayerPublic.Errors = append(currentPlayerPublic.Errors, "No target!")
					}
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
				PerformInputs(logger, state, player.LastMessage)
			}
		}
	}
	
	//calculate game/npcs/objects
	for _, projectile := range state.(*MatchState).PublicMatchState.Projectile {		
		if projectile == nil {
			continue
		}

		target := state.(*MatchState).PublicMatchState.Player[projectile.Target]
		distance := float32(math.Sqrt(math.Pow(float64(projectile.Position.X - target.Position.X), 2) + math.Pow(float64(projectile.Position.Y - target.Position.Y), 2)))	
		direction := PublicMatchState_Vector2Df {
			X: target.Position.X - projectile.Position.X,
			Y: target.Position.Y - projectile.Position.Y,
		}
		direction.X /= distance
		direction.Y /= distance

		if distance <= projectile.Speed {
			//impact
			fmt.Printf("%v impact\n", projectile.Id)
			target.Health -= projectile.Damage
			delete(state.(*MatchState).PublicMatchState.Projectile, projectile.Id)
			projectile = nil
			continue
		}

		projectile.Position.X = projectile.Position.X + (direction.X * projectile.Speed)
		projectile.Position.Y = projectile.Position.Y + (direction.Y * projectile.Speed)

		fmt.Printf("%v @ %v | %v\n", projectile.Id, projectile.Position.X, projectile.Position.Y)
	}

	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}

	}

	//send new game state (by creating protobuf message)
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
		player.MessageCountThisFrame = 0

		currentPlayerPublic   := state.(*MatchState).PublicMatchState.Player[player.Id];

		fmt.Printf("%v @ %v | %v\n", player.Id, currentPlayerPublic.Position.X, currentPlayerPublic.Position.Y)

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
	
	if state.(*MatchState).EmptyCounter == 50 {
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

