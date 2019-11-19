using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class AbilityScript : MonoBehaviour
{
	protected static Vector3 lastClick;
	protected static Dictionary<string,int> manaCostDictionary = new Dictionary<string,int>()
	{
		{"none",-0 },
		{"fireball",-2 },
		{"mass explosion", -4 },
		{"heal",-2 },
		{"mass heal", -5 },
		{"keen sight", -1 },
		{"smite", -1 },
		{"siphon soul", -3 },
		{"spot weakness", -1 },
		{"regeneration", -1 },
		{"time warp", -10 },
		{"divine luck", -3 },
		{"bulk up", -1 },
		{"mana drain", -3 },


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
		{"punch", AbilityType.attack},
		{"fireball", AbilityType.attack},
		{"mass explosion", AbilityType.attack},
		{"spook", AbilityType.attack},
		{"smite", AbilityType.attack},

		{"siphon soul", AbilityType.recovery | AbilityType.attack},

		{"bulk up", AbilityType.buff },
		{"divine luck", AbilityType.buff },

		{"heal", AbilityType.recovery},
		{"mass heal", AbilityType.recovery},
		{"focus", AbilityType.recovery},
		{"eat", AbilityType.recovery},
		{"regeneration", AbilityType.recovery},
		{"mana drain", AbilityType.recovery},


		{"life tap", AbilityType.misc},
		{"spot weakness", AbilityType.misc },
		{"keen sight", AbilityType.misc},
		{"time warp", AbilityType.misc },
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

	public class Buff
	{
		public Buff(string _name, string _function, int _turns, StatBlock.StackType _stackType, float _constant, CombatController _target = null)
		{
			name = _name;
			function = _function;
			turns = _turns;
			constant = _constant;
			target = _target;
			stackType = _stackType;
		}

		public string name;
		public string function;
		public CombatController target;
		public float constant;
		public int turns;
		public StatBlock.StackType stackType;
	}

	void AddBuff(Buff _buff, CombatController _target)
	{
		var _same = _target.myStats.buffs.Find(x => x.name == _buff.name);
		switch(_buff.stackType)
		{
			case StatBlock.StackType.Pick_Most_Potential:
				if (_same != null)
				{
					if(_same.constant < _buff.constant || (_same.constant == _buff.constant && _same.turns < _buff.turns))
					{
						_target.myStats.buffs.Remove(_same);
						_target.myStats.buffs.Add(_buff);
					}
				}
				else
					_target.myStats.buffs.Add(_buff);

				break;
			case StatBlock.StackType.Pick_Most_Turns:
				if(_same != null)
				{
					if(_same.turns < _buff.turns || (_same.turns == _buff.turns && _same.constant < _buff.constant))
					{
						_target.myStats.buffs.Remove(_same);
						_target.myStats.buffs.Add(_buff);
					}
				}
				else
					_target.myStats.buffs.Add(_buff);
				break;
			case StatBlock.StackType.Stack_Self:
				_target.myStats.buffs.Add(_buff);
				break;
			case StatBlock.StackType.Build_Up:
				if(_same != null)
				{
					_same.turns++;
					_same.constant += _buff.constant;
				}
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




	protected IEnumerator Punch(CombatController _target,CombatController _self)
	{
		EffectTools.SpawnEffect("punch", lastClick,1);
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

		//_self.AdjustMana(-manaCostDictionary["smite unlife"]);
		//EffectTools.SpawnEffect("punch",lastClick,1);
		int _bonusDamage = (_target.myStats.race.HasFlag(_targetRace)) ? 1 : 0;

		print(_constant + _bonusDamage);

		if(_target != null) _target.AdjustHealth(-(_constant + _bonusDamage),Elementals.Physical);
		yield return null;
	}

	protected IEnumerator Fireball(Vector3 _centerPos,CombatController _self,string _costName = "fireball")
	{
		//_self.AdjustMana(-manaCostDictionary[_costName]);
		var _sprite = EffectTools.SpawnEffect("fireball",_centerPos,0.6f);
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

		StartCoroutine(Fireball(lastClick + Vector3.left,_self,"mass explosion"));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Fireball(lastClick,_self,"mass explosion"));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(Fireball(lastClick + Vector3.right,_self,"mass explosion"));

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
		}

		yield return null;
		_target.AdjustMana(-2);
		yield return null;
	}

	protected IEnumerator BulkUp(CombatController _self)
	{
		EffectTools.SpawnEffect("fist up",lastClick,1);


		var _buff = new Buff("bulk up","strength",1,StatBlock.StackType.Build_Up,1);
		AddBuff(_buff, _self);

		yield return null;
	}

	protected IEnumerator DivineLuck(CombatController _self)
	{
		EffectTools.SpawnEffect("luck up",lastClick,1);


		var _buff = new Buff("divine luck","luck",3,StatBlock.StackType.Pick_Most_Potential,2);
		AddBuff(_buff,_self);
		yield return null;
	}

	protected IEnumerator Regeneration(CombatController _target, int _constant)
	{
		EffectTools.SpawnEffect("plusses",lastClick + Vector3.forward,1);

		if(_target != null)
		{
			//EffectTools.SpawnEffect("heal_circle",lastClick,1);

			var _buff = new Buff("regeneration","heal",3,StatBlock.StackType.Pick_Most_Potential, _constant);
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
		EffectTools.SpawnEffect("time warp",lastClick,1);
		var _buff = new Buff("time warped","extra turn",2,StatBlock.StackType.Stack_Self,1);
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
