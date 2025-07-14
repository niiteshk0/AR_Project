using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject footBallPrefab;
    [SerializeField] List<GameObject> footBall;
    [SerializeField] int poolSize = 5;
    void Start()
    {
        footBall = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(footBallPrefab);
            obj.SetActive(false);
            footBall.Add(obj);
        }
    }

    public GameObject GetBall()
    {
        foreach (GameObject obj in footBall)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
