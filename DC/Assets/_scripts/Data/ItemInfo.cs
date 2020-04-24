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


	public static ItemInfo Stick { get; } = new ItemInfo("Stick", 2, ItemType.Craftable, $"En Garde!\nCan be used to craft various items.", punch, 2);
	public static ItemInfo GoldSword { get; } = new ItemInfo("Gold Sword", 35, ItemType.Craftable | ItemType.OneHanded, $"Notvery useful!\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", punch, 2);
	
	
	public static ItemInfo SteelSword { get; } = new ItemInfo("Steel Sword", 15, ItemType.OneHanded, $"Sharp\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", punch, 3);
	public static ItemInfo AdamantineSword { get; } = new ItemInfo("Adamantine Sword", 77, ItemType.OneHanded, $"Like steel, just better!\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", punch, 6);
	public static ItemInfo PoisonedDagger { get; } = new ItemInfo("Poisoned Dagger", 32, ItemType.OneHanded, $"Poison.\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", poisionBite, 2);


	public static ItemInfo SteelBroadSword { get; } = new ItemInfo("Steel Broad Sword", 30, ItemType.TwoHanded, $"Big sword.\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", punch, 4);
	public static ItemInfo AdamantineBroadSword { get; } = new ItemInfo("Adamantine Broad Sword", 99, ItemType.TwoHanded, $"Biggest sword.\nDeals {ITEM_ACTIVE_CONSTANT} damage when held and used as a weapon.", punch, 8);


	public static ItemInfo GoldCoin { get; } = new ItemInfo("Gold Coin", 1, ItemType.Valueable, $"Cash, Moola, Money, whatever you wanna call it, people want it.");
	public static ItemInfo Goldbar { get; } = new ItemInfo("Gold Bar", 25, ItemType.Valueable, $"This should be worht quite a bit.");


	public static ItemInfo Headband { get; } = new ItemInfo("Headband", 10, ItemType.Helmet, 1, 0, $"Protects your noggin.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo SteelHelmet { get; } = new ItemInfo("Steel Helmet", 15, ItemType.Helmet, 2, 0, $"Not a bucket.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo GladiatorsHelmet { get; } = new ItemInfo("Gladiators Helmet", 15, ItemType.Helmet, 3, 0, $"Makes you feel like you're going into battle.\nGives you {ITEM_DEFENSE} defense.");


	public static ItemInfo LeatherVest { get; } = new ItemInfo("Leather Vest", 7, ItemType.Chestplate, 1, 1, $"Bare minimum.\nGives you {ITEM_DEFENSE} defense, and {ITEM_MAGIC_DEFENSE} magic defense.");
	public static ItemInfo SteelChestplate { get; } = new ItemInfo("Steel Chestplate", 15, ItemType.Chestplate, 3, 0, $"Feels sturdy.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo AdamantineChestplate { get; } = new ItemInfo("Adamantine Chestplate", 127, ItemType.Chestplate, 5, 0, $"Bare minimum... if you're going to fight a dragon.\nGives you {ITEM_DEFENSE} defense.");


	public static ItemInfo LeatherPants { get; } = new ItemInfo("Leather Pants", 7, ItemType.Chestplate, 1, 1, $"50% better than shorts.\nGives you {ITEM_DEFENSE} defense, and {ITEM_MAGIC_DEFENSE} magic defense.");
	public static ItemInfo SteelLeggings { get; } = new ItemInfo("Steel Leggings", 14, ItemType.Chestplate, 2, 0, $"Kneecap protectors.\nGives you {ITEM_DEFENSE} defense.");
	public static ItemInfo AdamantineLeggings { get; } = new ItemInfo("Adamantine Leggings", 117, ItemType.Chestplate, 4, 0, $"Also called ☼Leggings☼.\nGives you {ITEM_DEFENSE} defense.");

	public static ItemInfo GoldRing { get; } = new ItemInfo("Gold Ring", 15,  ItemType.Acessory, 0, 1, $"Fancy.\nGives you {ITEM_MAGIC_DEFENSE} magic defense.");
	public static ItemInfo StrikeRing { get; } = new ItemInfo("Strike Ring", 35, ItemType.Acessory, $"A magic ring.\nGives you acces to {ITEM_ACTIVE_ABILITY_LIST}.", forcePunch, 1);
	public static ItemInfo BoltRing { get; } = new ItemInfo("Bolt Ring", 45, ItemType.Acessory, 0, 0, $"A magic ring with a line symbol.\nGives you acces to {ITEM_ACTIVE_ABILITY_LIST}.", new List<Ability>() { fireball, thunderbolt }, new List<int>() { 3, 3 });
	public static ItemInfo MeteorRing { get; } = new ItemInfo("Meteor Ring", 55, ItemType.Acessory, $"A magic ring.\nGives you acces to {ITEM_ACTIVE_ABILITY_LIST}.", meteorShower, 1);


	public enum ItemType
	{
		Garbage = 1,
		Valueable = 2, //same as garbage, but just used to declare that it is worth something
		Consumable = 4,
		Targetable = 8,
		Craftable = 16,

		Helmet = 32,
		Chestplate = 64,
		Leggings = 128,
		Boots = 256,
		//Cape = 512,
		//Gauntlets = 1024,
		Acessory = 2048,

		OneHanded = 4096,
		TwoHanded = 8192,


		Equipment = Helmet | Chestplate | Leggings | Boots | /*Cape | Gauntlets |*/ Acessory | OneHanded | TwoHanded, //check against this to see if item is equiped
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


		public ItemInfo(string _name, int _price, ItemType _type, int _defense, int _magicDefense, string _description, Ability _activeAbility, int _activeconstant, Ability _passiveAbility = null, int _passiveConstant = 0) : this(_name, _price,  _type, _defense, _magicDefense, _description, new List<Ability>() { _activeAbility }, new List<int> { _activeconstant }, new List<Ability>() { _passiveAbility }, new List<int> { _passiveConstant }) { }

		/// <summary>
		/// Usually equipment with 0-1 active and/or passive ability.
		/// </summary>
		//public ItemInfo(string _name, int _price, ItemType _type, int _defense, int _magicDefense, string _description, Ability _activeAbility = null, int _activeconstant = 0, Ability _passiveAbility = null, int _passiveConstant = 0) : this(_name, _price,  _type, _defense, _magicDefense, _description, new List<Ability>() { _activeAbility }, new List<int> { _activeconstant }, new List<Ability>() { _passiveAbility }, new List<int> { _passiveConstant }) { }

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

			_description = _description.Replace(ITEM_ACTIVE_CONSTANT, GetConstantsList(activeConstants));
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
