using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private Sprite blockSprite;
    private const float BLOCKING_COOLDOWN = 1f;
    private bool isUnderCooldown;
    private float blockTimer = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        blockSprite = BuffIcons.TryGetBuffIcon(14);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUnderCooldown)
        {
            if (Input.GetKeyDown(Options.blockKey))
            {
                isUnderCooldown = true;
                StartCoroutine(BlockCoolDown());
                StartCoroutine(BlockBonus());
            }
        }

    }

    IEnumerator BlockCoolDown()
    {
        yield return new WaitForSeconds(BLOCKING_COOLDOWN);

        isUnderCooldown = false;
    }

    IEnumerator BlockBonus()
    {
        EffectTools.SpawnEffect(blockSprite, transform.position, blockTimer).transform.SetParent(transform);
        
        CombatController.playerCombatController.MyStats.baseDefense++;
        CombatController.playerCombatController.MyStats.baseMagicDefense++;
        yield return new WaitForSeconds(blockTimer);
        CombatController.playerCombatController.MyStats.baseDefense--;
        CombatController.playerCombatController.MyStats.baseMagicDefense--;
    }
}
