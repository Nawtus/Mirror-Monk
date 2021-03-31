using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged_Kick : MonoBehaviour
{
    [Range(1, 10)]
    public int damage = 1;
    public Character_Behaviour.TakeDamageAnimData playerAnimData;



    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Character_Behaviour playerBehaviour = col.gameObject.GetComponentInParent<Character_Behaviour>();

            float differenceX = col.bounds.center.x - GetComponent<Collider2D>().bounds.center.x;

            playerBehaviour.TakeDamage(damage, differenceX, "Ranged", playerAnimData);
        }
    }
}
