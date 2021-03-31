    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character_Controller : MonoBehaviour
{
    [HideInInspector] public Character_Behaviour behaviour;

    void Awake()
    {
        behaviour = GetComponent<Character_Behaviour>();
    }



    private void FixedUpdate()
    {
        if (behaviour.currentState == Character_Behaviour.PlayerStates.Walk || behaviour.currentState == Character_Behaviour.PlayerStates.Idle)
        {
            behaviour.Move(behaviour.direction, Time.deltaTime);
        }
        else
        {
            behaviour.rig.velocity = new Vector2(0, 0);
        }
    }



    void Update()
    {
        TestInput();
        UpdateInput();
    }



    private void UpdateInput()
    {
        if (behaviour != null)
        {
            behaviour.direction = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical"),
            };
            
            if (behaviour.currentState != Character_Behaviour.PlayerStates.HitMelee || behaviour.currentState != Character_Behaviour.PlayerStates.HitRanged)
            {
                if (Input.GetKey(behaviour.reflectInput))
                    behaviour.SetState(Character_Behaviour.PlayerStates.Reflect);

                if (behaviour.canDodge)
                {
                    if (Input.GetKey(behaviour.dodgeInput))
                    {
                        if (behaviour.direction.x != 0)
                        {
                            behaviour.SetState(Character_Behaviour.PlayerStates.DashDodge);
                        }
                        else
                        {
                            behaviour.SetState(Character_Behaviour.PlayerStates.Dodge);
                        }
                        behaviour.canDodge = false;
                        behaviour.dodgeTimer = 0;
                    }
                }
                else
                {
                    behaviour.CowndownDodge();
                }
            }
        }
    }



    void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DieWithItem"))
        {
            if (Input.GetKeyDown(behaviour.pickWeaponInput))
            {
                if (behaviour.withKnife)
                {
                    behaviour.DropItem();
                }
                collision.GetComponent<Melee_Behaviour>().LeaveKnife();
                behaviour.SetState(Character_Behaviour.PlayerStates.PickKnife);
            }
        }
        else if (collision.CompareTag("KnifeItem"))
        {
            if (Input.GetKeyDown(behaviour.pickWeaponInput))
            {
                if (behaviour.withKnife)
                {
                    behaviour.DropItem();
                }
                behaviour.SetState(Character_Behaviour.PlayerStates.PickKnife);
                Destroy(collision.gameObject);
            }
        }
    }
}
