using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHider : MonoBehaviour
{
    void Start()
    {
        Pathfinding.Instance.OnPathfindingSetup += HideObject;
    }

    private void OnDisable()
    {
        Pathfinding.Instance.OnPathfindingSetup -= HideObject;
    }

    void HideObject()
    {
        gameObject.SetActive(false);
    }
}
