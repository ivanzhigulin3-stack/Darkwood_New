using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding; 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))] 
public class Mover : MonoBehaviour
{
    private Rigidbody2D rb;
    private Seeker seeker;

    // Переменные для работы с путями A*
    private Path currentPath;
    private int currentWaypoint = 0;
    private bool isFollowingPath = false;
    private Coroutine pathUpdateCoroutine;

    [Header("A* Pathfinding Settings")]
    [SerializeField] private float nextWaypointDistance = 0.5f; // Дистанция, при которой точка считается достигнутой
    [SerializeField] private float pathUpdateInterval = 0.3f;   // Оптимизация. Пересчет пути

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Движение игрока
    public void Move(Vector2 direction, float speed)
    {
        isFollowingPath = false;
        rb.linearVelocity = direction * speed;
    }

    //ДВИЖЕНИЕ ПО ПУТИ A*
    public void StartFollowingTarget(Transform target, float speed)
    {
        if (isFollowingPath) return;

        isFollowingPath = true;
        pathUpdateCoroutine = StartCoroutine(UpdatePathRoutine(target));
    }

    public void StopFollowingTarget()
    {
        isFollowingPath = false;
        if (pathUpdateCoroutine != null)
        {
            StopCoroutine(pathUpdateCoroutine);
        }
        Stop();
    }

    private IEnumerator UpdatePathRoutine(Transform target)
    {
        while (isFollowingPath)
        {
            if (target != null && seeker.IsDone())
            {
                seeker.StartPath(transform.position, target.position, OnPathComplete);
            }
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentPath = p;
            currentWaypoint = 0; 
        }
    }

    public void MoveAlongPath(float speed)
    {
        if (!isFollowingPath || currentPath == null) return;

        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            Stop();
            return;
        }

        //направление к следующей точке маршрута A*
        Vector2 direction = ((Vector2)currentPath.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * speed;

        float distance = Vector2.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        // Перевод физики в спящий режим для экономии ресурсов ПК
        if (rb.IsSleeping() == false) rb.Sleep();
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        if (direction.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
}