using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : AbilityData
{
	protected static Vector3 lastClick;

	protected Vector3 randomVector3
	{
		get
		{
			return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}
	}

	public class Buff
	{
		public Buff(string _name, string _function, int _turns, Sprite _buffIcon, StatBlock.StackType _stackType, float _constant, CombatController _target = null, bool _shouldBeDisplyed = true)
		{
			name = _name;
			function = _function;
			turns = _turns;
			constant = _constant;
			target = _target;
			stackType = _stackType;
			buffIcon = _buffIcon;
			shouldBeDisplayed = _shouldBeDisplyed;
		}

		public string name;
		public string function;
		public CombatController target;
		public float constant;
		public int turns;
		public StatBlock.StackType stackType;
		public Sprite buffIcon;
		public bool shouldBeDisplayed;
	}

	protected void AddBuff(Buff _buff, CombatController _target)
	{
		var _same = _target.myStats.buffList.Find(x => x.name == _buff.name);
		switch(_buff.stackType)
		{
			case StatBlock.StackType.Pick_Most_Potent:
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
	}

	protected IEnumerator Punch(CombatController _target, int _damage, Elementals _element = Elementals.Physical)
	{
		EffectTools.SpawnEffect(PUNCH, lastClick, 1);
		if (_target != null) _target.AdjustHealth(-Mathf.Max(_damage, 0), _element);
		yield return null;
	}

	/*
	protected IEnumerator Punch(CombatController _target, CombatController _self)
	{
		EffectTools.SpawnEffect(PUNCH, lastClick,1);
		if(_target != null) _target.AdjustHealth(-Mathf.Max(_self.myStats.strength,0), Elementals.Physical);
		yield return null;
	}
	*/

	protected IEnumerator DoubleKick(Vector3 _centerPos, CombatController _target, CombatController _self)
	{
		yield return StartCoroutine(Punch(_target,_self.myStats.strength));
		_target = null;
		yield return new WaitForSeconds(2);
		var _hit = CheckIfHit(_centerPos);
		if (_hit.transform != null)
		{
			_target = _hit.transform.GetComponent<CombatController>();
		}
		yield return StartCoroutine(Punch(_target, _self.myStats.strength));
	}


	protected IEnumerator WildPunch(Vector3 _centerPos, CombatController _self)
	{
		CombatController _target = null;
		var _hit = CheckIfHit(_centerPos + randomVector3);
		if (_hit.transform != null)
		{
			_target = _hit.transform.GetComponent<CombatController>();
		}
		yield return StartCoroutine(Punch(_target, _self.myStats.strength + 2));
		yield return null;
	}

	protected IEnumerator ForcePunch(CombatController _target, CombatController _self)
	{
		yield return StartCoroutine(Punch(_target, _self.myStats.strength + 2, Elementals.Air));
		yield return null;
	}

	protected IEnumerator TiltSwing(CombatController _target, CombatController _self)
	{
		float _r1 = Random.Range(0, 2);
		int _r = (int)((_r1-0.5f) * 2);
		print("r1: " + _r1 + "r: " + _r);
		yield return StartCoroutine(Punch(_target, _self.myStats.strength + _r));
		yield return null;
	}

	protected IEnumerator ChaosThesis(Vector3 _centerPos, CombatController _self)
	{
		int _r = Random.Range(0,3);
		CombatController _target = null;
		var _hit = CheckIfHit(_centerPos);
		if (_hit.transform != null)
		{
			_target = _hit.transform.GetComponent<CombatController>();
		}

		print("chose: " + _r);
		if (_r == 0)
		{
			yield return StartCoroutine(ManaDrain(_target,_self, 2));
		}
		else if (_r == 1)
		{
			yield return StartCoroutine(Fireball(_centerPos, _self, 2));
		}
		else if (_r == 2)
		{
			yield return StartCoroutine(Heal(_target, _self.myStats.luck));
		}

		yield return null;
	}

	protected IEnumerator LifeTap(CombatController _self)
	{
        EffectTools.SpawnEffect("blue blast", _self.transform.position, 1);
		int _tapped = _self.AdjustHealth(-Mathf.Min(5, _self.currentHealth -1),Elementals.Void);
		_self.AdjustMana(_tapped);
		yield return null;
	}

	protected IEnumerator SiphonSoul(CombatController _target, CombatController _self)
	{
		//_self.AdjustMana(-manaCostDictionary["siphon soul"]);
		print("siphon at: " + _target);

		if(_target != null)
		{
			int _hpRecover = _target.AdjustHealth(-Mathf.Max(_self.myStats.luck,0),Elementals.Unlife);
			print("recover: " +_hpRecover);

			//print(_self.AdjustHealth(-5,Elementals.None) + " was given");
			_self.AdjustHealth(Mathf.Max(_hpRecover,0),Elementals.Fire);// new Elementals[] { Elementals.Light });

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


			_target.AdjustHealth(-Mathf.Max(_constant + _bonusDamage, 0),Elementals.None);
		}
		yield return null;
	}

	protected IEnumerator Fireball(Vector3 _centerPos, CombatController _self, int _bonus = 0)
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
				_cc.AdjustHealth(-Mathf.Max(_self.myStats.intelligence + _bonus,0), Elementals.Fire);
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
			_target.AdjustHealth(Mathf.CeilToInt(Mathf.Max(_baseStat,0)),Elementals.Light);

		}

		if (lastClick != null)
			EffectTools.SpawnEffect("heal_circle",lastClick, 1);// lastClick,1);

		yield return null;
	}

	protected IEnumerator ManaDrain(CombatController _target, CombatController _self, int _bonus = 0)
	{

		if(_target != null)
		{
			int _manaRecover = 0;

			_manaRecover = _target.AdjustMana(-Mathf.Max(2 + (Mathf.CeilToInt((float)_self.myStats.luck/2) + _bonus),0));


			_self.AdjustMana(Mathf.Max(_manaRecover,0));

			yield return null;
			//_target.AdjustMana(-2);
		}

		yield return null;
	}

	protected IEnumerator BulkUp(CombatController _self)
	{
		EffectTools.SpawnEffect("fist up",lastClick,1);

		var _buff = new Buff(BULK_UP,"strength",2, TryGetBuffIcon("pluss_strength"), StatBlock.StackType.Build_Up,1);
		AddBuff(_buff, _self);

		yield return null;
	}

	protected IEnumerator Debulk(CombatController _target)
	{
		if (_target != null)
		{
			var _buff = new Buff(DEBULK, "strength", 3, TryGetBuffIcon("pluss_strength"), StatBlock.StackType.Pick_Most_Potent, -2);
			AddBuff(_buff, _target);
		}

		yield return null;
	}

	protected IEnumerator DivineLuck(CombatController _self)
	{
		EffectTools.SpawnEffect("luck up",lastClick,1);


		var _buff = new Buff(DIVINE_LUCK,"luck", 3, TryGetBuffIcon("divine_luck"), StatBlock.StackType.Pick_Most_Potent,2);
		AddBuff(_buff,_self);
		yield return null;
	}

	protected IEnumerator Regeneration(CombatController _target, int _constant)
	{
		EffectTools.SpawnEffect("plusses",lastClick + Vector3.forward,1);

		if(_target != null)
		{
			//EffectTools.SpawnEffect("heal_circle",lastClick,1);

			var _buff = new Buff(REGENERATION,HEAL,3, TryGetBuffIcon("yellow_pluss"), StatBlock.StackType.Pick_Most_Potent, Mathf.Max(_constant,0));
			AddBuff(_buff,_target);
		}
		yield return null;
	}

	protected IEnumerator MassHeal(CombatController _self)
	{
		/*
		var _dt = new CombatController[CombatController.turnOrder.Count];
		CombatController.turnOrder.CopyTo(_dt);
		foreach(CombatController _cc in _dt)
		{
			if(_cc != this)
				StartCoroutine(Heal(_cc, Mathf.Max(_self.myStats.luck, 0)));
		}
		*/
		Vector3? q = new Vector3(0, 0, 0);

		CombatController.turnOrder.ForEach(x => {
			lastClick = x.transform.position;
			x.StartCoroutine(Heal(x, Mathf.Max(_self.myStats.luck, 0)));
		});
		//CombatController.turnOrder.ForEach(x => EffectTools.SpawnEffect("heal_circle",x.transform.position + Vector3.forward,1));

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
		_self.AdjustMana(Mathf.Max(_self.myStats.intelligence, 0));
		yield return null;// new WaitForSeconds(1);
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
		var _buff = new Buff(TIME_WARP,"extra turn",2, TryGetBuffIcon("pluss_time"),StatBlock.StackType.Stack_Self,1);
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
