using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;
using Generic.Enums;
using System;
using GridSystem;
using Buildings;
using UI;

public class GameManager : Singleton<GameManager>
{
    private GameState state; 
    public GameState State{get{return state ;} private set{state = value; onGameStateChanged?.Invoke(value);}}
    Action<GameState> onGameStateChanged;
    public GridMapManager GridMapManagerRef;
    public UIManager UIManagerRef;
    public List<Building> AvailableBuildings;

    public bool PlacingBuilding;

    private Building _buildingToPlace;

    [SerializeField]
    private List<Building> _placedBuildings; 

    [SerializeField]
    private Transform _buildingsParent;

    private void Start()
    {
        UIManagerRef.onProductionItemClicked += BuildingPlacementCheckRunner;
    }
    private void Update() 
    {
        if(PlacingBuilding)
        {
            BuildingPlacementCheck();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            UIManagerRef.UpdateDescriptionPanel(GetPlacedBuilding(GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)).Occupation));
        }
    }
    void BuildingPlacementCheckRunner(Building buildingToPlace)
    {
        PlacingBuilding = true;
        _buildingToPlace= buildingToPlace;
    }
    void BuildingPlacementCheck()
    {
        Vector2Int hoveringGridLocation = GridMapManagerRef.GetGridLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        bool isOccupied = GridMapManagerRef.isSpaceOccupied(hoveringGridLocation, _buildingToPlace.Dimensions);
        if(isOccupied)
        {
            GridMapManagerRef.HighlightGrids(hoveringGridLocation , _buildingToPlace.Dimensions , Color.red);   
        }
        else
        {
            GridMapManagerRef.HighlightGrids(hoveringGridLocation , _buildingToPlace.Dimensions , Color.green);
        }

        if(Input.GetMouseButtonDown(0) && !isOccupied)
        {
            GridMapManagerRef.OccupyGrids(_buildingToPlace.BuildingID + "/" + _placedBuildings.Count , GridMapManagerRef.GetGridLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition)) , _buildingToPlace.Dimensions);
            _placedBuildings.Add(Instantiate(_buildingToPlace , GridMapManagerRef.GridToWorldLocation(hoveringGridLocation) + _buildingsParent.position ,Quaternion.identity , _buildingsParent));
            GridMapManagerRef.ResetHighlightedGrids();
            UIManagerRef.ClearClickedItems();
            PlacingBuilding = false;
        }
    }

    public Building GetPlacedBuilding(string uniqID)
    {
        return _placedBuildings[int.Parse(uniqID.Split('/')[1])];
    }
}
