package main

import (
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama/runtime"
    "github.com/golang/protobuf/proto"
)

type CommandKeys struct {
	CCW bool
	CW bool
}

type InternalPlayer struct {
	Public *PublicMatchState_Player
	Id       string
	CommandKeys CommandKeys
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
		Debug: true,
		EmptyCounter : 0,
		PublicMatchState : PublicMatchState{
			Ball :   &PublicMatchState_Ball{ 
				PosX : 0,
				PosY : 0,
				DirX : 0,
				DirY : 0,
			},
			Player: make(map[string]*PublicMatchState_Player),
		},
		InternalPlayer: make(map[string]*InternalPlayer),
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
		logger.Printf(">match join username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
			
		state.(*MatchState).PublicMatchState.Player[presence.GetUserId()] = &PublicMatchState_Player{
			Id: presence.GetUserId(), 
		}
		
		state.(*MatchState).InternalPlayer[presence.GetUserId()] = &InternalPlayer{
			Public: state.(*MatchState).PublicMatchState.Player[presence.GetUserId()],
			Id: presence.GetUserId(),
			CommandKeys: CommandKeys {
				CCW: false,
				CW: false,
			},
		}
	}

	logger.Printf("match join %v", state.(*MatchState))
	return state
}

func (m *Match) MatchLeave(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	if state.(*MatchState).Debug {
		for _, presence := range presences {
			logger.Printf("match leave username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
		}
	}

	return state
}

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	if state.(*MatchState).Debug {
		logger.Printf("match loop match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match loop match_id %v message count %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), len(messages))
	}
	/*
	for _, message := range messages { 
		//entry.UserID, entry.SessionId, entry.Username, entry.Node, entry.OpCode, entry.Data, entry.ReceiveTime
		if message.GetOpCode() == 1 {
			state.(*MatchState).player[message.GetUserId()].commandKeys.CCW = true
		}
		if message.GetOpCode() == 2 {
			state.(*MatchState).player[message.GetUserId()].commandKeys.CCW = false
		}
		if message.GetOpCode() == 3	{
			state.(*MatchState).player[message.GetUserId()].commandKeys.CW = true
		}
		if message.GetOpCode() == 4	{
			state.(*MatchState).player[message.GetUserId()].commandKeys.CW = false
		}
	}*/
	
	//calculate game
	for _, player := range state.(*MatchState).InternalPlayer {
		if player.CommandKeys.CCW {
			player.Public.Position += 2
		}
		if player.CommandKeys.CW {
			player.Public.Position -= 2
		}
	}
	
	//create protobuf message
	out, err := proto.Marshal(&state.(*MatchState).PublicMatchState)
	if err != nil {
			logger.Printf("Failed to encode PublicMatchState:", err)
	}
	dispatcher.BroadcastMessage(1, out, nil, nil)
	
	
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

