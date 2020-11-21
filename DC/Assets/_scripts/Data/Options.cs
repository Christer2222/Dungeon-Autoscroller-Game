using UnityEngine;
public class Options
{
	public static KeyCode abilitiesHotkey = KeyCode.Alpha1;
	public static KeyCode itemsHotkey = KeyCode.Alpha2;
	public static KeyCode fleeHotkey = KeyCode.Alpha3;
	public static KeyCode inspectHotkey = KeyCode.Alpha4;

	public static KeyCode levelUpHotkey = KeyCode.C;

	public static KeyCode blockKey = KeyCode.B;

	public static KeyCode mapKey = KeyCode.M;

	public static KeyCode acceptKey = KeyCode.Space;

#if UNITY_EDITOR
	public static bool finishedTutorial = true;//false;
#else
	public static bool finishedTutorial = false;
#endif
}
