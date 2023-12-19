using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Aoiti.Pathfinding;

public class EnemyMovable : EnemyBase
{
    [Header("Pathfinding")]
    [SerializeField] float gridSize = 0.5f; //increase patience or gridSize for larger maps
    [SerializeField] float speed = 0.05f; //increase for faster movement
    [Tooltip("The layers that the navigator can not pass through.")]
    [SerializeField] LayerMask obstacles;
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")]
    [SerializeField] bool searchShortcut = false;
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid = false;
    [SerializeField] bool drawDebugLines;

    [Header("Light candle behavior")]
    [SerializeField] private float rotateDurtionWhileMoving = 0.3f;
    [SerializeField] private float slightRotationZ = 10f;
    [SerializeField] private float lightingTime = 1f;

    private Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    private List<Vector2> path;
    private List<Vector2> pathLeftToGo = new List<Vector2>();
    private bool isMoving = false;
    private bool isRotationWhileMovingRoutineRunning = false;
    private Candle candleToLight = null;
    private SightingArea sightingArea;
    private Vector2 originalPosition;
    private Quaternion originalRot;

    #region public
    public bool IsBusy { get { return candleToLight != null; } }

    public void GoLightCandle(Candle sender)
    {
        candleToLight = sender;
        GetMoveCommand(sender.transform.position);
    }

    public bool CanNoticeCandle(Vector3 position)
    {
        return Vector3.Distance(position, transform.position) <= sightingArea.DetectCandleDistance;
    }
    #endregion

    protected override void Start()
    {
        originalPosition = transform.position;
        originalRot = transform.rotation;
        sightingArea = GetComponentInChildren<SightingArea>();
        pathfinder = new Pathfinder<Vector2>(GetDistance, GetNeighbourNodes, 1000); //increase patience or gridSize for larger maps

        base.Start();
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        if (pathLeftToGo.Count > 0) //if the target is not yet reached
        {
            Vector3 dir = ((Vector3)pathLeftToGo[0] - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            if (!isRotationWhileMovingRoutineRunning)
            {
                isRotationWhileMovingRoutineRunning = true;
                StartCoroutine(RotateWhileMovingRoutine(dir));
            }

            if (((Vector2)transform.position - pathLeftToGo[0]).sqrMagnitude < speed * speed * Time.deltaTime)
            {
                isRotationWhileMovingRoutineRunning = false;
                StopAllCoroutines();
                transform.position = pathLeftToGo[0];
                pathLeftToGo.RemoveAt(0);

                if (pathLeftToGo.Count == 0)
                {
                    isMoving = false;
                    // reach candle
                    if (candleToLight != null)
                    {
                        StartCoroutine(LightUpCandleAndReturn());
                    }
                    // back to original pos
                    else
                    {
                        StartCoroutine(BackToOriginalState());
                    }
                }    
            }
        }

#if UNITY_EDITOR
        if (drawDebugLines)
        {
            for (int i = 0; i < pathLeftToGo.Count - 1; i++) //visualize your path in the sceneview
            {
                Debug.DrawLine(pathLeftToGo[i], pathLeftToGo[i + 1]);
            }
        }
#endif
    }

    private IEnumerator RotateWhileMovingRoutine(Vector3 targetDir)
    {
        // rotate to move direction
        Vector3 originalOrientation = transform.right;
        if (originalOrientation != targetDir)
        {
            float elapsedTime = 0f;
            while (elapsedTime < rotateDurtionWhileMoving)
            {
                elapsedTime += Time.deltaTime;
                transform.right = Vector3.Lerp(originalOrientation, targetDir, elapsedTime / rotateDurtionWhileMoving);
                yield return null;
            }
            transform.right = targetDir;
        }

        // small rotation to add feelings
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 slightRot1 = transform.rotation.eulerAngles - new Vector3(0, 0, slightRotationZ);
        Vector3 slightRot2 = transform.rotation.eulerAngles + new Vector3(0, 0, slightRotationZ);
        while (true)
        {
            yield return StartCoroutine(RotateBetweenTwoAnglesRoutine(currentRot, slightRot1, rotateDurtionWhileMoving));
            yield return StartCoroutine(RotateBetweenTwoAnglesRoutine(slightRot1, currentRot, rotateDurtionWhileMoving));
            yield return StartCoroutine(RotateBetweenTwoAnglesRoutine(currentRot, slightRot2, rotateDurtionWhileMoving));
            yield return StartCoroutine(RotateBetweenTwoAnglesRoutine(slightRot2, currentRot, rotateDurtionWhileMoving));
        }
    }

    private IEnumerator LightUpCandleAndReturn()
    {
        // light up
        yield return new WaitForSeconds(lightingTime);
        candleToLight?.LightUp();
        candleToLight = null;

        // return to original pos
        GetMoveCommand(originalPosition);
    }

    private IEnumerator BackToOriginalState()
    {
        if (transform.rotation != originalRot)
        {
            yield return StartCoroutine(RotateBetweenTwoAnglesRoutine(
                transform.rotation.eulerAngles, originalRot.eulerAngles, rotateDurtionWhileMoving));
        }

        StartCoroutine(RotateRoutine());
    }

    #region pathfinding related (copied from the plugin example)

    private void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            isMoving = true;
            isRotationWhileMovingRoutineRunning = false;
            StopAllCoroutines();
            if (searchShortcut && path.Count > 0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }

            pathLeftToGo = GetCleanerPaths(pathLeftToGo);
        }
    }

    /// <summary>
    /// Clean up the paths a little bit to join straight lines as one path
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    private List<Vector2> GetCleanerPaths(List<Vector2> paths, float angleTolerance = 0.0001f)
    {
        if (paths == null || paths.Count <= 2)
            return paths;

        var res = new List<Vector2>
        {
            paths[0],
            paths[1]
        };
        Vector3 dir = (paths[1] - path[0]).normalized;
        for (int i = 2; i < paths.Count; i++)
        {
            Vector3 newDir = (paths[i] - paths[i - 1]).normalized;
            if (Quaternion.Angle(Quaternion.Euler(dir), Quaternion.Euler(newDir)) < angleTolerance)
            {
                // eliminate the previous path point and use this one
                res.RemoveAt(res.Count - 1);
                res.Add(paths[i]);
            }
            else
            {
                res.Add(paths[i]);
            }    
            dir = newDir;
        }

        return res;
    }

    /// <summary>
    /// Finds closest point on the grid
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector2 GetClosestNode(Vector2 target)
    {
        return new Vector2(Mathf.Round(target.x / gridSize) * gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }

    /// <summary>
    /// A distance approximation. 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    private float GetDistance(Vector2 A, Vector2 B)
    {
        return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
    }

    /// <summary>
    /// Finds possible conenctions and the distances to those connections on the grid.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Dictionary<Vector2, float> GetNeighbourNodes(Vector2 pos)
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j) * gridSize;
                if (!Physics2D.Linecast(pos, pos + dir, obstacles))
                {
                    neighbours.Add(GetClosestNode(pos + dir), dir.magnitude);
                }
            }
        }
        return neighbours;
    }

    private List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();

        for (int i = 0; i < path.Count; i++)
        {
            newPath.Add(path[i]);
            for (int j = path.Count - 1; j > i; j--)
            {
                if (!Physics2D.Linecast(path[i], path[j], obstacles))
                {

                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
    #endregion
}
