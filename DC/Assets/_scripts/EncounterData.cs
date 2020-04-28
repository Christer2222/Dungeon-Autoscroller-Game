using UnityEngine;
using System.Collections.Generic;
using AbilityInfo;

public class EncounterData : AbilityCollection
{   /* 	/----------------------------\
	*	| Space to line up abilities |
	*///\----------------------------/
	#region enemy stat blocks		
	//----------------------------------------------------------------------------------------------------------hp,--mp,--------lv,xp, st, de, in, lu------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static StatBlock urnBlock = new StatBlock(StatBlock.Race.Construct, "Urn",							001, 000,		00, 0, 00, 00, 00, 00, new List<Ability> { wobble }, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.urnDrop);
	public static StatBlock nosemanBlock = new StatBlock(StatBlock.Race.Demon, "Noseman",						002, 000,		01, 0, 01, 01, 01, 01, new List<Ability> { punch }, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.nosemanDrop);
	public static StatBlock eyeballBlock = new StatBlock(StatBlock.Race.Demon, "Eyeball",						007, 007,		02, 0, 01, 02, 01, 02, new List<Ability> { punch, manaDrain }, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.eyeballDrop);
	public static StatBlock lightElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Light Elemental",	010, 002,		02, 0, 01, 02, 01, 02, new List<Ability> { punch, heal }, _absorbs: Elementals.Light, _weaknesses: Elementals.Void, _aiType: StatBlock.AIType.Coward, _drops: DropTable.lightElementalDrop);
	public static StatBlock fireElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Fire Elemental",		005, 005,		02, 0, 02, 02, 01, 02, new List<Ability> { punch, fireball }, _absorbs: Elementals.Fire, _weaknesses: Elementals.Water, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.fireElementalDrop);
	public static StatBlock airElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Air Elemental",		007, 005,		02, 0, 01, 02, 01, 02, new List<Ability> { punch }, _absorbs: Elementals.Air, _weaknesses: Elementals.Earth, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.airElementalDrop);
	public static StatBlock waterElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Water Elemental",	012, 005,		02, 0, 02, 02, 01, 02, new List<Ability> { punch, regeneration }, _absorbs: Elementals.Water, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.waterElementalDrop);
	public static StatBlock earthElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Earth Elemental",	015, 005,		02, 0, 01, 02, 01, 02, new List<Ability> { punch }, _absorbs: Elementals.Earth, _weaknesses: Elementals.Air, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.earthElementalDrop);
	public static StatBlock druidBlock = new StatBlock(StatBlock.Race.Elf, "Druid",								005, 010,		02, 0, 01, 02, 01, 02, new List<Ability> { punch, heal, regeneration }, _resistances: Elementals.Earth, _aiType: StatBlock.AIType.Coward, _drops: DropTable.druidDrop);
	public static StatBlock harpyBlock = new StatBlock(StatBlock.Race.Demon, "Harpy",							010, 005,		02, 0, 01, 02, 01, 02, new List<Ability> { punch, bulkUp }, _weaknesses: Elementals.Electricity, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.harpyDrop);
	public static StatBlock blueEyeballBlock = new StatBlock(StatBlock.Race.Demon, "Blueball",					012, 007,		03, 0, 02, 03, 02, 03, new List<Ability> { punch, manaDrain, curse }, _aiType: StatBlock.AIType.Smart, _drops: DropTable.blueEyeballDrop);
	public static StatBlock snowmanBlock = new StatBlock(StatBlock.Race.Elemental, "Snowman",					009, 012,		03, 0, 01, 02, 01, 02, new List<Ability> { freezingStrike }, _absorbs: Elementals.Ice, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Coward, _drops: DropTable.snowmanDrop);
	public static StatBlock poisionousSpider = new StatBlock(StatBlock.Race.Animal, "Poisionous Spider",		005, 008,		03, 0, 02, 02, 03, 02, new List<Ability> { poision }, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.poisionousSpiderDrop);
	public static StatBlock fleshGolemBlock = new StatBlock(StatBlock.Race.Construct, "Flesh Golem",			030, 010,		04, 0, 03, 02, 01, 01, new List<Ability> { doubleKick, bulkUp }, _weaknesses: Elementals.Fire | Elementals.Ice, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.fleshGolemDrop);
	public static StatBlock guardianBlock = new StatBlock(StatBlock.Race.Human, "Guardian",						020, 020,		05, 0, 05, 04, 01, 03, new List<Ability> { forcePunch, magicShield }, _aiType: StatBlock.AIType.Smart, _defense: 1, _drops: DropTable.guardianDrop);
	public static StatBlock giantBlock = new StatBlock(StatBlock.Race.Human, "Giant",							030, 000,		05, 0, 08, 04, 01, 01, new List<Ability> { punch }, _aiType: StatBlock.AIType.Dumb, _defense: 1, _drops: DropTable.giantDrop);
	public static StatBlock deathSpider = new StatBlock(StatBlock.Race.Animal, "Death Spider",					015, 018,		05, 0, 04, 03, 05, 03, new List<Ability> { poision, poisionBite }, _immunities: Elementals.Poision, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.deathSpiderDrop);
	public static StatBlock ghostBlock = new StatBlock(StatBlock.Race.Undead, "Ghost",							008, 008,		06, 0, 01, 03, 01, 01, new List<Ability> { punch }, _weaknesses: Elementals.Light, _absorbs: Elementals.Unlife, _immunities: Elementals.Physical, _aiType: StatBlock.AIType.Dumb, _drops: DropTable.ghostDrop);
	public static StatBlock stoneGolemBlock = new StatBlock(StatBlock.Race.Construct, "Stone Golem",			045, 010,		06, 0, 06, 02, 01, 01, new List<Ability> { doubleKick, bulkUp, hardenSkin }, _weaknesses: Elementals.Water, _immunities: Elementals.Light, _aiType: StatBlock.AIType.Dumb, _defense: 2, _drops: DropTable.stoneGolemDrop);
	public static StatBlock steelGolemBlock = new StatBlock(StatBlock.Race.Construct, "Steel Golem",			060, 010,		08, 0, 10, 04, 03, 01, new List<Ability> { doubleKick, bulkUp, hardenSkin }, _weaknesses: Elementals.Water | Elementals.Electricity, _immunities: Elementals.Light | Elementals.Poision, _aiType: StatBlock.AIType.Dumb, _defense: 4, _magicDefense: -1, _drops: DropTable.steelGolemDrop);
	public static StatBlock moonManBlock = new StatBlock(StatBlock.Race.Alien, "Moonman",						030, 999,		10, 0, 02, 03, 02, 03, new List<Ability> { meteorShower }, _resistances: Elementals.Void, _aiType: StatBlock.AIType.Genious, _drops: DropTable.moonManDrop);
	public static StatBlock goldGolemBlock = new StatBlock(StatBlock.Race.Construct, "Gold Golem",				075, 030,		10, 0, 15, 05, 08, 03, new List<Ability> { doubleKick, bulkUp, hardenSkin, fireball }, _immunities: Elementals.Light | Elementals.Poision, _aiType: StatBlock.AIType.Smart, _defense: 6, _magicDefense: -2, _drops: DropTable.goldGolemDrop);
	#endregion

