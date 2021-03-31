using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Ranged_Behaviour : Guy_IA
{
    [Header("Pistol DATA")]
    public GuyData pistolData;

    [Header("Pistol Settings")]
    public bool pistolKick;
    [SerializeField] protected float speed;
    public bool draw;

    [Range(3,7)]
    [SerializeField] private float shootDistanceX;
    private float shootDistanceY;
    public float timeToFollow;
    public int shoots;

    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletDrop;



    private void OnEnable()
    {
        Initialize();
    }



    private void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        bulletSpawn = GetComponentInChildren<Transform>().GetChild(2);

        currentState = GuyStates.Null;

        health = pistolData.health;
        speed = pistolData.speed;
        dieLayer = pistolData.dieLayer;
        timeToFollow = pistolData.timeToFollow;

        shootDistanceX = Random.Range(3f, 5f);
        shootDistanceY = 0.001f;
        shoots = 5;

        SetState(GuyStates.Follow);
    }



    public void SetState(GuyStates state)
    {
        if (state == currentState)
            return;

        if (currentState == GuyStates.Follow && state == GuyStates.Kick)
            return;

        if (state != GuyStates.Kick)
            anim.ResetTrigger("Kick");

        //Start dos estados
        switch (state)
        {
            case GuyStates.Attack:
                StartCoroutine(Shoot());
                break;

            case GuyStates.Reloud:
                StartCoroutine(Reloud());
                break;

            case GuyStates.Kick:
                StartCoroutine(Kick());
                break;

            case GuyStates.Die:
                anim.SetTrigger("Reflected");
                anim.SetBool("Shooting", false);
                this.enabled = false;
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
                Flip(player.transform.position);
                if (Mathf.Abs(transform.position.x - player.transform.position.x) >= shootDistanceX) // EIXO X
                {
                    FollowX();
                    followCrono = 0;
                }
                else
                {
                    anim.SetBool("Walking", false);
                    followCrono += 0.01f;
                    if (followCrono >= timeToFollow)
                    {
                        followCrono = timeToFollow;
                        if (Mathf.Abs(player.transform.position.y - transform.position.y) >= shootDistanceY)
                        {
                            FollowY();
                        }
                        else
                        {
                            SetState(GuyStates.Attack);
                            followCrono = 0;
                        }
                    }
                }
                break;
        }
    }



    public void FollowX()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), speed * Time.deltaTime);
        anim.SetBool("Shooting", false);
        anim.SetBool("Walking", true);
    }



    public void FollowY()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, player.transform.position.y), speed * Time.deltaTime);
        anim.SetBool("Shooting", false);
        anim.SetBool("Walking", true);           
    }



    private IEnumerator Shoot()
    {
        if (!draw)
        {
            anim.SetTrigger("Draw");
            yield return new WaitForSeconds(1f);
            draw = true;
        }

        anim.SetBool("Shooting", true);
        yield return new WaitForSeconds(1f);
        SetState(GuyStates.Follow);
    }


    private IEnumerator Kick()
    {
        anim.SetTrigger("Kick");
        anim.SetBool("Shooting", false);
        anim.SetBool("Walking", false);

        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + ((isLookLeft ? Vector2.left : Vector2.right) * 1f);

        yield return new WaitForSeconds(1f);
        float progress = 0;
        while (progress < 0.3f)
        {
            if (progress >= 0.3f)
                progress = 0.3f;

            progress += Time.deltaTime;

            transform.position = Vector2.Lerp(startPos, targetPos, progress);
            yield return new WaitForEndOfFrame();
        }
    }



    private IEnumerator Reloud()
    {
        anim.SetTrigger("Reloud");
        anim.SetBool("Shooting", false);
        yield return new WaitForSeconds(2f);
        SetState(GuyStates.Follow);
    }



    public void VerifyDraw()
    {
        if (draw)
        {
            SetState(GuyStates.Die);
        }
    }



    public void InstantiateShoot()
    {
        shoots -= 1;
        Instantiate(bulletPrefab, new Vector3(bulletSpawn.transform.position.x, bulletSpawn.transform.position.y, bulletSpawn.transform.position.z), new Quaternion
            (0, 0, 0, 0), transform);
    }



    public void BulletDrop()
    {
        Instantiate(bulletDrop, transform).transform.SetParent(null);
    }
}
