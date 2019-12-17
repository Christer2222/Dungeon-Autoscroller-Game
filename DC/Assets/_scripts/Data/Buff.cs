using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Buff
{
	public Buff(string _name, string _trait, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true) : this(_name, new List<string> { _trait }, _turns, _buffIcon, _stackType, _constant, _target, _shouldBeDisplyed) { }
	public Buff(string _name, Ability _function, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true) : this(_name, new List<Ability> { _function }, _turns, _buffIcon, _stackType, _constant, _target, _shouldBeDisplyed) { }

	public Buff(string _name, List<string> _traits, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true)
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

	public Buff(string _name, List<Ability> _functions, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true)
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
	public List<string> traits = new List<string>();
	public CombatController target;
	public float constant;
	public int turns;
	public StatBlock.StackType stackType;
	public Sprite buffIcon;
	public bool shouldBeDisplayed;
}
