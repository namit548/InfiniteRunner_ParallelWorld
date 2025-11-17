using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> obstacles = new List<GameObject>();
    public float obstacleTime = 72f;
    private void Start()
    {
        InvokeRepeating("SpawnObstacle", 0.1f, obstacleTime);
    }

    void SpawnObstacle()
    {
        int obstacleNumber = Random.Range(0, obstacles.Count);

        Instantiate(obstacles[obstacleNumber], this.transform.position, Quaternion.identity);
    }

  

}
