using System;

public class TutorialData
{
	public static SequenceFlags currentSequence = 0;

	[Flags]
	public enum SequenceFlags
	{
		None					= 0x00000000,
		/*
		OpenedAbilities			= 0x00000001,
		SelectedAbility			= 0x00000002,
		UsedAbilitySelf			= 0x00000004,
		UsedAbilityOpponent		= 0x00000008,

		OpenedInventory			= 0x00000010,
		SelectedItem			= 0x00000020,
		SelectedConsumableItem	= 0x00000040,
		SelectedTargetableItem	= 0x00000080,
		SelectedAutoEquipment	= 0x00000100,
		SelectedSlotEquipment	= 0x00000200,

		Fled					= 0x00000400,

		InspectedEnemy			= 0x00000800,

		LeveledUp				= 0x00001000,
		SpentAbilityPoints		= 0x00002000,
		SelectedNewAbility		= 0x00004000,
		SelectedNewClass		= 0x00008000,

		EncounteredEnemy		= 0x00010000,
		GottenItemDrops			= 0x00020000,

		FoundEnvironmentGoodie	= 0x00040000,
		FoundEnvironmentTrap	= 0x00080000,
		FoundEnvironmentSecret	= 0x00100000,
		
		//FoundShop				= 0x00200000,
		//PurchasedItem			= 0x00400000,
		//SoldItem				= 0x00800000,



		AbilityRelated			= OpenedAbilities | SelectedAbility | UsedAbilitySelf | UsedAbilitySelf,
		InventoryRelated		= OpenedInventory | SelectedItem | SelectedConsumableItem | SelectedTargetableItem | SelectedAutoEquipment | SelectedSlotEquipment,
		LevelUpRelated			= LeveledUp | SpentAbilityPoints | SelectedNewAbility | SelectedNewClass,
		CombatRelated			= EncounteredEnemy | GottenItemDrops,
		Environmentrelated		= FoundEnvironmentGoodie | FoundEnvironmentTrap | FoundEnvironmentSecret,
		//ShopRelated			= FoundShop | PurchasedItem | SoldItem,
		*/
	}
}
