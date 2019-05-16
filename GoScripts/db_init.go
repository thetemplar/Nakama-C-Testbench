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
		Visable				:true,
		ThreadModifier		:1,
		Cooldown			:0,
		GlobalCooldown		:1.5,
		Dispellable			:false,
		MissileID			:1,
		EffectID			:0,
		IconID		 		:1,
		Speed				:50,
		Radius				:0,
		BaseCost			:0,
		CostPerSec			:0,
		CostPercentage  	:0,
		SpellDamage			:10,
		SpellDamageType		:GameDB_Spells_DamageType_Fire,
		CastDuration		:0,
		Duration			:0,
		Range				:100,
		FacingFront			:false,
		TargetAuraRequired  :0,
		CasterAuraRequired  :0,
		Mechanic			:GameDB_Spells_Mechanic_None,
		
		Targets				:[]GameDB_Spells_Target { GameDB_Spells_Target_Unit },
		ApplySpell			:[]*GameDB_Spells {},
		
		Interrupt			:GameDB_Interrupts_None,
	}
	GameDB.Spells[1] = fireball

	return GameDB
}


func (spell GameDB_Spells) OnHit() {
	
}