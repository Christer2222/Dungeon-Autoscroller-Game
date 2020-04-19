public class DebugController
{
#if UNITY_EDITOR
	public static bool debugAbilities = false;
	public static bool useBonusPoints;
#else
	public static bool debugAbilities = false;
	public static bool useBonusPoints;
#endif
}
