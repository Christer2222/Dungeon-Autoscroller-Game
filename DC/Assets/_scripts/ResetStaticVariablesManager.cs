using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetStaticVariablesManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		LevelUpScreen.abilityPointsToSpend = LevelUpScreen.traitPointsToSpend = 0;
		CombatController.ClearAllValues();
		ForwardMover.encounterTimer = 2;
		ForwardMover.buffTimer = 1;

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
