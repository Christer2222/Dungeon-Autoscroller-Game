using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestStaticVariables : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		LevelUpScreen.abilityPointsToSpend = LevelUpScreen.traitPointsToSpend = 0;
		CombatController.ClearAllValues();
    }
}
