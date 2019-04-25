
package main

import (
	"fmt"
	"context"
	"database/sql"
    "github.com/heroiclabs/nakama/api"
	"github.com/heroiclabs/nakama/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	logger.Print("RUNNING IN GO: Groups and Friends")
	
	//send note to blocked user & send refresh friendlist notification
	if err := initializer.RegisterAfterBlockFriends(NotificationBlockFriends); err != nil {
		return err
	}
	//send note to deleted user & send refresh friendlist notification
	if err := initializer.RegisterAfterDeleteFriends(NotificationDeleteFriends); err != nil {
		return err
	}
	//only send refresh friendlist notification
	if err := initializer.RegisterAfterAddFriends(NotificationAddFriends); err != nil {
		return err
	}
	
	//send note to refresh grouplist
	if err := initializer.RegisterAfterJoinGroup(NotificationJoinGroup); err != nil {
		return err
	}	
	//send note to refresh grouplist
	if err := initializer.RegisterAfterPromoteGroupUsers(NotificationPromoteGroupUsers); err != nil {
		return err
	}
	//send note to refresh grouplist
	if err := initializer.RegisterAfterKickGroupUsers(NotificationKickGroupUsers); err != nil {
		return err
	}
	//send note to refresh grouplist
	if err := initializer.RegisterAfterAddGroupUsers(NotificationAddGroupUsers); err != nil {
		return err
	}
	//send note to refresh grouplist
	if err := initializer.RegisterAfterLeaveGroup(NotificationLeaveGroup); err != nil {
		return err
	}
	//send note to refresh grouplist
	if err := initializer.RegisterAfterDeleteGroup(NotificationDeleteGroup); err != nil {
		return err
	}
	//send note to refresh grouplist
	if err := initializer.RegisterAfterUpdateGroup(RegisterAfterUpdateGroup); err != nil {
		return err
	}
	//send note to refresh grouplist
	
	return nil
}

//friends
func NotificationBlockFriends(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.BlockFriendsRequest) error {	
	userID := ctx.Value("user_id").(string)
	//to the now-blocked user
	subject := fmt.Sprintf("%v blocked you", userID)	
    content := map[string]interface{}{ "username": userID }
	nk.NotificationSend(ctx, in.Ids[0], subject, content, 1, "" , true)
	
	//confirm to the sender
	subject = fmt.Sprintf("you blocked %v", userID)
    content = map[string]interface{}{ "username": in.Ids[0] }
	nk.NotificationSend(ctx, userID, subject, content, 2, "" , true)
	nk.NotificationSend(ctx, userID, subject, content, 1, "" , true)
	
	logger.Printf("Intercepted NotificationBlockFriends: %s blocked %s", userID, in.Ids[0])
	return nil
}

func NotificationDeleteFriends(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.DeleteFriendsRequest) error {
	userID := ctx.Value("user_id").(string)
	
	//to the now-deleted user
	subject := fmt.Sprintf("%v deleted you from his friendlist", userID)
    content := map[string]interface{}{ "username": userID }
	nk.NotificationSend(ctx, in.Ids[0], subject, content, 3, "" , true)
	nk.NotificationSend(ctx, in.Ids[0], subject, content, 1, "" , true)
	
	//confirm to the sender
	subject = fmt.Sprintf("you deleted %v from your friendlist", userID)
    content = map[string]interface{}{ "username": in.Ids[0] }
	nk.NotificationSend(ctx, userID, subject, content, 1, "" , true)
	
	logger.Printf("Intercepted NotificationDeleteFriends: %s deleted %s from his list", userID, in.Ids[0])	
	return nil
}

func NotificationAddFriends(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.AddFriendsRequest) error {
	userID := ctx.Value("user_id").(string)
	
	//to the now-deleted user
	subject := fmt.Sprintf("%v added you from to his friendlist", userID)
    content := map[string]interface{}{ "username": userID }
	nk.NotificationSend(ctx, in.Ids[0], subject, content, 1, "" , true)
	
	//confirm to the sender
	subject = fmt.Sprintf("you added %v to your friendlist", userID)
    content = map[string]interface{}{ "username": in.Ids[0] }
	nk.NotificationSend(ctx, userID, subject, content, 1, "" , true)
	
	logger.Printf("Intercepted NotificationAddFriends: %s added %s to his list", userID, in.Ids[0])	
	return nil
}

//groups
func NotificationJoinGroup(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.JoinGroupRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationJoinGroup")
	return nil
}

func NotificationPromoteGroupUsers(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.PromoteGroupUsersRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationPromoteGroupUsers")
	return nil
}

func NotificationKickGroupUsers(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.KickGroupUsersRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationKickGroupUsers")
	return nil
}

func NotificationAddGroupUsers(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.AddGroupUsersRequest) error {	
	logger.Printf("Intercepted NotificationAddGroupUsers")
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationAddGroupUsers")
	return nil
}

func NotificationLeaveGroup(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.LeaveGroupRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationLeaveGroup")
	return nil
}

func NotificationDeleteGroup(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.DeleteGroupRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted NotificationDeleteGroup")
	return nil
}

func RegisterAfterUpdateGroup(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, in *api.UpdateGroupRequest) error {	
	query := "SELECT source_id, destination_id FROM group_edge WHERE source_id = $1::UUID"

	rows, err := db.QueryContext(ctx, query, in.GroupId)
	if err != nil {
		logger.Printf("Error retrieving group_edge. %s ", (err))
		return err
	}
	defer rows.Close()
	for rows.Next() {
		var source_id string
		var destination_id string
		if err = rows.Scan(&source_id, &destination_id); err != nil {
			logger.Printf("Error retrieving group_edge. %s ", (err))
			return err
		}
		
		subject := fmt.Sprintf("NotificationJoinGroup %v", source_id)
		content := map[string]interface{}{ "group": source_id }
		nk.NotificationSend(ctx, destination_id, subject, content, 5, "" , true)
	}	
	
	logger.Printf("Intercepted RegisterAfterUpdateGroup")
	return nil
}







