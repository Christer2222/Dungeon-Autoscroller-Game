using UnityEngine;
using System;

public class AnimationTextParser
{
	private static Sprite[] spriteArray;

	public static Sprite[] ParseDocument(TextAsset _textAsset)
	{
		if (_textAsset == null)
		{
			return null;
		}

		if (spriteArray == null)
		{
			spriteArray = Resources.LoadAll<Sprite>("Sprites/Enemies/EnemySpriteSheet");
		}

		string[] _entries = _textAsset.text.Split(new string[]{Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
		Sprite[] _result = new Sprite[_entries.Length];

		for (int i = 0; i < _entries.Length; i++)
		{
			if (_entries[i].Contains("_"))
			{
				_entries[i] = _entries[i].Remove(0, _entries[i].IndexOf("_") + 1);
			}

			_result[i] = spriteArray[int.TryParse(_entries[i], out int x)? x : 0];
		}



		return _result;
	}
}
