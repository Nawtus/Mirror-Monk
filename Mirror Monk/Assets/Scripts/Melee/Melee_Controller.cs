using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Controller : MonoBehaviour
{
    [HideInInspector] public Melee_Behaviour behaviour;

    void Awake()
    {
        behaviour = GetComponent<Melee_Behaviour>();
    }



    void Update()
    {
        behaviour.Flip(behaviour.player.transform.position);

        if (behaviour != null && !behaviour.player.GetComponent<Character_Behaviour>().die && !this.gameObject.CompareTag("Die") && !this.gameObject.CompareTag("DieWithItem"))
        {         
            if (behaviour.combatOrganize.nearGuy == this.gameObject)
            {
                behaviour.InFront();
            }
            else
            {
                switch (behaviour.randomlyAction)
                {
                    case 0:
                        behaviour.SetState(Guy_IA.GuyStates.Wait);
                        break;

                    case 1:
                        behaviour.SetState(Guy_IA.GuyStates.WalkAroundPlayer);
                        break;

                    case 2:
                        behaviour.SetState(Guy_IA.GuyStates.WalkAroundPlayer);
                        break;

                    case 3:
                        behaviour.Backstab();
                        break;
                }
            }
        }
    }
}
