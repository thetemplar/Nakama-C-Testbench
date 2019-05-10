package main

import (
	"context"
	"math"
	"database/sql"
	"github.com/heroiclabs/nakama/runtime"
	"github.com/golang/protobuf/proto"
	"time"
)

type MatchState struct {
	PublicMatchState    PublicMatchState
	EmptyCounter        int
	Debug               bool

	InternalPlayer      map[string]*InternalPlayer

	//OldMatchState       map[int64]PublicMatchState
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
			Stopwatch: []int64{0, 0, 0, 0, 0},
		},
		InternalPlayer: make(map[string]*InternalPlayer),
		//OldMatchState: make(map[int64]PublicMatchState),
	}
	

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
	
	msg := &SendPackage{}
	if err := proto.Unmarshal(message.GetData(), msg); err != nil {
		logger.Printf("Failed to parse incoming SendPackage:", err)
	}

	if message.GetOpCode() == 0 {
		//ClientState := state.(*MatchState).OldMatchState[msg.ServerTickPerformingOn]
		add := PublicMatchState_Vector2Df {
			X: msg.XAxis / float32(currentPlayerInternal.MessageCountThisFrame) * BaseMovementSpeed,
			Y: msg.YAxis / float32(currentPlayerInternal.MessageCountThisFrame) * BaseMovementSpeed,
		}
		rotated := PublicMatchState_Vector2Df_Rotate(add, msg.Rotation)
		currentPlayerPublic.Position.X += rotated.X;
		currentPlayerPublic.Position.Y += rotated.Y;
		currentPlayerPublic.Rotation = msg.Rotation;

		//simple "wall"
		if currentPlayerPublic.Position.X < -25 { currentPlayerPublic.Position.X = -25 }
		if currentPlayerPublic.Position.X >  25 { currentPlayerPublic.Position.X =  25 }
		if currentPlayerPublic.Position.Y < -25 { currentPlayerPublic.Position.Y = -25 }
		if currentPlayerPublic.Position.Y >  25 { currentPlayerPublic.Position.Y =  25 }
	}

	currentPlayerPublic.LastProcessedClientTick = msg.ClientTick
}

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	start := time.Now()
	if state.(*MatchState).Debug {
		logger.Printf("match loop match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match loop match_id %v message count %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), len(messages))
	}

	state.(*MatchState).PublicMatchState.Tick = tick
	//get new inputs
	for _, message := range messages { 
		if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil {
			continue
		}
		state.(*MatchState).InternalPlayer[message.GetUserId()].MessageCountThisFrame++
	}
	state.(*MatchState).PublicMatchState.Stopwatch[0] = int64(time.Since(start))
	//get new inputs
	for _, message := range messages { 
		//logger.Printf("message from %v with opcode %v", message.GetUserId(), message.GetOpCode())
		//entry.UserID, entry.SessionId, entry.Username, entry.Node, entry.OpCode, entry.Data, entry.ReceiveTime
		if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil {
			continue
		}

		state.(*MatchState).InternalPlayer[message.GetUserId()].LastMessage = message
		state.(*MatchState).InternalPlayer[message.GetUserId()].LastMessageServerTick = tick
		state.(*MatchState).InternalPlayer[message.GetUserId()].MissingCount = 0
		
		PerformInputs(logger, state, state.(*MatchState).InternalPlayer[message.GetUserId()].LastMessage)
	}

	state.(*MatchState).PublicMatchState.Stopwatch[1] = int64(time.Since(start))
	//did a player not send an package? then re-do his last
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
		if player.LastMessageServerTick != tick {
			player.MissingCount++
			if player.MissingCount > 1 && player.LastMessage != nil {
				logger.Printf("2nd missing Package from player %v in a row, inserting last known package.", player.Id)
				PerformInputs(logger, state, player.LastMessage)
			}
		}
	}
	
	state.(*MatchState).PublicMatchState.Stopwatch[2] = int64(time.Since(start))
	//calculate game/npcs/objects
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
	}

	state.(*MatchState).PublicMatchState.Stopwatch[3] = int64(time.Since(start))
	//send new game state (by creating protobuf message)
	for _, player := range state.(*MatchState).InternalPlayer {		
		if player == nil {
			continue
		}
		player.MessageCountThisFrame = 0
		

		out, err := proto.Marshal(&state.(*MatchState).PublicMatchState)
		if err != nil {
				logger.Printf("Failed to encode PublicMatchState:", err)
		}
		dispatcher.BroadcastMessage(1, out, []runtime.Presence { player.Presence }, nil)
	}	
	state.(*MatchState).PublicMatchState.Stopwatch[4] = int64(time.Since(start))
	
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

