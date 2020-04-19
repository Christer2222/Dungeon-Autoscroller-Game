#if UNITY_EDITOR
using UnityEngine;

public class Cheats : MonoBehaviour
{
    private bool addedDebugAbilities;

    private void Start()
    {
        string c = "<color=#AA20AA>";
        string cend = "</color>";
        print($"{c}5:{cend} traitpoints++ | {c}6:{cend} addDebugAbilities() | {c}7:{cend} kill all enemies | {c}8:{cend} playerXP + some | {c}9:{cend} force encounter");
        if (DebugController.debugAbilities)
            AddDebugAbilities();

        if (DebugController.useBonusPoints)
            LevelUpScreen.traitPointsToSpend += 10;
    }

    void AddDebugAbilities()
    {
        if (addedDebugAbilities) return;
            addedDebugAbilities = true;

        CombatController.playerCombatController.myStats.abilities.AddRange(AbilityClass.DebugAbilities());
        CombatController.playerCombatController.RefreshAbilityList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
            LevelUpScreen.traitPointsToSpend++;

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            AddDebugAbilities();
        }

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