public class Options
{
#if UNITY_EDITOR
	public static bool finishedTutorial = true;//false;
#else
	public static bool finishedTutorial = false;
#endif
}
