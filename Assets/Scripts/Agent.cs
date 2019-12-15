using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public static void SelectAgent(Agent agent)
    {
        Debug.Log(agent.name + " selected.");
        InputController.OnTileMouseDown = agent.SelectMovementDestination;
    }

    public void SelectMovementDestination(NavTile goal)
    {
        Physics.Raycast(transform.position + Vector3.up * 0.001f, Vector3.down, out RaycastHit startHitInfo, float.MaxValue, LayerMask.GetMask("NavGrid"));            
        NavTile start = startHitInfo.collider.GetComponent<NavTile>();
        List<NavTile> pathTiles = FindObjectOfType<NavGrid>().FindPath(start, goal);

        if (pathTiles != null && pathTiles[pathTiles.Count - 1] != start)
        {
            StartCoroutine(Animate(pathTiles));
        }
    }

    private IEnumerator Animate(List<NavTile> pathTiles)
    {
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

    private void OnMouseOver()
    {
        InputController.HandleAgentMouseOver(this);
    }

    private void OnMouseDown()
    {
        InputController.HandleAgentMouseDown(this);
    }
}