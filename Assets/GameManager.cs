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
        if(Input.GetMouseButtonDown(0))
        {
            GridMapManagerRef.ResetHighlightedGrids();
        }
        if(Input.GetMouseButton(0))
        {
            Vector2Int hoveringGridLocation = GridMapManagerRef.GetGridLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(GridMapManagerRef.isSpaceOccupied(hoveringGridLocation,new Vector2Int(2,3)))
            {
                GridMapManagerRef.HighlightGrids(hoveringGridLocation , new Vector2Int(2,3) , Color.red);   
            }
            else
            {
                GridMapManagerRef.HighlightGrids(hoveringGridLocation , new Vector2Int(2,3) , Color.green);
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            GridMapManagerRef.OccupyGrids("test" , GridMapManagerRef.GetGridLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition)) , new Vector2Int(2,3));
        }
    }
}
