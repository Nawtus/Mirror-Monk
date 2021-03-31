using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using Cinemachine;

public class CombatTrigger : MonoBehaviour
{
    public EnemyOrder enemyOrder;
    private CinemachineVirtualCamera mainCamera;
    private Transform playerTransform;

    public List<Transform> spotRespawns;
    public int indexEnemyOrderStart;
    public int waves;

    [Tooltip("All Enemies in the selected layers")]
    public LayerMask enemiesLayer;
    public List<GameObject> allEnemies;

    private void OnEnable()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }



    private void Update()
    {
        allEnemies = FindObjectsOfType<GameObject>().Where(o => enemiesLayer == (enemiesLayer | 1 << o.gameObject.layer)).ToList();

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].CompareTag("Die") || allEnemies[i].CompareTag("DieWithItem") || !allEnemies[i].activeSelf)
            {
                allEnemies[i].layer = 0;
            }
        }

        if (allEnemies.Count == 0)
        {
            this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }

        if (waves == 0 && allEnemies.Count == 0)
        {
            this.enabled = false;
        }
    }



    private void InstatiateEnemies()
    {
        for (int i = 0; i < enemyOrder.enemyGroup[indexEnemyOrderStart].fight.Length; i++)
        {
            Instantiate(enemyOrder.enemyGroup[indexEnemyOrderStart].fight[i], spotRespawns[i].position, new Quaternion(0,0,0,0));
        }
        indexEnemyOrderStart++;
        waves--;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && waves > 0)
        {
            mainCamera.Follow = null;
            InstatiateEnemies();
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }



    private void OnDisable()
    {
        mainCamera.Follow = playerTransform;
    }
}
