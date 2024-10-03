using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject birdPrefab;
    public int birdCount = 50;
    private List<Bird> birds = new List<Bird>();

    void Start()
    {
        Vector2 bounds = GetWorldBounds();

        for (int i = 0; i < birdCount; i++)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(-bounds.x, bounds.x),
                Random.Range(-bounds.y, bounds.y)
            );
            GameObject boidObject = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);
            Bird boid = boidObject.GetComponent<Bird>();
            birds.Add(boid);
        }
    }

    Vector2 GetWorldBounds()
    {
        Camera cam = Camera.main;

        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        float xBound = (topRight.x - bottomLeft.x) / 2f;
        float yBound = (topRight.y - bottomLeft.y) / 2f;

        return new Vector2(xBound, yBound);
    }

    public List<Bird> GetBoids()
    {
        return birds;
    }
}
