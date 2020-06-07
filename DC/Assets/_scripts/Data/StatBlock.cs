using System.Collections.Generic;
using System.Linq;
using AbilityInfo;
using UnityEngine;


public class StatBlock
{
	public enum Race
	{
		None,
		Human,
		Dwarf,
		Elf,
		Undead,
		Orc,
		Elemental,
		Demon,
		Construct,
		Alien,
		Animal,
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

	public StatBlock(Race _race,
		string _name,
		int _maxHealth,int _maxMana,
		int _level,int _xp,
		int _strenght,int _dexterity,int _intelligence,int _luck,
		List<Ability> _abilities, //should be List<Ienumerator> to have less human errors
		List<ItemDrop> _drops,
		Elementals _weaknesses = default, Elementals _resistances = default, Elementals _immunities = default, Elementals _absorbs = default,
		AIType _aiType = default,
		List<Buff> _buffs = default,
		int _baseDefense = 0, int _baseMagicDefense = 0
		)
	{
		race = _race;
		name = _name;
		weaknesses = _weaknesses;
		resistances = _resistances;
		immunities = _immunities;
		absorbs = _absorbs;

		maxHealth = currentHealth = _maxHealth;
		maxMana = currentMana = _maxMana;

		level = _level;
		xp = _xp;

		baseStrength = _strenght;
		baseDexterity = _dexterity;
		baseIntelligence = _intelligence;
		baseLuck = _luck;
		abilities = new List<Ability>();
		for(int i = 0; i < _abilities.Count; i++)
		{
			abilities.Add(_abilities[i]);
		}

		drops = _drops;

		aiType = _aiType;

		baseDefense = _baseDefense;
		baseMagicDefense = _baseMagicDefense;

		string _path = "Assets/Resources/Sprites/Enemies/AnimationTexts/";
		string _standardizedName = _name.Replace(" ", "_").ToLower();
		string _extention = "_a_text";

		TextAsset _animationTextAsset = Resources.Load<TextAsset>("Sprites/Enemies/AnimationTexts/" + _standardizedName + "_a_text");
		if (_animationTextAsset == null)
		{
			_animationTextAsset = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase(_path + "EMPTY_" + _standardizedName + _extention + ".txt");
		}

		idleAnimation = AnimationTextParser.ParseDocument(_animationTextAsset, AnimationTextParser.Type.Enemy);

	}

	public Race race;
	public string name;
	public Elementals resistances, weaknesses, immunities, absorbs;
	public int maxHealth, maxMana;
	public int currentHealth, currentMana;
	public int level, xp;

	public int baseStrength, baseDexterity, baseIntelligence, baseLuck;
	public int baseDefense, baseMagicDefense;

	public List<Ability> abilities;
	public AIType aiType;

	public List<Buff> buffList = new List<Buff>();

	public Sprite[] idleAnimation;

	public List<ItemDrop> drops;

	public int Strength
	{
		get
		{
			int _bs = baseStrength;
			foreach(Buff _b in buffList)
			{
				if (_b.traits.Contains(Buff.TraitType.Strength_Constant)) _bs += (int)_b.constant;
				else if (_b.traits.Contains(Buff.TraitType.Strength_Multiplier))
				{
					_bs = (int)(_bs * _b.constant);
					UnityEngine.Debug.Log("base strenght: " + baseStrength + " _bs: " + _bs + " buff constant: " + _b.constant);
				}
			}

			return _bs;
		}
	}

	public int Dexterity
	{
		get
		{
			int _bd = baseDexterity;
			foreach(Buff _b in buffList)
			{
				if(_b.traits.Contains(Buff.TraitType.Dexterity_Constant)) _bd += (int)_b.constant;
				else if (_b.traits.Contains(Buff.TraitType.Dexterity_Multiplier)) _bd = (int)(_bd * _b.constant);

			}

			return _bd;
		}
	}

	public int Intelligence
	{
		get
		{
			int _bi = baseIntelligence;
			foreach(Buff _b in buffList)
			{
				if(_b.traits.Contains(Buff.TraitType.Intelligence_Constant)) _bi += (int)_b.constant;
				else if (_b.traits.Contains(Buff.TraitType.Intelligence_Multiplier)) _bi = (int)(_bi * _b.constant);

			}

			return _bi;
		}
	}

	public int Luck
	{
		get
		{
			int _bl = baseLuck;
			foreach(Buff _b in buffList)
			{
				if(_b.traits.Contains(Buff.TraitType.Luck_Constant)) _bl += (int)_b.constant;
				else if (_b.traits.Contains(Buff.TraitType.Luck_Multiplier)) _bl = (int)(_bl * _b.constant);

			}

			return _bl;
		}
	}

	public int Defense
	{
		get
		{
			int _bd = baseDefense;
			foreach (Buff _b in buffList)
			{
				if (_b.traits.Contains(Buff.TraitType.Physical_Defence_Constant)) _bd += (int)_b.constant;
				//else if (_b.traits.Contains("defense_multiplier")) _bd = (int)(_bd * _b.constant);
			}
			return _bd;
		}
	}

	public int MagicDefense
	{
		get
		{
			int _bm = baseMagicDefense;
			foreach (Buff _b in buffList)
			{
				if (_b.traits.Contains(Buff.TraitType.Magic_Defence_Constant)) _bm += (int)_b.constant;
				//else if (_b.traits.Contains("magicDefense_multiplier")) _bm = (int)(_bm * _b.constant);
			}
			return _bm;
		}
	}

	public string GetToolTipStats()
	{
		string _green = "<color=#00FF00>";
		string _red = "<color=#FF0000>";
		string _end = "</color>";

		bool _alteredDefense = Defense != baseDefense;
		bool _alteredMagicDefense = MagicDefense != baseMagicDefense;


		string _defenseString = (_alteredDefense?((Defense > baseDefense)?_green:_red):string.Empty) + Defense + ((_alteredDefense)? _end: string.Empty);
		string _magicDefenseString = (_alteredMagicDefense?((MagicDefense > baseMagicDefense)?_green:_red):string.Empty) + MagicDefense + ((_alteredMagicDefense)? _end: string.Empty);


		string moves = string.Join(", ", abilities.Select(x => x.name));// abilities.Aggregate
		//string moves = abilities.Select(x => x.name).Aggregate((x, y) => x + ", " + y);
		return $"{name}" +
			$"\n" +
			$"\nHP: {currentHealth}/{maxHealth}" +
			$"\nMP: {currentMana}/{maxMana}" +
			$"\nPhysical Defense: {_defenseString}" +
			$"\nMagic Defense: {_magicDefenseString}" +
			$"\n" +
			$"\nMoves: {moves}";
	}

	public StatBlock Clone()
	{
		var _clone = (StatBlock)MemberwiseClone();
		_clone.buffList = new List<Buff>();
		_clone.abilities = new List<Ability>();
		_clone.abilities.AddRange(abilities);
		//Debug.Log(_clone.abilities.Count);
		//UnityEngine.Debug.Log("name: " + _clone.name + " " + _clone.buffList.GetHashCode());
		return _clone;// MemberwiseClone();
	}
}
