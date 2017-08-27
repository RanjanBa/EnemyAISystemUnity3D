using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    public bool DebugPath;

    [HideInInspector]
    public Transform[] pathTransformPosition;

    private void Start()
    {
        int numberOfChildGM = transform.childCount;

        pathTransformPosition = new Transform[numberOfChildGM];

        for (int i = 0; i < numberOfChildGM; i++)
        {
            pathTransformPosition[i] = transform.GetChild(i);
        }
    }

    private void OnDrawGizmos()
    {
        if (DebugPath)
        {
            int numberOfChildGM = transform.childCount;

            pathTransformPosition = new Transform[numberOfChildGM];

            for (int i = 0; i < numberOfChildGM; i++)
            {
                pathTransformPosition[i] = transform.GetChild(i);
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < pathTransformPosition.Length; i++)
            {
                if (i >= pathTransformPosition.Length - 1)
                {
                    Gizmos.DrawLine(pathTransformPosition[i].position, pathTransformPosition[0].position);
                    break;
                }

                Gizmos.DrawLine(pathTransformPosition[i].position, pathTransformPosition[i + 1].position);
            }
        }
    }
}
