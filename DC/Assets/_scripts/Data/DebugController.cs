public class DebugController
{
#if UNITY_EDITOR
	public static bool debugAbilities = true;
	public static int bonusAbilityPoints = 50;
#else
	public static bool debugAbilities = false;
	public static int bonusAbilityPoints = 0;
#endif
}
