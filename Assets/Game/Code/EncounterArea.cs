using UnityEngine;
using System.Collections;

public class EncounterArea : MonoBehaviour
{

    public Monster[] Monsters;

    [Range(1, 10)]
    public int GemiddeldeLevel;

    private float spawnTimer;

    public void OnTriggerStay(Collider col)
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > 1)
            {
                Monster randomMonster = Monsters[Random.Range(0, Monsters.Length)];
                if (Random.Range(0, 101) < randomMonster.SpawnRate)
                {
                    GameManager.instance.ActivateBattle(randomMonster);
                }
                spawnTimer = 0;
            }
        }
    }
}
