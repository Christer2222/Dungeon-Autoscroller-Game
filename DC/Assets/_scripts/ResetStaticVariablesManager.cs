using UnityEngine;

public class ResetStaticVariablesManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		LevelUpScreen.abilityPointsToSpend = LevelUpScreen.traitPointsToSpend = 0;
		CombatController.ClearAllValues();
    }
}
