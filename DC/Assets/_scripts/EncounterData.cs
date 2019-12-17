using UnityEngine;
using System.Collections.Generic;
using AbilityInfo;

public class EncounterData : AbilityScript
{
	#region enemy stat blocks
	public static StatBlock ghostBlock = new StatBlock(StatBlock.Race.Undead, "Ghost", 8, 8, 6, 0, 1, 3, 1, 1, new List<Ability> { punch }, _weaknesses: Elementals.Light, _absorbs: Elementals.Unlife, _immunities: Elementals.Physical, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock nosemanBlock = new StatBlock(StatBlock.Race.Demon, "Noseman", 2, 0, 10, 0, 1, 1, 1, 1, new List<Ability> { punch }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock eyeballBlock = new StatBlock(StatBlock.Race.Demon, "Eyeball", 7, 7, 2, 1, 1, 2, 1, 2, new List<Ability> { punch, manaDrain }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock lightElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Light Elemental", 10, 2, 2, 0, 1, 2, 1, 2, new List<Ability> { punch, heal }, _absorbs: Elementals.Light, _weaknesses: Elementals.Void, _aiType: StatBlock.AIType.Coward);
	public static StatBlock airElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Air Elemental", 7, 5, 2, 1, 1, 2, 1, 2, new List<Ability> { punch }, _absorbs: Elementals.Air, _weaknesses: Elementals.Earth, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock earthElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Earth Elemental", 15, 5, 2, 1, 1, 2, 1, 2, new List<Ability> { punch }, _absorbs: Elementals.Earth, _weaknesses: Elementals.Air, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock fireElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Fire Elemental", 5, 5, 2, 1, 1, 2, 1, 2, new List<Ability> { punch, fireball }, _absorbs: Elementals.Fire, _weaknesses: Elementals.Water, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock waterElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Water Elemental", 12, 5, 2, 0, 1, 2, 1, 2, new List<Ability> { punch, regeneration }, _absorbs: Elementals.Water, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock harpyBlock = new StatBlock(StatBlock.Race.Demon, "Harpy", 10, 5, 2, 0, 1, 2, 1, 2, new List<Ability> { punch, bulkUp }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock druidBlock = new StatBlock(StatBlock.Race.Elf, "Druid", 5, 10, 2, 0, 1, 2, 1, 2, new List<Ability> { punch, heal, regeneration }, _resistances: Elementals.Earth, _aiType: StatBlock.AIType.Coward);
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

		public Encounter(StatBlock _monsterBL = default, StatBlock _monsterBM = default, StatBlock _monsterBR = default,
						 StatBlock _monsterTL = default, StatBlock _monsterTM = default, StatBlock _monsterTR = default,
						 EncounterLocation _encounterLocation = default, int _level = 99)
		{
			monsterBL = _monsterBL;
			monsterBM = _monsterBM;
			monsterBR = _monsterBR;
			monsterTL = _monsterTL;
			monsterTM = _monsterTM;
			monsterTR = _monsterTR;

			encounterLocation = _encounterLocation;
			level = _level;
		}
	}

	public static readonly Encounter[] encounterTable = new Encounter[]
	{
		new Encounter(_level: 0, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock),

		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock, _monsterBR: nosemanBlock),
		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: harpyBlock),
		
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTR: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: eyeballBlock.Clone(), _monsterTR: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: nosemanBlock.Clone(), _monsterTL: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBL: waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: lightElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: druidBlock),

		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterTM: ghostBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterTL: ghostBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterBM: ghostBlock),
		new Encounter(_level: 6, _encounterLocation: EncounterLocation.None, _monsterTR: ghostBlock),

	};

	public static readonly Vector3[] offsetTable = new Vector3[]
	{
		new Vector3(0.66f,0,0),
		new Vector3(0,0,0),
		new Vector3(-0.66f,0,0),
		new Vector3(1,1,0),
		new Vector3(0.33f,1,0),
		new Vector3(-0.33f,1,0),
	};
}
