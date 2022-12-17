using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic;
using Generic.Enums;
using System;
using System.Linq;
using GridSystem;
using Buildings;
using UI;
using AgentSystem;

public class GameManager : Singleton<GameManager>
{
    private GameState state; 
    public GameState State{get{return state ;} private set{state = value; onGameStateChanged?.Invoke(value);}}
    Action<GameState> onGameStateChanged;
    public GridMapManager GridMapManagerRef;
    public UIManager UIManagerRef;    
    public AgentManager AgentManagerRef;
    public List<Building> AvailableBuildings;

    public bool PlacingBuilding;

    private Building _buildingToPlace;

    [SerializeField]
    private List<Building> _placedBuildings; 

    [SerializeField]
    private Transform _buildingsParent;
    [SerializeField]
    private List<Agent> _selectedAgents;

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
            MapGrid tempGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _selectedAgents.Clear();
            if(tempGrid.isAgent)
            {
                _selectedAgents.Add(AgentManagerRef.GetAgent(tempGrid.Position));
                AgentManagerRef.SetSelectedAgents(_selectedAgents);
            }
            else if(tempGrid.Occupation != "")
            {
                UIManagerRef.UpdateDescriptionPanel(GetPlacedBuilding(tempGrid.Occupation));
                GetPlacedBuilding(tempGrid.Occupation).GetDamaged(10);
            }
        }
        else if(Input.GetMouseButtonUp(1))
        {
            MapGrid tempGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(tempGrid.Occupation == "")
            {
                AgentManagerRef.SpawnAgent("1",tempGrid.Position);
            }
           
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
            PlaceBuilding(hoveringGridLocation);
            PlacingBuilding = false;
        }
    }

    public void PlaceBuilding(Vector2Int startingLocation)
    {
        GridMapManagerRef.OccupyGrids(_buildingToPlace.BuildingID + "/" + _placedBuildings.Count , startingLocation , _buildingToPlace.Dimensions);
        _placedBuildings.Add(Instantiate(_buildingToPlace , GridMapManagerRef.GridToWorldLocation(startingLocation) + _buildingsParent.position ,Quaternion.identity , _buildingsParent));
        GridMapManagerRef.ResetHighlightedGrids();
        _placedBuildings.Last().baseGrid = startingLocation;
        _placedBuildings.Last().onDestroyed += DestroyBuilding;
        UIManagerRef.ClearClickedItems();
    }
    void DestroyBuilding(Building buildingToDestroy)
    {
        GameObject tempGO = _placedBuildings.Find(building => building == buildingToDestroy).gameObject;
        _placedBuildings.Remove(buildingToDestroy);
        GridMapManagerRef.UnOccupyGrids(buildingToDestroy.baseGrid, buildingToDestroy.Dimensions);
        Destroy(tempGO);
    }
    public Building GetPlacedBuilding(string uniqID)
    {
        return _placedBuildings[int.Parse(uniqID.Split('/')[1])];
    }
}
