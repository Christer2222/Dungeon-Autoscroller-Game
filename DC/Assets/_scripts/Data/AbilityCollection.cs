using AbilityInfo;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollection
{
	public static List<Ability> DebugAbilities()
	{
		return new List<Ability>()
		{
			poison, poisonBite, airSlash, bubble, crystalLance,
			eruption, thunderbolt, hardenSkin, magicShield, meteorShower, freezingStrike,
			chaosThesis, debulk, divineFists, bulkUp, manaDrain, divineLuck, regeneration,
			restoreSoul, clense, syncSoul, curse, bless, punch, doubleKick, wildPunch, forcePunch,
			spotWeakness, smiteUnlife, siphonSoul, heal, lifeTap, massHeal,fireball, focus,
			timeWarp, keenSight,

			tiltSwing, massExplosion,
		};
	}

	/*
	public static List<Ability> debugAbilityList = new List<Ability>()
	{
			poision, poisionBite, airSlash, bubble, crystalLance,
			eruption, thunderbolt, hardenSkin, magicShield, meteorShower, freezingStrike,
			chaosThesis, debulk, divineFists, bulkUp, manaDrain, divineLuck, regeneration,
			restoreSoul, clense, syncSoul, curse, bless, punch, doubleKick, wildPunch, forcePunch,
			spotWeakness, smiteUnlife, siphonSoul, heal, lifeTap, massHeal,fireball, focus, 
			timeWarp, keenSight,

			tiltSwing, massExplosion,
	};
	*/

	public static readonly Ability punch = new Ability("Punch", AbilityScript.Punch, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);
	public static readonly Ability fireball = new Ability("Fireball", AbilityScript.Fireball, Elementals.Fire, SkillUsed.magic, AbilityType.attack, -2, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability massExplosion = new Ability("Mass Explosion", AbilityScript.MassExplosion, Elementals.Fire, SkillUsed.magic, AbilityType.attack, -4, ExtraData.nonPiercing);
	public static readonly Ability smiteUnlife = new Ability("Smite Undead", AbilityScript.Smite, Elementals.None, SkillUsed.healing, AbilityType.attack, -1, ExtraData.nonPiercing);
	public static readonly Ability doubleKick = new Ability("Double Kick", AbilityScript.DoubleKick, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);
	public static readonly Ability wildPunch = new Ability("Wild Punch", AbilityScript.WildPunch, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);
	public static readonly Ability tiltSwing = new Ability("Tilt Swing", AbilityScript.TiltSwing, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.attack, 0, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);
	public static readonly Ability forcePunch = new Ability("Force Punch", AbilityScript.ForcePunch, Elementals.Air, SkillUsed.heavy_hits | SkillUsed.magic, AbilityType.attack, -1, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);
	public static readonly Ability chaosThesis = new Ability("Chaos Thesis", AbilityScript.ChaosThesis, Elementals.Void, SkillUsed.magic | SkillUsed.healing, AbilityType.attack, -1, ExtraData.none);
	public static readonly Ability manaDrain = new Ability("Mana Drain", AbilityScript.ManaDrain, Elementals.Water, SkillUsed.healing, AbilityType.attack, -3, ExtraData.magic);
	public static readonly Ability meteorShower = new Ability("Meteor Shower", AbilityScript.MeteorShower, Elementals.Fire | Elementals.Earth, SkillUsed.magic, AbilityType.attack, -7, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability freezingStrike = new Ability("Freezing Strike", AbilityScript.FreezingStrike, Elementals.Ice, SkillUsed.magic, AbilityType.attack, -1, ExtraData.nonPiercing | ExtraData.magic | ExtraData.makes_contact_with_user);
	public static readonly Ability thunderbolt = new Ability("Thunderbolt", AbilityScript.Thunderbolt, Elementals.Electricity, SkillUsed.magic, AbilityType.attack, -4, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability eruption = new Ability("Eruption", AbilityScript.Eruption, Elementals.Fire, SkillUsed.magic, AbilityType.attack, -5, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability airSlash = new Ability("Air Slash", AbilityScript.AirSlash, Elementals.Air, SkillUsed.magic, AbilityType.attack, -3, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability bubble = new Ability("Bubble", AbilityScript.Bubble, Elementals.Water, SkillUsed.magic, AbilityType.attack, -2, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability poison = new Ability("Poison", AbilityScript.Poison, Elementals.Poison, SkillUsed.light_hits | SkillUsed.magic, AbilityType.attack, -2, ExtraData.none);
	public static readonly Ability poisonBite = new Ability("Poison Bite", AbilityScript.PoisonBite, Elementals.Physical | Elementals.Poison, SkillUsed.heavy_hits | SkillUsed.light_hits, AbilityType.attack, 0, ExtraData.nonPiercing | ExtraData.makes_contact_with_user);

	public static readonly Ability siphonSoul = new Ability("Siphon Soul", AbilityScript.SiphonSoul, Elementals.Unlife, SkillUsed.healing, AbilityType.attack | AbilityType.recovery, -1, ExtraData.magic);

	public static readonly Ability crystalLance = new Ability("Crystal Lance", AbilityScript.CrystalLance, Elementals.Earth | Elementals.Air, SkillUsed.heavy_hits, AbilityType.attack | AbilityType.defensive, -2, ExtraData.nonPiercing | ExtraData.magic);

	public static readonly Ability focus = new Ability("Focus", AbilityScript.Focus, Elementals.Water, SkillUsed.magic, AbilityType.recovery, 0, ExtraData.magic);
	public static readonly Ability heal = new Ability("Heal", AbilityScript.Heal, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -2, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability regeneration = new Ability("Regeneration", AbilityScript.Regeneration, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -1, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability massHeal = new Ability("Mass Heal", AbilityScript.MassHeal, Elementals.Light, SkillUsed.healing, AbilityType.recovery, -5, ExtraData.nonPiercing | ExtraData.magic);
	public static readonly Ability restoreSoul = new Ability("Restore Soul", AbilityScript.RestoreSoul, Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0, ExtraData.magic);
	public static readonly Ability clense = new Ability("Clense", AbilityScript.Clense, Elementals.Water, SkillUsed.healing, AbilityType.recovery, 0, ExtraData.magic);

	public static readonly Ability timeWarp = new Ability("Time Warp", AbilityScript.TimeWarp, Elementals.Void, SkillUsed.magic, AbilityType.buff, -10, ExtraData.magic);
	public static readonly Ability divineLuck = new Ability("Divine Luck", AbilityScript.DivineLuck, Elementals.Light, SkillUsed.healing | SkillUsed.heavy_hits, AbilityType.buff, -3, ExtraData.magic);
	public static readonly Ability bulkUp = new Ability("Bulk Up", AbilityScript.BulkUp, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.buff, -1, ExtraData.none);
	public static readonly Ability divineFists = new Ability("Divine Fists", AbilityScript.DivineFists, Elementals.Physical | Elementals.Light, SkillUsed.healing, AbilityType.buff, -6, ExtraData.magic);
	public static readonly Ability bless = new Ability("Bless", AbilityScript.Bless, Elementals.Fire, SkillUsed.healing, AbilityType.buff, -10, ExtraData.magic);

	public static readonly Ability debulk = new Ability("Debulk", AbilityScript.Debulk, Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -2, ExtraData.magic);
	public static readonly Ability curse = new Ability("Curse", AbilityScript.Curse, Elementals.Unlife, SkillUsed.healing, AbilityType.debuff, -5, ExtraData.magic);

	public static readonly Ability hardenSkin = new Ability("Harden Skin", AbilityScript.Harden, Elementals.Physical, SkillUsed.heavy_hits, AbilityType.defensive, -3, ExtraData.none);
	public static readonly Ability magicShield = new Ability("Magic Shield", AbilityScript.MagicShield, Elementals.Water, SkillUsed.magic, AbilityType.defensive, -3, ExtraData.none);

	//public static Ability eat							= new Ability("Eat",					Eat, default, default, AbilityType.misc, 0);
	public static readonly Ability keenSight = new Ability("Keen Sight", AbilityScript.DisplayCritAreas, Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1, ExtraData.none);
	public static readonly Ability spotWeakness = new Ability("Spot Weakness", AbilityScript.SpotWeakness, Elementals.Physical, SkillUsed.light_hits, AbilityType.misc, -1, ExtraData.none);
	public static readonly Ability lifeTap = new Ability("Life Tap", AbilityScript.LifeTap, Elementals.Unlife, SkillUsed.healing, AbilityType.misc, 0, ExtraData.magic);
	public static readonly Ability syncSoul = new Ability("Sync Soul", AbilityScript.SyncSoul, Elementals.Void, SkillUsed.healing | SkillUsed.magic, AbilityType.misc, -10, ExtraData.magic);

	public static readonly Ability wobble = new Ability("Wobble", AbilityScript.Wobble, Elementals.None, SkillUsed.none, AbilityType.none, 0, ExtraData.none);
	public static readonly Ability poisonTick = new Ability("Poison Tick", AbilityScript.PoisonTick, Elementals.Poison, SkillUsed.none, AbilityType.none, 0, ExtraData.none);
}
