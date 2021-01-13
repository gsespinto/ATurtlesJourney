using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    #region Variables
    [SerializeField] private ItemPrefab[] itemsPrefabs;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private float waitSeconds = 1;
    [SerializeField] private int maxSpawners = 1;

    private int nSpawners;
    private int spawnRange;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        IntitItemPrefabs();
        nSpawners = 1;
        StartCoroutine(Spawner());
        StartCoroutine(CreateSpawner(true, 7f));
    }

    IEnumerator Spawner(bool run = true)
    {
        while (run)
        {
            // if the game's paused then do nothing
            if (playerScript.isPaused)
            {
                yield return null;
            }

            // wait random amount of seconds within range
            yield return new WaitForSeconds(Random.Range(0f, waitSeconds));

            SpawnItem();
            
            yield return null;
        }
    }

    IEnumerator CreateSpawner(bool run, float waitSeconds)
    {
        while (run)
        {
            // if the game's paused or the player is dead stop coroutine
            if (playerScript.isPaused || playerScript.isDead)
            {
                run = false;
                yield return null;
            }

            // if the number of spawners hasn't reached it's limit
            // initialize new spawner
            if (nSpawners < maxSpawners)
            {
                yield return new WaitForSeconds(waitSeconds);
                StartCoroutine(Spawner());
                nSpawners++;
            }
            // else stop this coroutine
            else
            {
                run = false;
            }

            yield return null;
        }
    }

    /// <summary> Item spawn and initialization, for runner and evade mode </summary>
    private void SpawnItem()
    {
        Vector3 spawnPoint = new Vector3();
        MyEnums.Direction direction = 0;

        // Autorunner mode spawn params
        if (playerScript.autoRunner)
        {
            // Spawn the items to go from the left of the screen to the right
            spawnPoint = new Vector3(Random.Range(1.1f, 2f), Random.Range(0.05f, 0.95f), 0f);
            direction = MyEnums.Direction.LEFT;
        }
        // Evade mode spawn params
        else
        {
            // picks random item direction and spawnpoint (UP, LEFT, DOWN, RIGHT)
            int spawnPick = Random.Range(0, 4);
            switch (spawnPick)
            {
                case 0:
                    direction = MyEnums.Direction.LEFT;
                    spawnPoint = new Vector3(Random.Range(1.1f, 2f), Random.Range(0.05f, 0.95f), 0f);
                    break;
                case 1:
                    direction = MyEnums.Direction.DOWN;
                    spawnPoint = new Vector3(Random.Range(0.05f, 0.95f), Random.Range(1.1f, 2f), 0f);
                    break;
                case 2:
                    direction = MyEnums.Direction.RIGHT;
                    spawnPoint = new Vector3(Random.Range(-1.1f, -2f), Random.Range(0.05f, 0.95f), 0f);
                    break;
                case 3:
                    direction = MyEnums.Direction.UP;
                    spawnPoint = new Vector3(Random.Range(0.05f, 0.95f), Random.Range(-1.1f, -2f), 0f);
                    break;

            }
        }

        // Item initialization
        spawnPoint = Camera.main.ViewportToWorldPoint(spawnPoint);
        spawnPoint.z = 0f;
        Quaternion randomRotation = new Quaternion();
        randomRotation.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));

        GameObject item = GameObject.Instantiate(PickRandomItem(), spawnPoint, randomRotation);

        item.GetComponent<ItemScript>().playerScript = this.playerScript;
        item.GetComponent<ItemScript>().audioSource = this.GetComponent<AudioSource>();
        item.GetComponent<ItemScript>().orientation = direction;
    }

    /// <summary> Intializes the probability values of each item </summary>
    private void IntitItemPrefabs()
    {
        int currentProb = 0;

        for (int i = 0; i < itemsPrefabs.Length; i++)
        {
            itemsPrefabs[i].prob += currentProb;
            currentProb = itemsPrefabs[i].prob;

            // Debug.Log(itemsPrefabs[i].prefab.name + "'s spawn probability is: " + itemsPrefabs[i].prob);
        }

        spawnRange = currentProb;
    }

    /// <summary> Picks a random object from the item array taking in consideration it's probability </summary>
    /// <returns> Random object from item array </returns>
    private GameObject PickRandomItem()
    {
        GameObject spawningObj = null;
        int spawnIndex = Random.Range(0, spawnRange);

        for (int i = 0; i < itemsPrefabs.Length; i++)
        {
            if (spawnIndex < itemsPrefabs[i].prob)
            {
                spawningObj = itemsPrefabs[i].prefab;
                break;
            }
            else
            {
                continue;
            }
        }

        return spawningObj;
    }
}

[System.Serializable]
public class ItemPrefab
{
    public GameObject prefab = null;
    public int prob = 1;
}
