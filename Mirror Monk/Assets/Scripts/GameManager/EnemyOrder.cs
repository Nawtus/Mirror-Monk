using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyOrder", menuName = "ScriptableObject/EnemyOrder")]
public class EnemyOrder : ScriptableObject
{
    public EnemyOrderGroup[] enemyGroup;
    public int allEnemies = 0;


    public int AllEnemies()
    {
        for (int i = 0; i < enemyGroup.Length; i++)
        {
            allEnemies += enemyGroup.Length;
        }

        return allEnemies;
    }
}


[System.Serializable]
public class EnemyOrderGroup
{
    public GameObject[] fight;

}
