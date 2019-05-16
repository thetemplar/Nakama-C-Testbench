package main

type GameDB struct {
	Spells				map[int64]*GameDB_Spells
	Procs				map[int64]*GameDB_Procs
	
	Spellbook			[]*GameDB_Spells
}

type GameDB_Procs struct {
	Spell 				*GameDB_Spells
	Proc				[]*GameDB_Procs_Event
	Chance				float32
}

type GameDB_Procs_Event int8
const (
	GameDB_Procs_Event_OnDeath = 0
	GameDB_Procs_Event_AutoAttack = 1
	GameDB_Procs_Event_OnDamageDone = 2
	GameDB_Procs_Event_OnDamageReceived = 3
	GameDB_Procs_Event_OnPhysicalAttackDone = 4
	GameDB_Procs_Event_OnPhysicalAttackReceived = 5
	GameDB_Procs_Event_OnMagicalAttackDone = 6
	GameDB_Procs_Event_OnMagicalAttackReceived = 7
	GameDB_Procs_Event_OnHealingDone = 8
	GameDB_Procs_Event_OnHealingReceived = 9
)

type GameDB_Spells struct {
	Id					int64
	Name 				string
	Description 		string
	Visable				bool
	ThreadModifier		int32
	Cooldown			float32
	GlobalCooldown		float32
	Dispellable			bool
	MissileID			int32
	EffectID			int32
	IconID		 		int64
	Speed				float32
	Radius				float32
	BaseCost			float32
	CostPerSec			float32
	CostPercentage  	float32
	SpellDamage			float32
	SpellDamageType		GameDB_Spells_DamageType
	CastDuration		float32
	Duration			float32
	Range				float32
	FacingFront			bool
	TargetAuraRequired 	int64
	CasterAuraRequired 	int64
	Mechanic			GameDB_Spells_Mechanic
	
	Target				GameDB_Spells_Target
	ApplySpell			[]*GameDB_Spells
	
	Interrupt			GameDB_Interrupts
}


type GameDB_Spells_DamageType int
const (
	GameDB_Spells_DamageType_Physical = 0
	GameDB_Spells_DamageType_Arcane = 1
	GameDB_Spells_DamageType_Fire = 2
	GameDB_Spells_DamageType_Frost = 3
	GameDB_Spells_DamageType_Nature = 4
	GameDB_Spells_DamageType_Shadow = 5
	GameDB_Spells_DamageType_Holy = 6
)

type GameDB_Spells_Mechanic int
const (
	GameDB_Spells_Mechanic_None = 0
	GameDB_Spells_Mechanic_Rooted = 1
	GameDB_Spells_Mechanic_Sapped = 2
	GameDB_Spells_Mechanic_Invulnerable = 3
	GameDB_Spells_Mechanic_Interrupted = 4
	GameDB_Spells_Mechanic_Infected = 5
	GameDB_Spells_Mechanic_Shielded = 6
	GameDB_Spells_Mechanic_Slowed = 7
	GameDB_Spells_Mechanic_Stunned = 8
	GameDB_Spells_Mechanic_Healing = 9
)

type GameDB_Spells_Target int8
const (
	GameDB_Spells_Target_None = 0
	GameDB_Spells_Target_Unit = 1
	GameDB_Spells_Target_Enemy = 2
	GameDB_Spells_Target_Ally = 3
	GameDB_Spells_Target_Dead = 4
	GameDB_Spells_Target_DeadEnemy = 5
	GameDB_Spells_Target_DeadAlly = 6
	GameDB_Spells_Target_AoE = 7
)

type GameDB_Interrupts int8
const (
	GameDB_Interrupts_None = 0
	GameDB_Interrupts_OnMovement = 1
	GameDB_Interrupts_OnKnockback = 2
	GameDB_Interrupts_OnInterruptCast = 3
	GameDB_Interrupts_OnDamageTaken = 4
	GameDB_Interrupts_OnAttackingMeele = 5
	GameDB_Interrupts_OnAttackingSpell = 6
)