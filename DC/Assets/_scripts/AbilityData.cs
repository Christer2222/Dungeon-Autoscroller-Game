using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityData : MonoBehaviour
{
	protected class AbilityTest
	{
		public AbilityTest(string _name, IEnumerator _function, Elementals _element = default, AbilityType _type = default, int _manaCost = 0)
		{
			name = _name;
			manaCost = _manaCost;
			element = _element;
			type = _type;
			function = _function;
		}

		public string name;
		public int manaCost;
		public Elementals element;
		public AbilityType type;
		public IEnumerator function;
	}

	//protected AbilityTest punch = new AbilityTest("Punch", LifeTap, Elementals.Physical, AbilityType.attack, 0);

	protected const string
		NONE = "None",
		EAT = "Eat",
		SPOOK = "Spook",
		PUNCH = "Punch",
		DOUBLE_KICK = "Double Kick",
		WILD_PUNCH = "Wild Punch",
		TILT_SWING = "Tilt Swing",
		FORCE_PUNCH = "Force Punch",
		CHAOS_THESIS = "Chaos Thesis",
		FOCUS = "Focus",
		FIREBALL = "Fireball",
		MASS_EXPLOSION = "Mass Explosion",
		HEAL = "Heal",
		MASS_HEAL = "Mass Heal",
		KEEN_SIGHT = "Keen Sight",
		SMITE_UNLIFE = "Smite Unlife",
		SIPHON_SOUL = "Siphon Soul",
		SPOT_WEAKNESS = "Spot Weakness",
		REGENERATION = "Regeneration",
		TIME_WARP = "Time Warp",
		DIVINE_LUCK = "Divine Luck",
		BULK_UP = "Bulk Up",
		MANA_DRAIN = "Mana Drain",
		LIFE_TAP = "Life Tap",
		DIVINE_FISTS = "Divine Fists",
		DEBULK = "Debulk";


	protected static Dictionary<string, int> manaCostDictionary = new Dictionary<string, int>()
	{
		{NONE,-0 },
		{FORCE_PUNCH, -1 },
		{CHAOS_THESIS, -1 },
		{FIREBALL,-2 },
		{MASS_EXPLOSION, -4 },
		{HEAL,-2 },
		{MASS_HEAL, -5 },
		{KEEN_SIGHT, -1 },
		{SMITE_UNLIFE, -1 },
		{SIPHON_SOUL, -3 },
		{SPOT_WEAKNESS, -1 },
		{REGENERATION, -1 },
		{TIME_WARP, -10 },
		{DIVINE_LUCK, -3 },
		{BULK_UP, -1 },
		{MANA_DRAIN, -3 },
		{DIVINE_FISTS, -6 },
		{DEBULK, -2 },
	};

	public enum AbilityType
	{
		none = 0,
		misc = 1,
		defensive = 2,
		recovery = 4,
		offensive = 8,
		attack = 16,
		buff = 32,
	}

	protected static Dictionary<string, AbilityType> abilityTypeDictionary = new Dictionary<string, AbilityType>()
	{
		{PUNCH, AbilityType.attack},
		{FIREBALL, AbilityType.attack},
		{MASS_EXPLOSION, AbilityType.attack},
		{SPOOK, AbilityType.attack},
		{SMITE_UNLIFE, AbilityType.attack},

		{SIPHON_SOUL, AbilityType.recovery | AbilityType.attack},

		{BULK_UP, AbilityType.buff },
		{DIVINE_LUCK, AbilityType.buff },
		{DIVINE_FISTS, AbilityType.buff },


		{HEAL, AbilityType.recovery},
		{MASS_HEAL, AbilityType.recovery},
		{FOCUS, AbilityType.recovery},
		{EAT, AbilityType.recovery},
		{REGENERATION, AbilityType.recovery},
		{MANA_DRAIN, AbilityType.recovery},


		{LIFE_TAP, AbilityType.misc},
		{SPOT_WEAKNESS, AbilityType.misc },
		{KEEN_SIGHT, AbilityType.misc},
		{TIME_WARP, AbilityType.misc },

		{DEBULK, AbilityType.defensive }
	};


	//can combine elements by doing Elem | Elem and check by doing 
	public enum Elementals
	{
		None = 0,
		Physical = 1,
		Fire = 2,
		Water = 4,
		Earth = 8,
		Air = 16,
		Plasma = 32,
		Ice = 64,
		Poision = 128,
		Electricity = 256,
		Steam = 512,
		Light = 1024,
		Unlife = 2048,
		Void = 4096,
	}

	protected static Dictionary<string, Sprite> buffIconDictionary;

	protected Sprite TryGetBuffIcon(string _name)
	{
		return buffIconDictionary.TryGetValue(_name, out var y) ? buffIconDictionary[_name] : buffIconDictionary["default"];
	}
}
