#if UNITY_EDITOR
using UnityEngine;

public class Cheats : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            var a = CombatController.turnOrder.FindAll(x => x != CombatController.playerCombatController);
            foreach (var v in a) v.AdjustHealth(-999,AbilityInfo.Elementals.None, AbilityInfo.ExtraData.none);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
            CombatController.playerCombatController.AdjustPlayerXP(5 * CombatController.playerCombatController.myStats.level);

        if (Input.GetKeyDown(KeyCode.Keypad9))
            ForwardMover.encounterTimer = 0;
    }
}
#endif