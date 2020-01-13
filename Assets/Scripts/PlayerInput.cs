using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static Action<Agent> OnAgentMouseOver;
    public static Action<Agent> OnAgentMouseDown;
    public static Action<NavTile> OnTileMouseOver;
    public static Action<NavTile> OnTileMouseDown;
    public static Action OnCancel;

    private void Awake()
    {
        OnAgentMouseOver = null;
        OnAgentMouseDown = Agent.SelectAgent;
        OnTileMouseOver = null;
        OnTileMouseDown = null;
        OnCancel = null;
    }

    public static void HandleAgentMouseOver(Agent agent)
    {
        OnAgentMouseOver?.Invoke(agent);
    }

    public static void HandleAgentMouseDown(Agent agent)
    {
        OnAgentMouseDown?.Invoke(agent);
    }

    public static void HandleTileMouseOver(NavTile tile)
    {
        OnTileMouseOver?.Invoke(tile);
    }

    public static void HandleTileMouseDown(NavTile tile)
    {
        OnTileMouseDown?.Invoke(tile);
    }

    public static void HandleCancel()
    {
        OnCancel?.Invoke();
    }
}
