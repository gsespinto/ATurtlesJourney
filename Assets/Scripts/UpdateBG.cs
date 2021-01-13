using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBG : MonoBehaviour
{
    [SerializeField] private GameObject bgPrefab;
    private bool hasSpawned; 

    // Update is called once per frame
    void Update()
    {
        SpawnBG();
    }

    void SpawnBG()
    {
        Vector3 furtherRight = this.transform.position + Vector3.right * this.transform.localScale.x;
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(furtherRight);
        if (screenPoint.x < 2f && !hasSpawned)
        {
            GameObject.Instantiate(bgPrefab, this.transform.position + Vector3.right * this.transform.localScale.x, bgPrefab.transform.rotation);
            hasSpawned = true;
        }
        else if (screenPoint.x < 0.1f)
            Destroy(this.gameObject);
    }
}
