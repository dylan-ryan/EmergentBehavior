using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1f;
    public float alignmentStrength = 1f;
    public float cohesionStrength = 1f;
    public float separationStrength = 1.5f;
    public float avoidanceStrength = 2f;
    public float avoidanceRadius = 2f;
    public float boundaryAvoidanceStrength = 5f;
    public float boundaryAvoidanceRadius = 2f;
    public float rotationSpeed = 2f;

    public Vector2 velocity;

    private GameManager manager;
    private CloudManager cloudManager;
    private Camera mainCamera;
    private Vector2 screenBounds;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        cloudManager = FindObjectOfType<CloudManager>();
        mainCamera = Camera.main;
        screenBounds = GetScreenBounds();

        velocity = Random.insideUnitCircle.normalized * maxSpeed;
    }

    void Update()
    {
        List<Bird> nearbyBirds = GetNearbyBirds();
        List<GameObject> nearbyClouds = GetNearbyClouds();

        Vector2 separation = Separation(nearbyBirds) * separationStrength;
        Vector2 alignment = Alignment(nearbyBirds) * alignmentStrength;
        Vector2 cohesion = Cohesion(nearbyBirds) * cohesionStrength;
        Vector2 avoidance = AvoidClouds(nearbyClouds) * avoidanceStrength;
        Vector2 boundaryAvoidance = BoundaryAvoidance() * boundaryAvoidanceStrength;

        Vector2 steering = separation + alignment + cohesion + avoidance + boundaryAvoidance;
        steering = Vector2.ClampMagnitude(steering, maxSpeed);

        velocity = Vector2.Lerp(velocity, velocity + steering, Time.deltaTime).normalized * maxSpeed;

        transform.position += (Vector3)velocity * Time.deltaTime;

        SmoothRotateTowards(velocity);
    }

    void SmoothRotateTowards(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
    }

    Vector2 AvoidClouds(List<GameObject> clouds)
    {
        Vector2 avoidanceForce = Vector2.zero;

        foreach (GameObject cloud in clouds)
        {
            float distance = Vector2.Distance(transform.position, cloud.transform.position);
            if (distance < avoidanceRadius)
            {
                avoidanceForce += (Vector2)(transform.position - cloud.transform.position).normalized / distance;
            }
        }

        return avoidanceForce;
    }

    List<GameObject> GetNearbyClouds()
    {
        return cloudManager.GetClouds().Where(c => Vector2.Distance(transform.position, c.transform.position) < avoidanceRadius).ToList();
    }

    // Get nearby birds
    List<Bird> GetNearbyBirds()
    {
        return manager.GetBoids().Where(b => Vector2.Distance(transform.position, b.transform.position) < neighborRadius && b != this).ToList();
    }

    // Steering: Separation behavior
    Vector2 Separation(List<Bird> birds)
    {
        Vector2 force = Vector2.zero;

        foreach (Bird other in birds)
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance < separationDistance)
            {
                force += (Vector2)(transform.position - other.transform.position).normalized / distance;
            }
        }

        return force;
    }

    Vector2 Alignment(List<Bird> birds)
    {
        Vector2 avgVelocity = Vector2.zero;

        foreach (Bird other in birds)
        {
            avgVelocity += other.velocity;
        }

        if (birds.Count > 0)
        {
            avgVelocity /= birds.Count;
            avgVelocity = avgVelocity.normalized;
        }

        return (avgVelocity - velocity);
    }

    Vector2 Cohesion(List<Bird> birds)
    {
        Vector2 avgPosition = Vector2.zero;

        foreach (Bird other in birds)
        {
            avgPosition += (Vector2)other.transform.position;
        }

        if (birds.Count > 0)
        {
            avgPosition /= birds.Count;
            return (avgPosition - (Vector2)transform.position).normalized;
        }

        return Vector2.zero;
    }

    Vector2 BoundaryAvoidance()
    {
        Vector2 avoidanceForce = Vector2.zero;
        Vector3 pos = transform.position;

        if (pos.x > screenBounds.x - boundaryAvoidanceRadius)
            avoidanceForce += new Vector2(-1, 0);
        else if (pos.x < -screenBounds.x + boundaryAvoidanceRadius)
            avoidanceForce += new Vector2(1, 0);

        if (pos.y > screenBounds.y - boundaryAvoidanceRadius)
            avoidanceForce += new Vector2(0, -1);
        else if (pos.y < -screenBounds.y + boundaryAvoidanceRadius)
            avoidanceForce += new Vector2(0, 1);

        return avoidanceForce.normalized;
    }

    Vector2 GetScreenBounds()
    {
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
        return new Vector2(Mathf.Abs(topRight.x - bottomLeft.x) / 2, Mathf.Abs(topRight.y - bottomLeft.y) / 2);
    }
}
