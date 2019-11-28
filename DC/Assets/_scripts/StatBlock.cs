using System.Collections.Generic;
using UnityEngine;

public class StatBlock
{
	public enum Race
	{
		Human,
		Dwarf,
		Elf,
		Undead,
		Orc,
		Elemental,
		Demon,
	}

	public enum AIType
	{
		None,
		Dumb,
		Smart,
		Genious,
		Coward,
		Sprinter,
	}

	public enum StackType
	{
		Pick_Most_Turns,
		Pick_Most_Potential,
		Stack_Self,
		Build_Up,
	}

	public StatBlock(Race _race,
		string _name,
		int _maxHealth,int _maxMana,
		int _level,int _xp,
		int _strenght,int _dexterity,int _intelligence,int _luck,
		List<string> _abilities, //should be List<Ienumerator> to have less human errors
		AbilityScript.Elementals _weaknesses = default, AbilityScript.Elementals _resistances = default, AbilityScript.Elementals _immunities = default, AbilityScript.Elementals _absorbs = default,
		AIType _aiType = default,
		List<AbilityScript.Buff> _buffs = default)
	{
		race = _race;
		name = _name;
		weaknesses = _weaknesses;
		resistances = _resistances;
		immunities = _immunities;
		absorbs = _absorbs;

		maxHealth = _maxHealth;
		maxMana = _maxMana;

		level = _level;
		xp = _xp;

		baseStrength = _strenght;
		baseDexterity = _dexterity;
		baseIntelligence = _intelligence;
		baseLuck = _luck;
		abilities = new List<string>();
		for(int i = 0; i < _abilities.Count; i++)
		{
			abilities.Add(_abilities[i]);//.ToLower());
		}
		aiType = _aiType;
	}

	public Race race;
	public string name;
	public AbilityScript.Elementals resistances, weaknesses, immunities, absorbs;
	public int maxHealth,
		maxMana;
	public int level,
		xp;

	public int strength
	{
		get
		{
			int _bs = baseStrength;
			foreach(AbilityScript.Buff _b in buffList)
				if(_b.function.Contains("strength")) _bs += (int)_b.constant;

			return _bs;
		}
	}

	public int dexterity
	{
		get
		{
			int _bd = baseDexterity;
			foreach(AbilityScript.Buff _b in buffList)
				if(_b.function.Contains("dexterity")) _bd += (int)_b.constant;

			return _bd;
		}
	}

	public int intelligence
	{
		get
		{
			int _bi = baseIntelligence;
			foreach(AbilityScript.Buff _b in buffList)
				if(_b.function.Contains("intelligence")) _bi += (int)_b.constant;

			return _bi;
		}
	}

	public int luck
	{
		get
		{
			int _bl = baseLuck;
			foreach(AbilityScript.Buff _b in buffList)
				if(_b.function.Contains("luck")) _bl += (int)_b.constant;

			return _bl;
		}
	}

	public int baseStrength,
		baseDexterity,
		baseIntelligence,
		baseLuck;

	public List<string> abilities;
	public AIType aiType;

	public List<AbilityScript.Buff> buffList = new List<AbilityScript.Buff>();

	public StatBlock Clone()
	{
		var _clone = (StatBlock)MemberwiseClone();
		_clone.buffList = new List<AbilityScript.Buff>();
		_clone.abilities = new List<string>();
		_clone.abilities.AddRange(abilities);
		//Debug.Log(_clone.abilities.Count);
		return _clone;// MemberwiseClone();
	}
}
