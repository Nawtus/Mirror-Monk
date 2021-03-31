using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerData", menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health;
    public float movementSpeed;
    public float dashTime;
    public float dashDistance;

    public int dieLayer;

    [Header("Cowndowns")]
    public float dodgeCD;

    public KeyCode reflectInput;
    public KeyCode dodgeInput;
    public KeyCode pickWeaponInput;
}
