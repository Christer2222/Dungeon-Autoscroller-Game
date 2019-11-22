using UnityEngine;

public class EncounterData
{
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
		new Encounter(_level: 0, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.nosemanBlock.Clone()),

		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.nosemanBlock.Clone(), _monsterBR: CombatController.nosemanBlock.Clone()),
		new Encounter(_level: 1, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.harpyBlock.Clone()),

		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: CombatController.eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTR: CombatController.eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: CombatController.ghostBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.eyeballBlock.Clone(), _monsterTR: CombatController.eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: CombatController.nosemanBlock.Clone(), _monsterTL: CombatController.eyeballBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: CombatController.ghostBlock.Clone(), _monsterBL: CombatController.nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: CombatController.ghostBlock.Clone(), _monsterBL: CombatController.nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTR: CombatController.ghostBlock.Clone(), _monsterBL: CombatController.nosemanBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.lightElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: CombatController.fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: CombatController.fireElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: CombatController.earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.earthElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBL: CombatController.waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBR: CombatController.waterElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTL: CombatController.airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterTM: CombatController.airElementalBlock.Clone()),
		new Encounter(_level: 2, _encounterLocation: EncounterLocation.None, _monsterBM: CombatController.druidBlock.Clone()),

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
