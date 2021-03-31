using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Reflect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PunchGuy_PunchHB"))
        {
            collision.GetComponentInParent<Melee_Behaviour>().VerifyHealth();
        }
        else if (collision.gameObject.CompareTag("PistolGuy_GunHB"))
        {
            collision.GetComponentInParent<Ranged_Behaviour>().VerifyDraw();
        }
        else if (collision.gameObject.CompareTag("KnifeGuy_KnifeHB"))
        {
            collision.GetComponentInParent<Melee_Behaviour>().SetState(Guy_IA.GuyStates.DropItem);
        }
    }
}
