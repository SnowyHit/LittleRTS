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
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
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

    private MapGrid MouseButtonStartGrid;

    private List<MapGrid> SelectedGrids = new List<MapGrid>();
    private List<String> occupationOfGrids = new List<String>();

    private void Start()
    {
        UIManagerRef.onProductionItemClicked += BuildingPlacementCheckRunner;
        UIManagerRef.onResetClick += ResetPlacingBuilding;
        UIManagerRef.onMilitaryItemClicked += SpawnMilitaryUnit;
    }
    private void Update() 
    {
        if(PlacingBuilding)
        {
            BuildingPlacementCheck();
            return;
        }
        CheckMouseInputs();
    }

    void CheckMouseInputs()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(Input.GetMouseButtonUp(1))
        {
            occupationOfGrids.Clear();
            MapGrid hoveringGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            foreach (MapGrid grid in SelectedGrids)
            {
                if(grid.Occupation == "" || occupationOfGrids.Contains(grid.Occupation))
                    continue;
                occupationOfGrids.Add(grid.Occupation);
                if(grid.isAgent)
                {
                    Agent tempAgent = AgentManagerRef.GetAgent(grid.Position);
                    if(tempAgent != null)
                    {
                        tempAgent.Move(hoveringGrid.Position);
                    }
                    if(hoveringGrid.Occupation != "" && tempAgent is Soldier)
                    {
                        ((Soldier)tempAgent).aimedLocation = hoveringGrid.Position;
                        tempAgent.onMovementEnd += Attack;
                    }
                }
                else
                {
                    Building tempBuilding = GetPlacedBuilding(grid.Occupation);
                    if(tempBuilding.Type == BuildingType.Barracks)
                    {
                        ((Barracks)tempBuilding).SetFlagPoint(hoveringGrid.Position); 
                    }
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            SelectedGrids.Clear();
            occupationOfGrids.Clear();
            UIManagerRef.ResetInformationMenu();
            MouseButtonStartGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if(Input.GetMouseButtonUp(0) && MouseButtonStartGrid != null)
        {
            MapGrid tempGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SelectedGrids = GridMapManagerRef.GetAllGrids(MouseButtonStartGrid ,tempGrid);
            foreach (MapGrid grid in SelectedGrids)
            {
                if(grid.Occupation == "" || occupationOfGrids.Contains(grid.Occupation))
                    continue;
                occupationOfGrids.Add(grid.Occupation);
                if(grid.isAgent)
                {
                    UIManagerRef.UpdateDescriptionPanel(AgentManagerRef.GetAgent(grid.Position));
                }
                else
                {
                    UIManagerRef.UpdateDescriptionPanel(GetPlacedBuilding(grid.Occupation));
                }
            }
            UIManagerRef.FillInformationMenu();
            MouseButtonStartGrid = null;
        }
    }

    public void Attack(Vector2Int location , Agent agentRef)
    {
        Debug.Log("asdasd");
        Soldier soldierRef = (Soldier)agentRef;
        string aimedGridOccupation = GridMapManagerRef.GetGrid(soldierRef.aimedLocation).Occupation;
        if(GetPlacedBuilding(aimedGridOccupation))
        {
            soldierRef.AttackBuildingRunner(GetPlacedBuilding(aimedGridOccupation));
        }
        else if(AgentManagerRef.GetAgent(soldierRef.aimedLocation))
        {
            Soldier soldierToAttack = (Soldier)AgentManagerRef.GetAgent(soldierRef.aimedLocation);
            soldierRef.AttackSoldierRunner(soldierToAttack);
        }
    }
    void BuildingPlacementCheckRunner(Building buildingToPlace)
    {
        PlacingBuilding = true;
        _buildingToPlace= buildingToPlace;
    }
    void ResetPlacingBuilding()
    {
        GridMapManagerRef.ResetGridHighlights();
        PlacingBuilding = false;
        _buildingToPlace= null;
    }
    void BuildingPlacementCheck()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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
    void SpawnMilitaryUnit(Building buildingRef, Agent agentRef)
    {
        AgentManagerRef.SpawnAgent(agentRef.Id , GridMapManagerRef.GetClosestUnoccupiedGrid(buildingRef.baseGrid) , GridMapManagerRef.GetGrid(((Barracks)buildingRef).FlagPoint));
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
        if(_placedBuildings.Find(building => building == buildingToDestroy))
        {
            GameObject tempGO = _placedBuildings.Find(building => building == buildingToDestroy).gameObject;
            _placedBuildings.Remove(buildingToDestroy);
            GridMapManagerRef.UnOccupyGrids(buildingToDestroy.baseGrid, buildingToDestroy.Dimensions);
            Destroy(tempGO);
        }
    }
    public Building GetPlacedBuilding(string uniqID)
    {
        if(uniqID == null)
        {
            return null;
        }
        return _placedBuildings[int.Parse(uniqID.Split('/')[1])];
    }
}
