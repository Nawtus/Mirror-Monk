using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform parentPos;
    private SpriteRenderer spriteRenderer;

    [Range(1, 10)]
    public int damage = 2;
    public Character_Behaviour.TakeDamageAnimData playerAnimData;


    private void Awake()
    {
        parentPos = GetComponentInParent<Transform>().parent;
    }



    private void Start()
    {
        bulletSpeed = 3f;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameObject.transform.parent = null;
    }



    void Update()
    {
        transform.Translate(new Vector2(bulletSpeed * Time.deltaTime, 0));

        if (Vector2.Distance(transform.position, parentPos.position) > 15f)
        {
            Destroy(this.gameObject);
        }
    }



    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Character_Reflect") && col.GetComponentInParent<Character_Behaviour>().withKnife)
        {
            bulletSpeed = -3f;
            spriteRenderer.flipX = true;
        }

        if (col.gameObject.CompareTag("Player"))   
        {
            Character_Behaviour playerBehaviour = col.gameObject.GetComponentInParent<Character_Behaviour>();

            float differenceX = col.bounds.center.x - GetComponent<Collider2D>().bounds.center.x;

            playerBehaviour.TakeDamage(damage, differenceX, "Ranged", playerAnimData);

            Destroy(this.gameObject);
        }
    }
}
