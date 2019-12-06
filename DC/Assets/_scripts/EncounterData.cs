using UnityEngine;
using System.Collections.Generic;

public class EncounterData : AbilityData
{
	#region enemy stat blocks
	public static StatBlock ghostBlock = new StatBlock(StatBlock.Race.Undead, "Ghost", 2, 2, 1, 0, 1, 3, 1, 1, new List<string> { SPOOK }, _weaknesses: Elementals.Light, _absorbs: Elementals.Unlife, _immunities: Elementals.Physical, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock nosemanBlock = new StatBlock(StatBlock.Race.Demon, "Noseman", 2, 0, 4, 0, 1, 1, 1, 1, new List<string> { PUNCH }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock eyeballBlock = new StatBlock(StatBlock.Race.Demon, "Eyeball", 7, 7, 2, 1, 1, 2, 1, 2, new List<string> { PUNCH, MANA_DRAIN }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock lightElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Light Elemental", 10, 2, 2, 0, 1, 2, 1, 2, new List<string> { PUNCH, HEAL }, _absorbs: Elementals.Light, _weaknesses: Elementals.Void, _aiType: StatBlock.AIType.Coward);
	public static StatBlock airElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Air Elemental", 7, 5, 2, 1, 1, 2, 1, 2, new List<string> { PUNCH }, _absorbs: Elementals.Air, _weaknesses: Elementals.Earth, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock earthElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Earth Elemental", 15, 5, 2, 1, 1, 2, 1, 2, new List<string> { PUNCH }, _absorbs: Elementals.Earth, _weaknesses: Elementals.Air, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock fireElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Fire Elemental", 5, 5, 2, 1, 1, 2, 1, 2, new List<string> { PUNCH, FIREBALL }, _absorbs: Elementals.Fire, _weaknesses: Elementals.Water, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock waterElementalBlock = new StatBlock(StatBlock.Race.Elemental, "Water Elemental", 12, 5, 2, 0, 1, 2, 1, 2, new List<string> { PUNCH, REGENERATION }, _absorbs: Elementals.Water, _weaknesses: Elementals.Fire, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock harpyBlock = new StatBlock(StatBlock.Race.Demon, "Harpy", 10, 5, 2, 0, 1, 2, 1, 2, new List<string> { PUNCH, BULK_UP }, _aiType: StatBlock.AIType.Dumb);
	public static StatBlock druidBlock = new StatBlock(StatBlock.Race.Elf, "Druid", 5, 10, 2, 0, 1, 2, 1, 2, new List<string> { PUNCH, HEAL, REGENERATION }, _resistances: Elementals.Earth, _aiType: StatBlock.AIType.Coward);
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

	public struct Encounter
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
		new Encounter(_level: 0, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock.Clone()),

		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: nosemanBlock.Clone(), _monsterBR: nosemanBlock.Clone()),
		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: harpyBlock.Clone()),

		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTR: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: ghostBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: eyeballBlock.Clone(), _monsterTR: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: nosemanBlock.Clone(), _monsterTL: eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: ghostBlock.Clone(), _monsterBL: nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: ghostBlock.Clone(), _monsterBL: nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTR: ghostBlock.Clone(), _monsterBL: nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: lightElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBL: waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: druidBlock.Clone()),

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
