using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged_Controller : MonoBehaviour
{
    [HideInInspector] public Ranged_Behaviour behaviour;

    void Awake()
    {
        behaviour = GetComponent<Ranged_Behaviour>();
    }



    private void Update()
    {
        if (behaviour != null && !behaviour.player.GetComponent<Character_Behaviour>().die && !this.gameObject.CompareTag("Die"))
        {
            if (behaviour.currentState == Guy_IA.GuyStates.Reloud)
            {
                return;
            }
            else
            {
                if (behaviour.currentState == Guy_IA.GuyStates.Follow)
                {
                    behaviour.SetState(Guy_IA.GuyStates.Follow);
                }

                if (behaviour.shoots < 1)
                {
                    behaviour.SetState(Guy_IA.GuyStates.Reloud);
                }
            }

            if (behaviour.pistolKick)
            {
                behaviour.anim.SetLayerWeight(1, 1);

                if (behaviour.draw && Mathf.Abs(transform.position.x - behaviour.player.transform.position.x) <= 1f && Mathf.Abs(transform.position.y - behaviour.player.transform.position.y) <= 0.05f)
                {
                    behaviour.SetState(Guy_IA.GuyStates.Kick);
                }
            }
        }
    }
}
