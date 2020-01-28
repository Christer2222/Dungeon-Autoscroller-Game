using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityInfo;

public class Items : AbilityClass
{
	public static ItemInfo apple = new ItemInfo(heal, 2, 0);
	public static ItemInfo orange = new ItemInfo(heal, 4, 1);
	public static ItemInfo banana = new ItemInfo(heal, 6, 2);

	public class ItemInfo
	{
		public Ability ability;
		public int constant;
		public int spriteIndex;

		public ItemInfo(Ability _ability, int _constant, int _spriteIndex)
		{
			ability = _ability;
			constant = _constant;
			spriteIndex = _spriteIndex;

			string _path = "Sprites/Items/AnimationTexts/";
			string _standardizedName = _ability.name.ToLower().Replace(" ", "_");
			string _extention = "_sprite_index";
			var a = Resources.Load<TextAsset>(_path + _standardizedName + _extention);
			var _sprites = AnimationTextParser.ParseDocument(a, AnimationTextParser.Type.effect);
		}
	}
}


public class ItemDrop
{
	public ItemDrop(float _permilleToDrop, Items.ItemInfo _item)
	{
		permille = _permilleToDrop;
		item = _item;
	}

	public float permille;
	public Items.ItemInfo item;
}
