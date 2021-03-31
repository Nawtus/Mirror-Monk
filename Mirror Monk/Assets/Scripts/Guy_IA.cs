using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Guy_IA : MonoBehaviour
{
    [Header("Guy Settings")]
    public int health;
    [SerializeField] protected int dieLayer;

    [SerializeField] protected bool isLookLeft;

    public float followCrono;

    public GuyStates currentState;

    [HideInInspector] public GameObject player;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SortingGroup sortingGroup;


    public enum GuyStates
    {
        Null,
        Idle,
        Follow,
        Attack,
        Reloud,
        Kick,
        Wait,
        WalkAroundPlayer,
        Backstab,
        Hit,
        DropItem,
        LeaveItem,
        Die,
    }



    public void Flip(Vector3 target)
    {
        float diference = target.x - transform.position.x;
        if (diference > 0 && isLookLeft == true)
        {
            isLookLeft = !isLookLeft;
            transform.Rotate(0, 180, 0);
        }
        else if (diference < 0 && isLookLeft == false)
        {
            isLookLeft = !isLookLeft;
            transform.Rotate(0, 180, 0);
        }
    }



    public void TakeDamage(int damage)
    {
        health -= damage;
    }



    public virtual void Die()
    {
        gameObject.tag = "Die";
        sortingGroup.sortingOrder = dieLayer;
        health = 0;
    }



    protected void Follow(PolyNavAgent agent, float distanceFollow, bool front)
    {
        if (followCrono < 2f)
        {
            agent.Stop();
            anim.SetBool("Walking", false);
            followCrono += Time.deltaTime;
        }
        else
        {
            followCrono = 2f;
            anim.SetBool("Walking", true);

            if (front)
            {
                if (player.GetComponent<Character_Behaviour>().isLookLeft)
                {
                    agent.SetDestination(new Vector2(player.transform.position.x - (distanceFollow * 0.8f), player.transform.position.y));
                }
                else
                {
                    agent.SetDestination(new Vector2(player.transform.position.x + (distanceFollow * 0.8f), player.transform.position.y));
                }
            }
            else
            {
                if (player.GetComponent<Character_Behaviour>().isLookLeft)
                {
                    agent.SetDestination(new Vector2(player.transform.position.x + (distanceFollow * 0.8f), player.transform.position.y));
                }
                else
                {
                    agent.SetDestination(new Vector2(player.transform.position.x - (distanceFollow * 0.8f), player.transform.position.y));
                }
            }
        }
    }
}