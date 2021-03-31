using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GuyData", menuName ="ScriptableObject/GuyData")]
public class GuyData : ScriptableObject
{
    public int health;
    public float speed;
    public int dieLayer;

    public float timeToFollow;
}
