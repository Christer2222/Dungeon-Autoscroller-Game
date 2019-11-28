using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
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
		DIVINE_FISTS = "Divine Fists";

	protected static Dictionary<string,int> manaCostDictionary = new Dictionary<string,int>()
	{
		{NONE,-0 },
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

	protected static Dictionary<string,AbilityType> abilityTypeDictionary = new Dictionary<string,AbilityType>()
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

	protected static Vector3 lastClick;

	protected static Dictionary<string, Sprite> buffIconDictionary;

	Sprite GetBuffIcon(string _name)
	{
		return buffIconDictionary.TryGetValue("pluss_strength", out var y) ? buffIconDictionary["pluss_strength"] : buffIconDictionary["default"];
	}

	public class Buff
	{
		public Buff(string _name, string _function, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null)
		{
			name = _name;
			function = _function;
			turns = _turns;
			constant = _constant;
			target = _target;
			stackType = _stackType;
			buffIcon = _buffIcon;
		}

		public string name;
		public string function;
		public CombatController target;
		public float constant;
		public int turns;
		public StatBlock.StackType stackType;
		public Sprite buffIcon;
	}

	void AddBuff(Buff _buff, CombatController _target)
	{
		var _same = _target.myStats.buffList.Find(x => x.name == _buff.name);
		switch(_buff.stackType)
		{
			case StatBlock.StackType.Pick_Most_Potential:
				if (_same != null)
				{
					if(_same.constant < _buff.constant || (_same.constant == _buff.constant && _same.turns < _buff.turns))
					{
						_target.myStats.buffList.Remove(_same);
						_target.myStats.buffList.Add(_buff);
					}
				}
				else
					_target.myStats.buffList.Add(_buff);

				break;
			case StatBlock.StackType.Pick_Most_Turns:
				if(_same != null)
				{
					if(_same.turns < _buff.turns || (_same.turns == _buff.turns && _same.constant < _buff.constant))
					{
						_target.myStats.buffList.Remove(_same);
						_target.myStats.buffList.Add(_buff);
					}
				}
				else
					_target.myStats.buffList.Add(_buff);
				break;
			case StatBlock.StackType.Stack_Self:
				_target.myStats.buffList.Add(_buff);
				break;
			case StatBlock.StackType.Build_Up:
				if(_same != null)
				{
					_same.turns++;
					_same.constant += _buff.constant;
				}
				else
					_target.myStats.buffList.Add(_buff);
				break;
			default:
				Debug.LogError("ERROR when adding buff: " + _buff.name + " to: " + _target.transform.name);
				break;

		}
	}

	protected RaycastHit2D CheckIfHit(Vector3 _clickPos)
	{
		Debug.DrawLine(_clickPos,_clickPos + Vector3.up * 0.1f,Color.red,4,false);
		Debug.DrawLine(_clickPos,_clickPos + Vector3.right * 0.1f,Color.blue,4,false);

		lastClick = _clickPos;

		RaycastHit2D _hit = Physics2D.Raycast(_clickPos,Vector2.zero,0.01f);
		return _hit;
	}


	protected IEnumerator DivineFists(CombatController _target)
	{
		yield return StartCoroutine(DivineLuck(_target));
		yield return StartCoroutine(BulkUp(_target));
		yield return StartCoroutine(Regeneration(_target,3));

	}

	protected IEnumerator Punch(CombatController _target,CombatController _self)
	{
		EffectTools.SpawnEffect(PUNCH, lastClick,1);
		if(_target != null) _target.AdjustHealth(-_self.myStats.strength, Elementals.Physical);
		yield return null;
	}

	protected IEnumerator LifeTap(CombatController _self)
	{
        EffectTools.SpawnEffect("blue blast", _self.transform.position, 1);
		int _tapped = _self.AdjustHealth(-Mathf.Clamp(5,0,_self.currentHealth -1),Elementals.Void);
		_self.AdjustMana(_tapped);
		yield return null;
	}

	protected IEnumerator SiphonSoul(CombatController _target, CombatController _self)
	{
		//_self.AdjustMana(-manaCostDictionary["siphon soul"]);
		print("siphon at: " + _target);

		if(_target != null)
		{
			int _hpRecover = _target.AdjustHealth(-(_self.myStats.luck),Elementals.Unlife);
			print("recover: " +_hpRecover);

			//print(_self.AdjustHealth(-5,Elementals.None) + " was given");
			_self.AdjustHealth(Mathf.Clamp(_hpRecover,0,int.MaxValue),Elementals.Fire);// new Elementals[] { Elementals.Light });

			//StartCoroutine(Eat(_self,_hpRecover));
		}
		yield return null;
	}

	protected IEnumerator Smite(CombatController _target, StatBlock.Race _targetRace, int _constant)
	{
		EffectTools.SpawnEffect("light cloud",lastClick,1);

		if (_target != null)
		{

			//_self.AdjustMana(-manaCostDictionary["smite unlife"]);
			//EffectTools.SpawnEffect("punch",lastClick,1);
			int _bonusDamage = (_target.myStats.race.HasFlag(_targetRace)) ? 1 : 0;


			_target.AdjustHealth(-(_constant + _bonusDamage),Elementals.None);
		}
		yield return null;
	}

	protected IEnumerator Fireball(Vector3 _centerPos,CombatController _self)
	{
		//_self.AdjustMana(-manaCostDictionary[_costName]);
		var _sprite = EffectTools.SpawnEffect(FIREBALL,_centerPos,0.6f);
		float _blastDiameter = 1;// Mathf.Ceil((float)_self.myStats.intelligence / 5);
		StartCoroutine(EffectTools.PingPongSize(_sprite.transform,Vector3.zero,Vector3.one * _blastDiameter,0.5f,1));

		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.right * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.left * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.up * _blastDiameter * 0.5f,Color.white,5,false);
		Debug.DrawLine(_sprite.transform.position,_sprite.transform.position + Vector3.down * _blastDiameter * 0.5f,Color.white,5,false);

		foreach(Collider2D _col in Physics2D.OverlapCircleAll(_centerPos,_blastDiameter * 0.5f))
		{
			print(_col.transform.name + " was hit by firebazll");
			var _cc = _col.GetComponent<CombatController>();
			if (_cc != null)
			{
				_cc.AdjustHealth(-_self.myStats.intelligence, Elementals.Fire);
			}
		}
		yield return null;

	}

	protected IEnumerator MassExplosion(Vector3 _centerPos,CombatController _self)
	{
		//_self.AdjustMana(-manaCostDictionary["mass explosion"]);

		StartCoroutine(Fireball(lastClick + Vector3.left,_self));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Fireball(lastClick,_self));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Fireball(lastClick + Vector3.right,_self));

		yield return null;
	}

	protected IEnumerator Heal(CombatController _target, int _baseStat)
	{
		//_self.AdjustMana(-manaCostDictionary[_costName]);

		if(_target != null)
		{
			_target.AdjustHealth(Mathf.CeilToInt(_baseStat),Elementals.Light);

		}
		EffectTools.SpawnEffect("heal_circle",lastClick, 1);// lastClick,1);

		yield return null;
	}

	protected IEnumerator ManaDrain(CombatController _target, CombatController _self)
	{

		if(_target != null)
		{
			int _manaRecover = _target.AdjustMana(Mathf.Clamp(-(_self.myStats.luck),int.MinValue,0));

			_self.AdjustMana(Mathf.Clamp(_manaRecover,0,int.MaxValue));

			yield return null;
			_target.AdjustMana(-2);
		}

		yield return null;
	}

	protected IEnumerator BulkUp(CombatController _self)
	{
		EffectTools.SpawnEffect("fist up",lastClick,1);

		var _buff = new Buff(BULK_UP,"strength",1, GetBuffIcon("strenght_up"), StatBlock.StackType.Build_Up,1);
		AddBuff(_buff, _self);

		yield return null;
	}

	protected IEnumerator DivineLuck(CombatController _self)
	{
		EffectTools.SpawnEffect("luck up",lastClick,1);


		var _buff = new Buff(DIVINE_LUCK,"luck", 3, GetBuffIcon("divine_luck"), StatBlock.StackType.Pick_Most_Potential,2);
		AddBuff(_buff,_self);
		yield return null;
	}

	protected IEnumerator Regeneration(CombatController _target, int _constant)
	{
		EffectTools.SpawnEffect("plusses",lastClick + Vector3.forward,1);

		if(_target != null)
		{
			//EffectTools.SpawnEffect("heal_circle",lastClick,1);

			var _buff = new Buff(REGENERATION,HEAL,3, GetBuffIcon("yellow_pluss"), StatBlock.StackType.Pick_Most_Potential, _constant);
			AddBuff(_buff,_target);
		}
		yield return null;
	}

	protected IEnumerator MassHeal(CombatController _self)
	{

		var _dt = new CombatController[CombatController.turnOrder.Count];
		CombatController.turnOrder.CopyTo(_dt);
		foreach(CombatController _cc in _dt)
		{
			if(_cc != this)
				StartCoroutine(Heal(_cc, _self.myStats.luck));
		}


		CombatController.turnOrder.ForEach(x => x.StartCoroutine(Heal(x,_self.myStats.luck)));
		CombatController.turnOrder.ForEach(x => EffectTools.SpawnEffect("heal_circle",x.transform.position + Vector3.forward,1));

		//EffectTools.SpawnEffect("heal_circle",lastClick,1);// lastClick,1);

		/*
		foreach(CombatController _cc in CombatController.turnOrder)
		{
			StartCoroutine(Heal(_cc,_self.myStats.luck));
		}
		*/
		yield return null;
	}

	protected IEnumerator Eat(CombatController _target, int _amount = 0)
	{
		_target.AdjustHealth(_amount, Elementals.Light);// new Elementals[] { Elementals.Light });

		yield return null;
	}

	protected IEnumerator Focus(CombatController _self)
	{
		EffectTools.SpawnEffect("blue_sparkles",_self.transform.position,1);
		_self.AdjustMana(_self.myStats.intelligence);
		yield return new WaitForSeconds(1);
	}

	protected IEnumerator SpotWeakness(CombatController _target, CombatController _self)
	{
		if (_target == null)
		{
			//_self.AdjustMana(-manaCostDictionary["spot weakness"]);
			yield break;
		}

		StartCoroutine( DisplayCritAreas(_self, _target));
		yield return null;
	}

	protected IEnumerator TimeWarp(CombatController _self)
	{
		EffectTools.SpawnEffect(TIME_WARP,lastClick,1);
		var _buff = new Buff(TIME_WARP,"extra turn",2, GetBuffIcon("pluss_time"),StatBlock.StackType.Stack_Self,1);
		AddBuff(_buff,_self);
		yield return null;
	}

	/// <summary>
	/// Displays crit areas of all enemies if no parameter is given, otherwise, display the crit area of the enemy given.
	/// </summary>
	/// <param name="_self"></param>
	protected IEnumerator DisplayCritAreas(CombatController _self,CombatController _target = null)
	{
		/*
		if(_manacost == 0)
			_self.AdjustMana(-manaCostDictionary["keen sight"]);
		else
			_self.AdjustMana(_manacost);
		*/

		var _checks = (_target == null)? CombatController.turnOrder: new List<CombatController>() { _target };

		foreach (CombatController _cc in _checks)
		{
			var _critArea = _cc.transform.Find("$CritArea");
			print("area: " + _critArea);
			if (_critArea != null)
			{
				var _critImage = _critArea.GetComponent<SpriteRenderer>();
				StartCoroutine(EffectTools.BlinkImage(_critImage,Color.white,5.5f,10));
			}
		}

		yield return null;

	}

	protected IEnumerator Spook(CombatController _target,CombatController _self)
	{
		yield return StartCoroutine(EffectTools.BlinkImage(_self.transform.GetComponent<SpriteRenderer>(),new Color(1,1,1,0),1,1));
		_target.AdjustHealth(-_self.myStats.intelligence, Elementals.Unlife);

	}
}
