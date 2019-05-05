package main

import (
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama/runtime"
    "github.com/golang/protobuf/proto"
)

type InternalPlayer struct {
	Public   *PublicMatchState_Player
	Presence runtime.Presence
	Id       string
}


type MatchState struct {
	PublicMatchState PublicMatchState
	EmptyCounter     int
	Debug            bool
	
	InternalPlayer   map[string]*InternalPlayer
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
		},
		InternalPlayer: make(map[string]*InternalPlayer),
	}
	

	if state.Debug {
		logger.Printf("match init, starting with debug: %v", state.Debug)
	}
	tickRate := 5
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
			Public: state.(*MatchState).PublicMatchState.Player[presence.GetUserId()],
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

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	
	if state.(*MatchState).Debug {
		logger.Printf("match loop match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match loop match_id %v message count %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), len(messages))
	}
	
	//get new inputs
	for _, message := range messages { 
		//logger.Printf("message from %v with opcode %v", message.GetUserId(), message.GetOpCode())
		//entry.UserID, entry.SessionId, entry.Username, entry.Node, entry.OpCode, entry.Data, entry.ReceiveTime
		msg := &SendPackage{}
		if err := proto.Unmarshal(message.GetData(), msg); err != nil {
			logger.Printf("Failed to parse SendPackage:", err)
		}

		state.(*MatchState).PublicMatchState.Tick = tick

		if state.(*MatchState).InternalPlayer[message.GetUserId()] == nil {
			continue
		}
		currentPlayer := state.(*MatchState).InternalPlayer[message.GetUserId()].Public;

		currentPlayer.LastReceivedTick = msg.Tick

		if message.GetOpCode() == 0 { 		
			currentPlayer.Position.X += msg.XAxis * 0.2;
			currentPlayer.Position.Y += msg.YAxis * 0.2;

			//simple "wall"
			if currentPlayer.Position.X < -25 { currentPlayer.Position.X = -25 }
			if currentPlayer.Position.X >  25 { currentPlayer.Position.X =  25 }
			if currentPlayer.Position.Y < -25 { currentPlayer.Position.Y = -25 }
			if currentPlayer.Position.Y >  25 { currentPlayer.Position.Y =  25 }
		}
	}
	
	//calculate game
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

		out, err := proto.Marshal(&state.(*MatchState).PublicMatchState)
		if err != nil {
				logger.Printf("Failed to encode PublicMatchState:", err)
		}
		dispatcher.BroadcastMessage(1, out, []runtime.Presence { player.Presence }, nil)
	}
	
	
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

