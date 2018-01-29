using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

    public string Name;
    public Sprite TextureFront, TextureBack;

    [TextArea]
    public string Info;

    [Range(0, 10)]
    public int Attack, Defense, Agility;

    public int Health = 1;
    [HideInInspector]
    public int CurrentHealth;

    [Range(0, 100)]
    public float SpawnRate;

    [Range(0, 100)]
    public float CatchRate;
}
