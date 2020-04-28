using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetStaticVariablesManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		LevelUpScreen.traitPointsToSpend = 0;
		CombatController.ClearAllValues();
		EncounterController.currentGameState = EncounterController.GameState.Walking;// .ResetEncounterTimer();//encounterTimer = EncounterController.;
		EncounterController.buffTimer = 1;

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
