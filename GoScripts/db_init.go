package main

func init_db() *GameDB {

	GameDB := &GameDB {
		Spells: make(map[int64]*GameDB_Spells),
		Procs: make(map[int64]*GameDB_Procs),
		Spellbook: make([]*GameDB_Spells, 0),
	}

	fireball := &GameDB_Spells{
		Id					:1,
		Name 				:"Fireball",
		Description 		:"Fireball",
		Visible				:true,
		ThreadModifier		:1,
		Cooldown			:0,
		GlobalCooldown		:1.5,
		Dispellable			:false,
		MissileID			:1,
		EffectID			:0,
		IconID		 		:1,
		Speed				:100,
		Radius				:0,
		BaseCost			:0,
		CostPerSec			:0,
		CostPercentage  	:0,
		SpellDamageMin		:10,
		SpellDamageMax		:20,
		SpellDamageType		:GameDB_Spells_DamageType_Fire,
		CastDuration		:1,
		Duration			:0,
		Range				:100,
		FacingFront			:true,
		TargetAuraRequired  :0,
		CasterAuraRequired  :0,
		Mechanic			:GameDB_Spells_Mechanic_None,
		
		Target				:GameDB_Spells_Target_Unit,
		ApplySpell			:[]*GameDB_Spells {},
		
		Interrupt			:GameDB_Interrupts_None,
	}
	GameDB.Spells[1] = fireball

	
	chilled := &GameDB_Spells{
		Id					:3,
		Name 				:"Chilled",
		Description 		:"Chilled",
		Visible				:true,
		ThreadModifier		:1,
		Cooldown			:0,
		GlobalCooldown		:1.5,
		Dispellable			:false,
		MissileID			:0,
		EffectID			:1,
		IconID		 		:3,
		Speed				:0,
		Radius				:0,
		BaseCost			:0,
		CostPerSec			:0,
		CostPercentage  	:0,
		SpellDamageMin		:0,
		SpellDamageMax		:0,
		SpellDamageType		:GameDB_Spells_DamageType_Frost,
		CastDuration		:0,
		Duration			:5,
		Range				:0,
		FacingFront			:false,
		TargetAuraRequired  :0,
		CasterAuraRequired  :0,
		Mechanic			:GameDB_Spells_Mechanic_Slowed,
		
		Target				:GameDB_Spells_Target_None,
		ApplySpell			:[]*GameDB_Spells {},
		
		Interrupt			:GameDB_Interrupts_None,
	}
	GameDB.Spells[3] = chilled

	
	frostball := &GameDB_Spells{
		Id					:2,
		Name 				:"Frostball",
		Description 		:"Frostball",
		Visible				:true,
		ThreadModifier		:1,
		Cooldown			:0,
		GlobalCooldown		:1.5,
		Dispellable			:false,
		MissileID			:2,
		EffectID			:0,
		IconID		 		:2,
		Speed				:50,
		Radius				:0,
		BaseCost			:0,
		CostPerSec			:0,
		CostPercentage  	:0,
		SpellDamageMin		:5,
		SpellDamageMax		:10,
		SpellDamageType		:GameDB_Spells_DamageType_Frost,
		CastDuration		:0,
		Duration			:0,
		Range				:100,
		FacingFront			:true,
		TargetAuraRequired  :0,
		CasterAuraRequired  :0,
		Mechanic			:GameDB_Spells_Mechanic_None,
		
		Target				:GameDB_Spells_Target_Unit,
		ApplySpell			:[]*GameDB_Spells { GameDB.Spells[3] },
		
		Interrupt			:GameDB_Interrupts_None,
	}
	GameDB.Spells[2] = frostball
	return GameDB
}