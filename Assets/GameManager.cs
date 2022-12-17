using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;
using Generic.Enums;
using System;
using GridSystem;

public class GameManager : Singleton<GameManager>
{
    private GameState state; 
    public GameState State{get{return state ;} private set{state = value; onGameStateChanged?.Invoke(value);}}
    Action<GameState> onGameStateChanged;
    public GridMapManager GridMapManagerRef;
    
    private void Update() 
    {
        if(Input.GetMouseButton(0))
        {
            GridMapManagerRef.LightGrid(GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)),new Vector2Int(2,2));
        }
        if(Input.GetMouseButtonUp(0))
        {
            GridMapManagerRef.OccupyGrids("test" , GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)) , new Vector2Int(2,2));
        }
    }
}
