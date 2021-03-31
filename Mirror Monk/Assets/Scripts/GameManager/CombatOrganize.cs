using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatOrganize : MonoBehaviour
{
    public GameObject nearGuy;
    public GameObject backstabGuy;

    [SerializeField] List<GameObject> meleeGuys;
    [SerializeField] List<GameObject> punchGuys;
    [SerializeField] List<GameObject> knifeGuys;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }



    void Update()
    {
        punchGuys = new List<GameObject>(GameObject.FindGameObjectsWithTag("PunchGuy"));
        knifeGuys = new List<GameObject>(GameObject.FindGameObjectsWithTag("KnifeGuy"));

        if (meleeGuys.Count == 0)
        {
            meleeGuys.AddRange(punchGuys);
            meleeGuys.AddRange(knifeGuys);
        }
        else
        {
            NearestMeleeEnemy(meleeGuys);

            if (nearGuy != null && (nearGuy.CompareTag("Die") || nearGuy.CompareTag("DieWithItem") || !nearGuy.activeSelf))
            {
                meleeGuys.Remove(nearGuy);
                nearGuy = null;
            }

            if (backstabGuy == null && meleeGuys.Count >= 2)
            {
                backstabGuy = meleeGuys[Random.Range(1, meleeGuys.Count)];
            }


            if (backstabGuy != null && (backstabGuy.CompareTag("Die") || backstabGuy.CompareTag("DieWithItem") || !backstabGuy.activeSelf))
            {
                meleeGuys.Remove(backstabGuy);
                backstabGuy = null;
            }
        }
    }



    GameObject NearestMeleeEnemy(List<GameObject> meleeGuys)
    {
        nearGuy = null;
        float minDistance = float.MaxValue;
        Vector3 playerPos = player.transform.position;

        //nearGuy = SearchGuy(true, minDistance, midDistance, out minDistance);
        //secondNearGuy = SearchGuy(false, minDistance, midDistance, out midDistance);

        foreach (GameObject guy in meleeGuys)
        {
            float distance = Vector3.Distance(guy.transform.position, playerPos);
            if (distance < minDistance)
            {
                if (guy == backstabGuy)
                    return null;
                nearGuy = guy;
                minDistance = distance;
            }
        }
        return nearGuy;

        //float midDistance = float.MaxValue;
        /*foreach (GameObject guy in meleeGuys)
        {
            float distance = Vector3.Distance(guy.transform.position, playerPos);
            if (distance > minDistance && distance < midDistance)
            {
                secondNearGuy = guy;
                midDistance = distance;
            }
        }*/
    }


    GameObject SearchGuy(bool isNearGuy, float minDistance, float midDistance, out float newDistance)
    {
        Vector3 playerPos = player.transform.position;
        GameObject guy = null;

        foreach (GameObject currentGuy in meleeGuys)
        {
            float distance = Vector3.Distance(currentGuy.transform.position, playerPos);
            bool result = false;

            if (isNearGuy)
            {
                result = distance < minDistance;
            }
            else
            {
                result = distance > minDistance && distance < midDistance;
            }

            if (result)
            {
                if (isNearGuy)
                {
                    minDistance = distance;
                }
                else
                {
                    midDistance = distance;
                }

                guy = currentGuy;
            }
        }

        if (isNearGuy)
        {
            newDistance = minDistance;
        }
        else
        {
            newDistance = midDistance;
        }

        return guy;
    }
}
