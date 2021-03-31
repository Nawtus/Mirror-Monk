using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Melee_Behaviour : Guy_IA
{
    [Header("MeleeGuy DATA")]
    public GuyData meleeGuyData;

    [Header("MeleeGuy Settings")]
    public int randomlyAction;
    private const int maxActions = 4;
    [Range(0,1)]
    public float distanceFollow;
    public float timeToFollow;

    private Coroutine punchCoroutine;

    [HideInInspector] public float playerRadius;
    public Vector3 randomlyPos;

    [HideInInspector] public CombatOrganize combatOrganize;
    [HideInInspector] public PolyNavAgent agent;



    private void OnEnable()
    {
        Initialize();
    }



    private void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        combatOrganize = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CombatOrganize>();
        agent = GetComponent<PolyNavAgent>();
        randomlyAction = 0;

        currentState = GuyStates.Null;

        health = meleeGuyData.health;
        agent.maxSpeed = meleeGuyData.speed;
        dieLayer = meleeGuyData.dieLayer;
        timeToFollow = meleeGuyData.timeToFollow;

        playerRadius = 3;

        SetState(GuyStates.Follow);
    }




    public void SetState(GuyStates state)
    {
        if (state == currentState)
            return;

        if (state != GuyStates.Follow)
            followCrono = 0;

        if (state != GuyStates.Attack && punchCoroutine != null)
            StopCoroutine(punchCoroutine);
            anim.ResetTrigger("Attack");

        //Start dos estados
        switch (state)
        {      

            case GuyStates.Attack:
                punchCoroutine = StartCoroutine(Attack());
                break;

            case GuyStates.Wait:
                StartCoroutine(Wait(3));
                break;

            case GuyStates.WalkAroundPlayer:
                StartCoroutine(WalkAroundPlayer());
                break;

            case GuyStates.Hit:
                StartCoroutine(Hit(2));
                break;

            case GuyStates.DropItem:
                anim.SetTrigger("ReflectedDown");
                gameObject.tag = "DieWithItem";
                health = 0;
                Melee_Controller controller = GetComponent<Melee_Controller>();
                if (controller != null)
                {
                    controller.enabled = false;
                }
                break;

            case GuyStates.Die:
                anim.SetTrigger("ReflectedDown");
                Die();
                break;
        }

        currentState = state;
    }



    private void Update()
    {
        UpdateState();
    }



    private void UpdateState()
    {
        switch (currentState)
        {
            case GuyStates.Follow:
                Follow(agent, distanceFollow, true);
                break;

            case GuyStates.Backstab:
                Follow(agent, distanceFollow, false);
                break;

            case GuyStates.WalkAroundPlayer:
                if (Vector2.Distance(transform.position, agent.primeGoal) <= 0.2f)
                {
                    randomlyAction = Random.Range(0, maxActions);
                }
                break;

            case GuyStates.LeaveItem:
                Die();
                break;
        }
    }



    public void InFront()
    {
        if (combatOrganize.nearGuy == this.gameObject)
        {
            IsCloseToPlayer(GuyStates.Follow);
        }
    }



    private IEnumerator Attack()
    {
        while (true)
        {
            anim.SetBool("Walking", false);
            agent.Stop();
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(4f);
        }
    }



    private IEnumerator Wait(float timeToWait)
    {
        agent.Stop();
        anim.SetBool("Walking", false);
        yield return new WaitForSeconds(timeToWait);
        randomlyAction = 1;
    }



    private IEnumerator WalkAroundPlayer()
    {
        GetNewPositionAroundPlayer();
        if (!agent.map.PointIsValid(randomlyPos))
        {
            GetNewPositionAroundPlayer();
        } 
        yield return new WaitForSeconds(1f);
        agent.SetDestination(randomlyPos);
        anim.SetBool("Walking", true);
    }



    public void Backstab()
    {
        if (combatOrganize.backstabGuy == this.gameObject)
        {
            IsCloseToPlayer(GuyStates.Backstab);
        }
    }



    private IEnumerator Hit(int damage)
    {
        anim.SetTrigger("Reflected");
        TakeDamage(damage);
        agent.Stop();
        yield return new WaitForSeconds(1.2f);
    }



    public Vector3 GetNewPositionAroundPlayer()
    {
        Vector3 offset = Random.insideUnitCircle * playerRadius;
        Vector3 randomPos = player.transform.position + offset;
        return randomlyPos = randomPos;
    }
   


    private void IsCloseToPlayer(GuyStates direction)
    {
        if ((Mathf.Abs(transform.position.x - player.transform.position.x) > distanceFollow ||
                    Mathf.Abs(transform.position.x - player.transform.position.x) < 0.1f) ||
                    Mathf.Abs(transform.position.y - player.transform.position.y) > 0.1f)
        {
            SetState(direction);
        }
        else
        {
            SetState(GuyStates.Attack);
        }
    }


    public void LeaveKnife()
    {
        anim.SetTrigger("TakingKnife");
        gameObject.tag = "Die";
        SetState(GuyStates.Die);
    }



    public void VerifyHealth()
    {        
        if (health > 1)
        {
            SetState(GuyStates.Hit);
        }
        else
        {
            SetState(GuyStates.Die);
        }
    }



    public override void Die()
    {
        base.Die();
        if (agent != null)
        {
            agent.enabled = false;
        }

        Melee_Controller controller = GetComponent<Melee_Controller>();
        if (controller != null)
        {
            controller.enabled = false;
        }
    }
}
