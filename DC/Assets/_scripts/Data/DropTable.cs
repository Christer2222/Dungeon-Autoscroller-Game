using System.Collections.Generic;

public class DropTable
{
	public static List<ItemDrop> none;
	public static List<ItemDrop> urnDrop				= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 10, 10), new ItemDrop(1000, Items.apple, 1, 1), };
	public static List<ItemDrop> nosemanDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 2, 4), new ItemDrop(200, Items.apple, 1, 1), };
	public static List<ItemDrop> eyeballDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 2, 3), };
	
	public static List<ItemDrop> lightElementalDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 5, 6), new ItemDrop(200, Items.apple, 1, 1), };
	public static List<ItemDrop> fireElementalDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 4, 6), };
	public static List<ItemDrop> airElementalDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 4, 6), };
	public static List<ItemDrop> waterElementalDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 4, 6), };
	
	public static List<ItemDrop> earthElementalDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 4, 6), new ItemDrop(200, Items.apple, 1, 1), new ItemDrop(200, Items.stick, 1, 2), };
	public static List<ItemDrop> druidDrop				= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 1, 2), new ItemDrop(200, Items.apple, 1, 1), new ItemDrop(200, Items.stick, 1, 2), };
	public static List<ItemDrop> harpyDrop				= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 1, 1),  };
	public static List<ItemDrop> blueEyeballDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 5, 8), };

	public static List<ItemDrop> snowmanDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 10, 11), new ItemDrop(1000, Items.stick, 2, 2), };
	public static List<ItemDrop> poisionousSpiderDrop	= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 10, 10), new ItemDrop(50, Items.goldRing, 1, 1), };
	public static List<ItemDrop> fleshGolemDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 1, 2), new ItemDrop(200, Items.orange, 1, 1), };
	public static List<ItemDrop> guardianDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 15, 25), new ItemDrop(200, Items.orange, 1, 1), };

	public static List<ItemDrop> giantDrop				= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 20, 25), new ItemDrop(50, Items.goldRing, 1, 2), };
	public static List<ItemDrop> deathSpiderDrop		= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 2, 8), };
	public static List<ItemDrop> ghostDrop				= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 5, 18), };
	public static List<ItemDrop> stoneGolemDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 1, 2), new ItemDrop(200, Items.orange, 1, 1), };

	public static List<ItemDrop> steelGolemDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 5, 10), new ItemDrop(200, Items.banana, 1, 1), };
	public static List<ItemDrop> moonManDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 30, 40), new ItemDrop(200, Items.banana, 1, 1), };
	public static List<ItemDrop> goldGolemDrop			= new List<ItemDrop>() { new ItemDrop(1000, Items.goldCoin, 50, 70), };
}
