
package main

import (
	"context"
    "encoding/json"
	"database/sql"
    "github.com/heroiclabs/nakama/api"
	"github.com/heroiclabs/nakama/runtime"
	"github.com/gofrs/uuid"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	logger.Print("RUNNING IN GO")
	if err := initializer.RegisterMatchmakerMatched(createNewMatch); err != nil {
		return err
	}
	if err := initializer.RegisterMatch("match", func(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) (runtime.Match, error) {
		return &Match{}, nil
	}); err != nil {
		return err
	}
	
	if err := initializer.RegisterRpc("getPlayers", GetPlayers); err != nil {
		return err
	}	
	return nil
}


func createNewMatch(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, entries []runtime.MatchmakerEntry) (string, error) {	
	logger.Print(" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>> createNewMatch <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<")
	for _, entry := range entries { 
		logger.Printf("%+v\n", entry)
	}
	
    params := map[string]interface{}{ "debug": "true" }
    matchID, err := nk.MatchCreate(ctx, "match", params)
    if err != nil {
        // Handle errors as you want.
        return "", err
    }
    return matchID, nil
}

func GetPlayers(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	query := `
		SELECT id, username, display_name, avatar_url,
		lang_tag, location, timezone, metadata
		FROM users`

	rows, err := db.QueryContext(ctx, query)
	if err != nil {
		logger.Printf("Error retrieving players.", (err))
		return "", err
	}
	defer rows.Close()

	players := make([]*api.User, 0)

	for rows.Next() {
		var id string
		var username sql.NullString
		var displayName sql.NullString
		var avatarURL sql.NullString
		var lang sql.NullString
		var location sql.NullString
		var timezone sql.NullString
		var metadata []byte

		if err = rows.Scan(&id, &username, &displayName, &avatarURL, &lang, &location, &timezone, &metadata); err != nil {
			logger.Printf("Error retrieving players.", (err))
			return "", err
		}

		playerID := uuid.FromStringOrNil(id)
/*
		online := false
		if tracker != nil {
			online = tracker.StreamExists(PresenceStream{Mode: StreamModeNotifications, Subject: playerID})
		}
*/
		user := &api.User{
			Id:          playerID.String(),
			Username:    username.String,
			DisplayName: displayName.String,
			AvatarUrl:   avatarURL.String,
			LangTag:     lang.String,
			Location:    location.String,
			Timezone:    timezone.String,
			Metadata:    string(metadata),
			//Online:      online,
		}

		players = append(players, user)
	}
	if err = rows.Err(); err != nil {
		logger.Printf("Error retrieving players.", (err))
		return "", err
	}
	
	b, err := json.Marshal(players)
	if err != nil {
		logger.Printf("Error converting players to json.", (err))
    }
	return string(b), nil
}
