#if UNITY_EDITOR
using UnityEngine;

public class Cheats : MonoBehaviour
{
    private bool addedDebugAbilities;

    private void Start()
    {
        string c = "<color=#AA20AA>";
        string cend = "</color>";
        print($"{c}3:{cend} add banana:1 | {c}4:{cend} player -2 hp| {c}5:{cend} traitpoints++ | {c}6:{cend} addDebugAbilities() | {c}7:{cend} kill all enemies | {c}8:{cend} playerXP + some | {c}9:{cend} force encounter");
        if (DebugController.debugAbilities)
            AddDebugAbilities();

        if (DebugController.useBonusPoints)
            LevelUpScreen.instance.traitPointsToSpend += 10;
    }

    void AddDebugAbilities()
    {
        if (addedDebugAbilities) return;
            addedDebugAbilities = true;

        CombatController.playerCombatController.MyStats.abilities.AddRange(AbilityCollection.DebugAbilities());
        CombatController.playerCombatController.RefreshAbilityList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad3))
            PlayerInventory.instance.AddItemToInventory(new PlayerInventory.ItemQuantity() { amount = 1, item = Items.Banana });

        if (Input.GetKeyDown(KeyCode.Keypad4))
            CombatController.playerCombatController.AdjustHealth(-2, AbilityInfo.Elementals.None, AbilityInfo.ExtraData.none);

        if (Input.GetKeyDown(KeyCode.Keypad5))
            LevelUpScreen.instance.traitPointsToSpend++;

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
            CombatController.playerCombatController.AdjustPlayerXP(5 * CombatController.playerCombatController.MyStats.level);

        if (Input.GetKeyDown(KeyCode.Keypad9))
            EncounterController.instance.currentGameState = EncounterController.GameState.Starting_Battle;//.encounterTimer = 0;

    }
}
#endif