	public enum EncounterLocation
	{
		None = 0,
		Forest = 1,
		Graveyard = 2,
		Dungeon = 4,
		Town = 8,
		Sea = 16,
		Space = 32,
		Hell = 64,
	}


	public class Encounter
	{
		public StatBlock monsterBL, monsterBM, monsterBR, monsterTL, monsterTM, monsterTR;
		public EncounterLocation encounterLocation;
		public int level;
		public bool randomBottom, randomTop;

		public Encounter(StatBlock _monsterBL = null, StatBlock _monsterBM = null, StatBlock _monsterBR = null,
						 StatBlock _monsterTL = null, StatBlock _monsterTM = null, StatBlock _monsterTR = null,
						 EncounterLocation _encounterLocation = default, int _level = 99, bool _randomTop = true, bool _randomBottom = true)
		{
			monsterBL = _monsterBL;
			monsterBM = _monsterBM;
			monsterBR = _monsterBR;
			monsterTL = _monsterTL;
			monsterTM = _monsterTM;
			monsterTR = _monsterTR;

			encounterLocation = _encounterLocation;
			level = _level;

			randomBottom = _randomBottom;
			randomTop = _randomTop;
		}
	}

	public static readonly Encounter[] encounterTable = new Encounter[]
	{
		new Encounter(_level: 0, _encounterLocation: EncounterLocation.None, _monsterBL: urnBlock, _monsterBM: urnBlock,  _monsterBR: urnBlock),

		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock),
		
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: harpyBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: eyeballBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: eyeballBlock, _monsterTR: eyeballBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: nosemanBlock, _monsterTL: eyeballBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: fireElementalBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: earthElementalBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: waterElementalBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: airElementalBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: lightElementalBlock),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: druidBlock),

		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: snowmanBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterTM: blueEyeballBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterTM: fireElementalBlock, _monsterTR: fireElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBR: fireElementalBlock, _monsterTL: fireElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: earthElementalBlock, _monsterBR: earthElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: waterElementalBlock, _monsterBR: waterElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterTL: airElementalBlock, _monsterTR: airElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: waterElementalBlock, _monsterTR: fireElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: waterElementalBlock, _monsterBR: earthElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBL: fireElementalBlock, _monsterTM: airElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterTM: airElementalBlock, _monsterBR: waterElementalBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock, _monsterBR: nosemanBlock, _monsterTR: eyeballBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: eyeballBlock, _monsterBR: eyeballBlock, _monsterTR: eyeballBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterBM: druidBlock, _monsterBL: druidBlock),
		new Encounter(_level: 3, _encounterLocation: EncounterLocation.None, _monsterTM: harpyBlock, _monsterTL: harpyBlock),

		new Encounter(_level: 4, _encounterLocation: EncounterLocation.None, _monsterBM: fleshGolemBlock),
		new Encounter(_level: 4, _encounterLocation: EncounterLocation.None, _monsterBM: snowmanBlock, _monsterTM: blueEyeballBlock),
		new Encounter(_level: 4, _encounterLocation: EncounterLocation.None, _monsterTM: blueEyeballBlock, _monsterTL: blueEyeballBlock),
		new Encounter(_level: 4, _encounterLocation: EncounterLocation.None, _monsterTM: blueEyeballBlock, _monsterTL: eyeballBlock, _monsterTR: eyeballBlock, _randomTop: false),

		new Encounter(_level: 5, _encounterLocation: EncounterLocation.None, _monsterBM: giantBlock),
		new Encounter(_level: 5, _encounterLocation: EncounterLocation.None, _monsterBM: guardianBlock),

		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterTM: ghostBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterBM: ghostBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterBM: stoneGolemBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterBM: guardianBlock, _monsterBR: guardianBlock),


		new Encounter(_level: 7, _encounterLocation: EncounterLocation.None, _monsterBM: stoneGolemBlock, _monsterBL: stoneGolemBlock),
		new Encounter(_level: 7, _encounterLocation: EncounterLocation.None, _monsterBM: giantBlock, _monsterBL: giantBlock, _monsterBR: giantBlock),
		new Encounter(_level: 7, _encounterLocation: EncounterLocation.None, _monsterBM: guardianBlock, _monsterBR: guardianBlock, _monsterBL: guardianBlock),

		new Encounter(_level: 8, _encounterLocation: EncounterLocation.None, _monsterBM: steelGolemBlock),

		new Encounter(_level: 9, _encounterLocation: EncounterLocation.None, _monsterBM: steelGolemBlock, _monsterBL: steelGolemBlock),

		new Encounter(_level: 10, _encounterLocation: EncounterLocation.None, _monsterBM: moonManBlock),
		new Encounter(_level: 10, _encounterLocation: EncounterLocation.None, _monsterBM: goldGolemBlock),
		new Encounter(_level: 10, _encounterLocation: EncounterLocation.None, _monsterBM: steelGolemBlock, _monsterBL: steelGolemBlock, _monsterBR: steelGolemBlock),
	};

	public static readonly Vector3[] offsetTable = new Vector3[]
	{
		new Vector3(1.33f,0,0), //BR
		new Vector3(0,0,0), //BM
		new Vector3(-1.33f,0,0), //BL

		new Vector3(1.33f,1,0), //TR
		new Vector3(0,1,0), //TM
		new Vector3(-1.33f,1,0), //TL
	};


	public static Encounter RandomizeEncounter(Encounter _inputEncounter)
	{
		int randomizeFactor = 10;

		if (_inputEncounter.randomBottom)
		{
			var _list = Shuffle(new List<StatBlock>() { _inputEncounter.monsterBL, _inputEncounter.monsterBM, _inputEncounter.monsterBR }, randomizeFactor); //shuffle
			_inputEncounter.monsterBL = _list[0];
			_inputEncounter.monsterBM = _list[1];
			_inputEncounter.monsterBR = _list[2];
		}

		if (_inputEncounter.randomTop)
		{
			var _list = Shuffle(new List<StatBlock>() { _inputEncounter.monsterTL, _inputEncounter.monsterTM, _inputEncounter.monsterTR }, randomizeFactor); //shuffle
			_inputEncounter.monsterTL = _list[0];
			_inputEncounter.monsterTM = _list[1];
			_inputEncounter.monsterTR = _list[2];
		}

		return _inputEncounter;
	}

	static List<StatBlock> Shuffle(List<StatBlock> _originalList, int _randomizeFactor)
	{
		for (int i = 0; i < _randomizeFactor; i++) //shuffle around the list equal to the randomize factor
		{
			int _index = Random.Range(0, _originalList.Count); //take a random selected element
			_originalList.Add(_originalList[_index]); //add it to the back
			_originalList.RemoveAt(_index); //then remove it from the original position
		}

		return _originalList;
	}
}
