using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public List<GameObject> clouds = new List<GameObject>();
    public float spawnInterval = 5f;
    public Vector2 sizeRange = new Vector2(1f, 3f);
    public Vector2 speedRange = new Vector2(1f, 3f);

    private Camera mainCamera;
    private float timer;

    void Start()
    {
        mainCamera = Camera.main;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCloud();
            timer = 0f; // Reset timer
        }

        MoveClouds();
        RemoveOffscreenClouds();
    }

    void SpawnCloud()
    {
        float cloudYPosition = Random.Range(-mainCamera.orthographicSize, mainCamera.orthographicSize);
        Vector2 spawnPosition = new Vector2(-mainCamera.aspect * mainCamera.orthographicSize - 1f, cloudYPosition);

        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

        float randomSize = Random.Range(sizeRange.x, sizeRange.y);
        cloud.transform.localScale = new Vector3(randomSize, randomSize, 1f);

        float randomSpeed = Random.Range(speedRange.x, speedRange.y);
        cloud.GetComponent<Cloud>().speed = randomSpeed;

        clouds.Add(cloud);
    }

    void MoveClouds()
    {
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.position += Vector3.right * cloud.GetComponent<Cloud>().speed * Time.deltaTime;
        }
    }

    void RemoveOffscreenClouds()
    {
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            if (clouds[i].transform.position.x > mainCamera.aspect * mainCamera.orthographicSize + 1f)
            {
                Destroy(clouds[i]);
                clouds.RemoveAt(i);
            }
        }
    }

    public List<GameObject> GetClouds()
    {
        return clouds;
    }
}
