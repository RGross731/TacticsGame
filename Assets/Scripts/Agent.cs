using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private NavTile current;

    private void Awake()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.001f, Vector3.down, out RaycastHit hitInfo, float.MaxValue, LayerMask.GetMask("NavGrid")))
        {
            current = hitInfo.collider.GetComponent<NavTile>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.MaxValue, LayerMask.GetMask("NavGrid")))
            {
                NavTile goal = hitInfo.collider.GetComponent<NavTile>();
                List<NavTile> pathTiles = FindObjectOfType<NavGrid>().FindPath(current, goal);

                if (pathTiles != null && pathTiles[pathTiles.Count - 1] != current)
                {
                    StartCoroutine(Animate(pathTiles));
                }
            }
        }
    }

    private IEnumerator Animate(List<NavTile> pathTiles)
    {
        current = pathTiles[pathTiles.Count - 1];

        foreach (NavTile t in pathTiles)
        {
            transform.LookAt(t.transform.position, Vector3.up);

            while (transform.position != t.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, t.transform.position, 5 * Time.deltaTime);
                yield return null;
            }
        }

        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }
}
