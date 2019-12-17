using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilityInfo
{
	public enum SkillUsed
	{
		none = 0,
		heavy_hits = 1,
		light_hits = 2,
		healing = 4,
		magic = 8,
	}

	public enum AbilityType
	{
		none = 0,
		misc = 1,
		debuff = 2,
		recovery = 4,
		offensive = 8,
		attack = 16,
		buff = 32,
	}

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

	public struct TargetData
	{
		public TargetData(CombatController _self = null, CombatController _target = null, int _constant = 0, Elementals _element = default, Vector3 _centerPos = default, StatBlock.Race _race = default)
		{
			self = _self;
			target = _target;
			bonus = _constant;
			element = _element;
			centerPos = _centerPos;
			targetRace = _race;
		}

		public CombatController self;
		public CombatController target;
		public int bonus;
		public Elementals element;
		public Vector3 centerPos;
		public StatBlock.Race targetRace;
	}

	public class AbilityIcons
	{
		public static Dictionary<string, Sprite> buffIconDictionary;

		public static Sprite TryGetBuffIcon(string _name)
		{
			return buffIconDictionary.TryGetValue(_name, out var y) ? buffIconDictionary[_name] : buffIconDictionary["default"];
		}
	}

	public class Ability
	{
		public delegate IEnumerator FunctionToCall(TargetData inputData);

		public Ability(string _name, FunctionToCall _function, Elementals _element = default, SkillUsed _skill = default, AbilityType _type = default, int _manaCost = 0)
		{
			name = _name;
			function = _function;
			manaCost = _manaCost;
			element = _element;
			abilityType = _type;
		}
		
		public string name;
		public FunctionToCall function;
		public int manaCost;
		public Elementals element;
		public AbilityType abilityType;
		public SkillUsed abilityClass;
	}
}


