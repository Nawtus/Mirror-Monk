using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Character_Behaviour : MonoBehaviour
{
    [Header("Player DATA")]
    public PlayerData playerData;

    [Header("Player Settings")]
    public int health;
    [HideInInspector] public float movementSpeed;
    [HideInInspector] private float dashTime;
    [HideInInspector] private float dashDistance;
    [HideInInspector] private int dieLayer;
    public Vector2 direction;

    public bool isLookLeft;
    public bool die;
    public bool withKnife;

    public PlayerStates currentState;

    [HideInInspector] public Rigidbody2D rig;
    [HideInInspector] public Animator anim;
    private SortingGroup sortingGroup;
    [SerializeField] private GameObject knifeDrop;

    public bool canDodge;
    public float dodgeTimer;
    private float dodgeCD;

    [Header("Keyboard Settings")]
    public KeyCode reflectInput;
    public KeyCode dodgeInput;
    public KeyCode pickWeaponInput;

    public enum PlayerStates
    {
        Null,
        Idle,
        Walk,
        Reflect,
        Rest,
        Dodge,
        DashDodge,
        PickKnife,
        HitMelee,
        HitRanged,
        Die,
    }



    private void OnEnable()
    {
        Initialize();
    }



    private void Initialize()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        sortingGroup = GetComponent<SortingGroup>();

        currentState = PlayerStates.Null;

        health = playerData.health;
        movementSpeed = playerData.movementSpeed;
        dashTime = playerData.dashTime;
        dashDistance = playerData.dashDistance;
        dieLayer = playerData.dieLayer;

        canDodge = true;

        dodgeCD = playerData.dodgeCD;
        reflectInput = playerData.reflectInput;
        dodgeInput = playerData.dodgeInput;
        pickWeaponInput = playerData.pickWeaponInput;

        SetState(PlayerStates.Idle);
    }



    public void SetState(PlayerStates state)
    {
        if (state == currentState)
            return;

        if (currentState == PlayerStates.Rest && state == PlayerStates.Reflect)
            return;

        //Não conseguir mudar skill durante algumas delas
        if ((currentState == PlayerStates.Reflect && state == PlayerStates.DashDodge) || 
            (currentState == PlayerStates.Reflect && state == PlayerStates.Dodge) || 
            (currentState == PlayerStates.Dodge && state == PlayerStates.DashDodge) ||
            (currentState == PlayerStates.Dodge && state == PlayerStates.Reflect) ||
            (currentState == PlayerStates.DashDodge && state == PlayerStates.Reflect) ||
            (currentState == PlayerStates.DashDodge && state == PlayerStates.Dodge))
            return;

        //Não cancelar anim dos hits Meele com skill
        if (currentState == PlayerStates.HitMelee && state == PlayerStates.Reflect ||
            currentState == PlayerStates.HitMelee && state == PlayerStates.Dodge ||
            currentState == PlayerStates.HitMelee && state == PlayerStates.DashDodge)
            return;

        //Não cancelar anim dos hits Ranged com skill
        if (currentState == PlayerStates.HitRanged && state == PlayerStates.Reflect ||
            currentState == PlayerStates.HitRanged && state == PlayerStates.Dodge ||
            currentState == PlayerStates.HitRanged && state == PlayerStates.DashDodge)
            return;

        //Evitar sobrecarga do Input
        if (currentState == PlayerStates.Reflect || 
            currentState == PlayerStates.Rest || 
            currentState == PlayerStates.PickKnife || 
            currentState == PlayerStates.HitMelee || 
            currentState == PlayerStates.HitRanged)
            StopAllCoroutines();

        //Start dos estados
        switch (state)
        {
            case PlayerStates.Reflect:
                StartCoroutine(Reflect());
                break;

            case PlayerStates.Rest:
                StartCoroutine(Rest());
                break;

            case PlayerStates.Dodge:
                StartCoroutine(Dodge());
                break;

            case PlayerStates.DashDodge:
                StartCoroutine(DashDodge());
                break;

            case PlayerStates.PickKnife:
                StartCoroutine(PickKnife());
                break;

            case PlayerStates.HitMelee:
                StartCoroutine(Hit(1));
                break;

            case PlayerStates.HitRanged:
                StartCoroutine(Hit(2.4f));
                DropItem();
                break;

            case PlayerStates.Die:
                Die();
                DropItem();
                break;
        }
        currentState = state;
    }



    private void Update()
    {
        UpdateData();
    }



    public void UpdateData()
    {   
        movementSpeed = playerData.movementSpeed;
        dashTime = playerData.dashTime;
        dashDistance = playerData.dashDistance;
        dieLayer = playerData.dieLayer;

        reflectInput = playerData.reflectInput;
        dodgeInput = playerData.dodgeInput;
        pickWeaponInput = playerData.pickWeaponInput;
    }



    public void Move(Vector2 direction, float delta)
    {
        if (direction.x != 0 && direction.y != 0)
        {
            direction.x *= 0.75f;
            direction.y *= 0.75f;
        }

        if (direction.x != 0 || direction.y != 0)
        {
            anim.SetBool("Walking", true);
            SetState(PlayerStates.Walk);
        }
        else
        {
            anim.SetBool("Walking", false);
            SetState(PlayerStates.Idle);
        }

        if (direction.x > 0 && isLookLeft)
        {
            Flip();
        }
        else if (direction.x < 0 && !isLookLeft)
        {
            Flip();
        }

        rig.velocity = (direction * movementSpeed);
    }



    private IEnumerator Reflect()
    {
        anim.SetTrigger("Reflect");
        yield return new WaitForSeconds(0.5f);
        SetState(PlayerStates.Rest);
    }



    private IEnumerator Rest()
    {
        yield return new WaitForSeconds(0.5f);
        SetState(PlayerStates.Idle);
    }


    public bool CowndownDodge()
    {
        dodgeTimer += Time.deltaTime;
        if (dodgeTimer >= dodgeCD)
        {
            dodgeTimer = dodgeCD;
            canDodge = true;
        }
        return canDodge;
    }



    private IEnumerator Dodge()
    {
        anim.SetTrigger("Dodge");
        yield return new WaitForSeconds(1f);
        SetState(PlayerStates.Idle);
    }



    private IEnumerator DashDodge()
    {
        anim.SetTrigger("DashDodge");
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + ((isLookLeft ? Vector2.left : Vector2.right) * dashDistance);

        float progress = 0;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        while (progress < 1f)
        {
            if (progress >= 1f)
                progress = 1f;

            progress += Time.deltaTime / dashTime;

            transform.position = Vector2.Lerp(startPos, targetPos, progress);
            yield return new WaitForEndOfFrame();
        }
        SetState(PlayerStates.Idle);
    }



    private IEnumerator DownMovement(Vector2 direction)
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + (direction * 2f);
        float progress = 0; 

        while (progress < 0.2f)
        {
            progress += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, targetPos, progress);
            yield return new WaitForEndOfFrame();
        }

        startPos = transform.position;
        targetPos = startPos + (direction * 0.3f);
        while (progress > 0.2f && progress < 1f)
        {
            if (progress >= 1f)
                progress = 1f;

            progress += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, targetPos, progress);
            yield return new WaitForEndOfFrame();
        }
    }



    private IEnumerator Hit(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetState(PlayerStates.Idle);
    }


    private IEnumerator PickKnife()
    {
        anim.SetTrigger("GetKnife");   

        anim.SetLayerWeight(anim.GetLayerIndex("Knife"), 1);
        withKnife = true;

        yield return new WaitForSeconds(1f);
        SetState(PlayerStates.Idle);
    }



    public void DropItem()
    {
        if (withKnife)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Knife"), 0);
            withKnife = false;
            Instantiate(knifeDrop, transform).transform.parent = null;

            if (isLookLeft)
                knifeDrop.transform.rotation = new Quaternion(0, 180, 0, 0);
            else
                knifeDrop.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }  
    



    public void Flip()
    {
        if (isLookLeft)
        {
            isLookLeft = !isLookLeft;
            transform.Rotate(0, 180, 0);
        }
        else
        {
            isLookLeft = !isLookLeft;
            transform.Rotate(0, 180, 0);
        }
    }



    private void Die()
    {
        die = true;
        gameObject.tag = "Die";
        sortingGroup.sortingOrder = dieLayer;
    }



    public void TakeDamage(int damage, float setDifferenceX, string enemyStyle, TakeDamageAnimData animData)
    {
        if (enemyStyle == "Melee" && currentState == PlayerStates.Reflect)
        {
            return;
        }

        if (setDifferenceX > 0)
        {
            if (isLookLeft)
            {
                if (enemyStyle == "Ranged")
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageFront);
                        StartCoroutine(DownMovement(Vector2.right));
                        SetState(PlayerStates.HitRanged);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieFront);
                        StartCoroutine(DownMovement(Vector2.right));
                        SetState(PlayerStates.Die);
                    }
                }
                else
                {
                    if (currentState != PlayerStates.Reflect)
                    {
                        if (health > 1)
                        {
                            anim.SetTrigger(animData.damageFront);
                            SetState(PlayerStates.HitMelee);
                        }
                        else
                        {
                            anim.SetTrigger(animData.dieFront);
                            SetState(PlayerStates.Die);
                        }
                    }
                }
            }
            else
            {
                if (enemyStyle == "Ranged")
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageBack);
                        StartCoroutine(DownMovement(Vector2.right));
                        SetState(PlayerStates.HitRanged);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieBack);
                        StartCoroutine(DownMovement(Vector2.right));
                        SetState(PlayerStates.Die);
                    }
                }
                else
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageBack);
                        SetState(PlayerStates.HitMelee);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieBack);
                        SetState(PlayerStates.Die);
                    }
                }
            }
        }
        else
        {
            if (isLookLeft)
            {
                if (enemyStyle == "Ranged")
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageBack);
                        StartCoroutine(DownMovement(Vector2.left));
                        SetState(PlayerStates.HitRanged);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieBack);
                        StartCoroutine(DownMovement(Vector2.left));
                        SetState(PlayerStates.Die);
                    }
                }
                else
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageBack);
                        SetState(PlayerStates.HitMelee);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieBack);
                        SetState(PlayerStates.Die);
                    }
                }
            }
            else
            {
                if (enemyStyle == "Ranged")
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageFront);
                        StartCoroutine(DownMovement(Vector2.left));
                        SetState(PlayerStates.HitRanged);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieFront);
                        StartCoroutine(DownMovement(Vector2.left));
                        SetState(PlayerStates.Die);
                    }
                }
                else
                {
                    if (health > 1)
                    {
                        anim.SetTrigger(animData.damageFront);
                        SetState(PlayerStates.HitMelee);
                    }
                    else
                    {
                        anim.SetTrigger(animData.dieFront);
                        SetState(PlayerStates.Die);
                    }
                }
            }
        }

        health -= damage;
    }



    [System.Serializable]
    public struct TakeDamageAnimData
    {
        public string damageFront;
        public string dieFront;
        public string damageBack;
        public string dieBack;
    }
}
