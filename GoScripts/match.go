package main

import (
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama/runtime"
)

type Player struct {
	id       string
	position int
}

type Ball struct {
	pos_x int
	pos_y int
	dir_x int
	dir_y int
}

type MatchState struct {
	player       []Player
	ball         Ball
	emptyCounter int
	debug        bool
}

type Match struct{

}


func (m *Match) MatchInit(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, params map[string]interface{}) (interface{}, int, string) {
	logger.Print(" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>> MatchInit <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<")
	var debugFlag bool
	for _, entry := range params { 
		logger.Printf("%+v\n", entry)
	}
	
	if d, ok := params["debug"]; ok {
		if dv, ok := d.(bool); ok {
			debugFlag = dv
		}
	}
	state := &MatchState{
		debug: debugFlag,
		emptyCounter : 0,
		player : []Player{ Player{ id: "", position : 0, }, Player{ id: "", position : 180, }, },
		ball :   Ball{ pos_x : 0, pos_y : 0, dir_x : 0, dir_y : 0, },
	}
	

	if state.debug {
		logger.Printf("match init, starting with debug: %v", state.debug)
	}
	tickRate := 10
	label := ""

	return state, tickRate, label
}

func (m *Match) MatchJoinAttempt(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presence runtime.Presence, metadata map[string]string) (interface{}, bool, string) {
	if state.(*MatchState).debug {
		logger.Printf("match join attempt username %v user_id %v session_id %v node %v with metadata %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId(), metadata)
	}

	return state, true, ""
}

func (m *Match) MatchJoin(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	if state.(*MatchState).debug {
		for _, presence := range presences {
			logger.Printf("match join username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
		}
	}

	return state
}

func (m *Match) MatchLeave(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, presences []runtime.Presence) interface{} {
	if state.(*MatchState).debug {
		for _, presence := range presences {
			logger.Printf("match leave username %v user_id %v session_id %v node %v", presence.GetUsername(), presence.GetUserId(), presence.GetSessionId(), presence.GetNodeId())
		}
	}

	return state
}

func (m *Match) MatchLoop(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, messages []runtime.MatchData) interface{} {
	if state.(*MatchState).debug {
		logger.Printf("match loop match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match loop match_id %v message count %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), len(messages))
	}
	for _, message := range messages { 
		//entry.UserID, entry.SessionId, entry.Username, entry.Node, entry.OpCode, entry.Data, entry.ReceiveTime
		
		dispatcher.BroadcastMessage(message.GetOpCode(), message.GetData(), nil, nil)
	}
	
	
	//end if no ones sending smth (all dc'ed)
	if len(messages) == 0 {
		state.(*MatchState).emptyCounter = state.(*MatchState).emptyCounter + 1;
	} else {
		state.(*MatchState).emptyCounter = 0
	}
	
	if state.(*MatchState).emptyCounter == 20 {
		return nil
	}
	
	return state
}

func (m *Match) MatchTerminate(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, dispatcher runtime.MatchDispatcher, tick int64, state interface{}, graceSeconds int) interface{} {
	if state.(*MatchState).debug {
		logger.Printf("match terminate match_id %v tick %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), tick)
		logger.Printf("match terminate match_id %v grace seconds %v", ctx.Value(runtime.RUNTIME_CTX_MATCH_ID), graceSeconds)
	}

	return state
}

