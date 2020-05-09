using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Buff
{
	public enum TraitType
	{
		None,
		Strength_Multiplier,
		Strength_Constant,
		Dexterity_Multiplier,
		Dexterity_Constant,
		Intelligence_Multiplier,
		Intelligence_Constant,
		Luck_Multiplier,
		Luck_Constant,
		Extra_Turn,
		Busy,
		Magic_Defence_Constant,
		Physical_Defence_Constant,
	}

	public Buff(string _name, TraitType _trait, int _turns, Sprite _buffIcon, StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true) : this(_name, new List<TraitType> { _trait }, _turns, _buffIcon, _stackType, _constant, _target, _shouldBeDisplyed) { }
	public Buff(string _name, Ability _function, int _turns, Sprite _buffIcon, StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true) : this(_name, new List<Ability> { _function }, _turns, _buffIcon, _stackType, _constant, _target, _shouldBeDisplyed) { }

	public Buff(string _name, List<TraitType> _traits, int _turns, Sprite _buffIcon, StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true)
	{
		name = _name;
		traits = _traits;
		turns = _turns;
		constant = _constant;
		target = _target;
		stackType = _stackType;
		buffIcon = _buffIcon;
		shouldBeDisplayed = _shouldBeDisplyed;
	}

	public Buff(string _name, List<Ability> _functions, int _turns, Sprite _buffIcon, StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true)
	{
		name = _name;
		functions = _functions;
		turns = _turns;
		constant = _constant;
		target = _target;
		stackType = _stackType;
		buffIcon = _buffIcon;
		shouldBeDisplayed = _shouldBeDisplyed;
	}

	public string name;
	public List<Ability> functions = new List<Ability>();
	public List<TraitType> traits = new List<TraitType>();
	public CombatController target;
	public float constant;
	public int turns;
	public StackType stackType;
	public Sprite buffIcon;
	public bool shouldBeDisplayed;

	public enum StackType
	{
		Pick_Most_Turns,
		Pick_Most_Potent,
		Add_Duplicate,
		Add_One_Duration_Add_All_Potency,
		Add_One_Duration_And_One_Potency,
	}

}

public class BuffIcons
{
	public static Dictionary<string, Sprite> buffIconDictionary;
	public static Sprite[] buffSpriteSheet = Resources.LoadAll<Sprite>("Sprites/UI/StatusSpriteSheet");

	public static Sprite TryGetBuffIcon(string _name)
	{
		return buffIconDictionary.TryGetValue(_name, out var y) ? buffIconDictionary[_name] : buffIconDictionary["default"];
	}

	public static Sprite TryGetBuffIcon(int _index)
	{
		return buffSpriteSheet[_index];
	}
}
