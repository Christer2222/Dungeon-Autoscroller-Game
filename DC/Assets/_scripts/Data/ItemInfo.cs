using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Items : AbilityClass
{
	private const string ITEM_ACTIVE_CONSTANT = "½";
	private const string ITEM_DEFENSE = "¼";
	private const string ITEM_MAGIC_DEFENSE = "¾";
	private const string ITEM_ACTIVE_ABILITY_LIST = "¹";

	
	public static ItemInfo Apple { get; } = new ItemInfo("Apple", 4, ItemType.Consumable, $"Apparently wards against doctors.\nHeals {ITEM_ACTIVE_CONSTANT}hp.", heal, 2);
	public static ItemInfo Orange { get; } = new ItemInfo("Orange", 6, ItemType.Consumable, $"Named after a color, or has a color named after it?\nHeals {ITEM_ACTIVE_CONSTANT}hp.", heal, 4);
	public static ItemInfo Banana { get; } = new ItemInfo("Banana", 8, ItemType.Consumable, $"Is this a snack?\nHeals {ITEM_ACTIVE_CONSTANT}hp.", heal, 6);

	public static ItemInfo Stick { get; } = new ItemInfo("Stick", 2, ItemType.Craftable | ItemType.OneHanded, $"En Garde!\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.\nCan also be used to craft various items.", punch, 2);

	public static ItemInfo GoldCoin { get; } = new ItemInfo("Gold Coin", 1, ItemType.Valueable, $"Cash, Moola, Money, whatever you wanna call it, people want it.");
	public static ItemInfo Goldbar { get; } = new ItemInfo("Gold Bar", 25, ItemType.Valueable, $"This should be worht quite a bit.");

	public static ItemInfo Headband { get; } = new ItemInfo("Headband", 10, ItemType.Headgear, 1, 0, $"Protects your noggin.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo SteelHelmet { get; } = new ItemInfo("Steel Helmet", 15, ItemType.Headgear, 2, 0, $"Not a bucket.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo GoldRing { get; } = new ItemInfo("Gold Ring", 15,  ItemType.Acessory, 0, 1, $"Fancy.\nGives you {ITEM_MAGIC_DEFENSE} magic defense.");
	public static ItemInfo StrikeRing { get; } = new ItemInfo("Strike Ring", 35, ItemType.Acessory | ItemType.Targetable, $"A magic ring.\nGives you acces to {ITEM_ACTIVE_ABILITY_LIST}.", forcePunch, 1);
	
	public enum ItemType
	{
		Garbage = 1,
		Valueable = 2, //same as garbage, but just used to declare that it is worth something
		Consumable = 4,
		Targetable = 8,
		Craftable = 16,

		Headgear = 32,
		Chestplate = 64,
		Leggings = 128,
		Boots = 256,
		Cape = 512,
		Gauntlets = 1024,
		Acessory = 2048,

		OneHanded = 4096,
		TwoHanded = 8192,


		Equipment = Headgear | Chestplate | Leggings | Boots | Cape | Gauntlets | Acessory, //check against this to see if item is equiped
	}

	public class ItemInfo
	{
		public string name;
		public List<Ability> passiveAbilities;
		public List<int> passiveConstants;
		public List<Ability> activeAbilities;
		public List<int> activeConstants;
		public ItemType type;
		public Sprite sprite;
		public int price;
		public string description;
		public int defense;
		public int magicDefense;

		/// <summary>
		/// Usually valuables.
		/// </summary>
		public ItemInfo(string _name, int _price, ItemType _type, string _description) : this(_name, _price,  _type, 0, 0, _description, null, null, null, null) { }

		/// <summary>
		/// Usually consumables. Should never have passive effects.
		/// </summary>
		public ItemInfo(string _name, int _price, ItemType _type, string _description, Ability _activeAbility, int _activeConstant) : this(_name, _price,  _type, 0, 0, _description, new List<Ability>() { _activeAbility }, new List<int> { _activeConstant }, null, null) { }
		/*
		/// <summary>
		/// Usually equipment with 0-1 active and/or passive ability.
		/// </summary>
		public ItemInfo(string _name, int _price, ItemType _type, int _defense, int _magicDefense, string _description, Ability _activeAbility = null, int _activeconstant = 0, Ability _passiveAbility = null, int _passiveConstant = 0) : this(_name, _price,  _type, _defense, _magicDefense, _description, new List<Ability>() { _activeAbility }, new List<int> { _activeconstant }, new List<Ability>() { _passiveAbility }, new List<int> { _passiveConstant }) { }
		*/
		/// <summary>
		///	Usually equipment.
		/// </summary>
		public ItemInfo(string _name, int _price, ItemType _type, int _bonusDefense, int _bonusMagicDefense, string _description, List<Ability> _activeAbilities = null, List<int> _activeConstants = null, List<Ability> _passiveAbilities = null, List<int> _passiveConstants = null)
		{
			if (_activeAbilities == null) _activeAbilities = new List<Ability>();
			if (_activeConstants == null) _activeConstants = new List<int>();
			if (_passiveAbilities == null) _passiveAbilities = new List<Ability>();
			if (_passiveConstants == null) _passiveConstants = new List<int>();



			name = _name;
			price = _price;
			type = _type;
			defense = _bonusDefense;
			magicDefense = _bonusMagicDefense;
			activeAbilities = _activeAbilities;
			activeConstants = _activeConstants;
			passiveAbilities = _passiveAbilities;
			passiveConstants = _passiveConstants;

			if (activeAbilities.Count != activeConstants.Count) throw new System.ArgumentException("Length of the active abilities list is not the same as the length of the active constant list on item "+name+".");
			if (passiveAbilities.Count != passiveConstants.Count) throw new System.ArgumentException("Length of the passive abilities list is not the same as the length of the passive constant list on item " + name + ".");

			_description = _description.Replace(ITEM_ACTIVE_CONSTANT, GetConstantsList(_activeConstants));
			_description = _description.Replace(ITEM_DEFENSE, _bonusDefense.ToString());
			_description = _description.Replace(ITEM_MAGIC_DEFENSE, _bonusMagicDefense.ToString());
			_description = _description.Replace(ITEM_ACTIVE_ABILITY_LIST, GetAbilityNameList(activeAbilities));
			description = _description;

			string _path = "Sprites/Items/AnimationTexts/";
			string _standardizedName = _name.ToLower().Replace(" ", "_");
			string _extention = "_sprite_index.txt";
			var textAssetContaintinSpriteData = Resources.Load<TextAsset>(_path + _standardizedName + _extention);

			if (textAssetContaintinSpriteData == null)
				textAssetContaintinSpriteData = AnimationTextParser.GetNewTextAssetOrAddNewToAssetDatabase("Assets/Resources/" + _path + _standardizedName + _extention);

			var _sprites = AnimationTextParser.ParseDocument(textAssetContaintinSpriteData, AnimationTextParser.Type.Item);
			sprite = (_sprites != null)? (_sprites.Length > 0)? _sprites[0]: null: null; //if the spritesheet is found, but no sprites are given
		}

		string GetConstantsList(List<int> _constants)
		{
			string _constantsToString = "";
			for (int i = 0; i < _constants.Count; i++)
			{
				_constantsToString += _constants[i];
				if (i + 1 < _constants.Count) _constantsToString += ", ";
			}
			return _constantsToString;
		}

		string GetAbilityNameList(List<Ability> _abilities)
		{
			string _abilityNamesToString = "";
			for (int i = 0; i < _abilities.Count; i++)
			{
				_abilityNamesToString += _abilities[i].name;
				if (i + 1 < _abilities.Count) _abilityNamesToString += ", ";
			}
			return _abilityNamesToString;

		}
	}
}


public class ItemDrop
{
	public ItemDrop(float _permilleToDrop, Items.ItemInfo _item, int _minCount, int _maxCount)
	{
		permille = _permilleToDrop;
		item = _item;
		maxCount = _maxCount;
		minCount = _minCount;
	}

	public float permille;
	public Items.ItemInfo item;
	public int maxCount;
	public int minCount;
}
