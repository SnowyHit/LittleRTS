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

    private List<Agent> SelectedAgents = new List<Agent>();
    private List<String> occupationOfGrids = new List<String>();

    private void Start()
    {
        UIManagerRef.onProductionItemClicked += BuildingPlacementCheckRunner;
        UIManagerRef.onResetClick += ResetPlacingBuilding;
        UIManagerRef.onMilitaryItemClicked += SpawnMilitaryUnit;
    }
    private void Update()
    {
        if (PlacingBuilding)
        {
            BuildingPlacementCheck();
            return;
        }
        CheckMouseInputs();
    }

    void CheckMouseInputs()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            occupationOfGrids.Clear(); // Clear occupations for preventing repetition
            MapGrid hoveringGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            foreach (MapGrid grid in SelectedGrids)
            {
                if (grid.Occupation == "" || occupationOfGrids.Contains(grid.Occupation)) // Checks if grid is valid.
                    continue;
                occupationOfGrids.Add(grid.Occupation);
                if (!grid.isAgent) // if hovering grid is a building, set flag of the building to there.
                {
                    Building tempBuilding = GetPlacedBuilding(grid.Occupation);
                    if (tempBuilding.Type == BuildingType.Barracks)
                    {
                        ((Barracks)tempBuilding).SetFlagPoint(hoveringGrid.Position);
                    }
                }
            }
            foreach (Agent agent in SelectedAgents)//If we selected any units before
            {
                if (agent != null)
                {
                    agent.Move(hoveringGrid.Position); //Move them to the hovering grid.
                    agent.onMovementEnd -= Attack; //Reset On Movement end action to prevent repetition.
                }
                if (hoveringGrid.Occupation != "" && agent is Soldier)
                {
                    ((Soldier)agent).aimedLocation = hoveringGrid.Position;
                    agent.onMovementEnd += Attack;//If hovering grid is a building/Unit , start attacking it when the movement is ended.
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            //Clear everything and wait for a MouseButtonUp.
            SelectedGrids.Clear();
            SelectedAgents.Clear();
            occupationOfGrids.Clear();
            UIManagerRef.ResetInformationMenu();
            MouseButtonStartGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(0) && MouseButtonStartGrid != null)
        {
            //Get every grid between mouse button down and mouseButton Up
            //Update UI , save selected grids/Units.
            MapGrid tempGrid = GridMapManagerRef.GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SelectedGrids = GridMapManagerRef.GetAllGrids(MouseButtonStartGrid, tempGrid);
            foreach (MapGrid grid in SelectedGrids)
            {
                if (grid.Occupation == "" || occupationOfGrids.Contains(grid.Occupation))
                    continue;
                occupationOfGrids.Add(grid.Occupation);
                if (grid.isAgent)
                {
                    SelectedAgents.Add(AgentManagerRef.GetAgent(grid.Position));
                }
                else
                {
                    UIManagerRef.UpdateDescriptionPanel(GetPlacedBuilding(grid.Occupation));
                }
            }
            foreach (Agent agent in SelectedAgents)
            {
                UIManagerRef.UpdateDescriptionPanel(agent);
            }
            UIManagerRef.FillInformationMenu();
            MouseButtonStartGrid = null;
        }
    }

    /// <summary>
    /// Given agents Attacks the thing on the given location.
    /// Does Nothing if given location is not occupied.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="agentRef"></param>
    public void Attack(Vector2Int location, Agent agentRef)
    {
        Soldier soldierRef = (Soldier)agentRef;
        string aimedGridOccupation = GridMapManagerRef.GetGrid(soldierRef.aimedLocation).Occupation;
        if (GetPlacedBuilding(aimedGridOccupation))
        {
            soldierRef.AttackBuildingRunner(GetPlacedBuilding(aimedGridOccupation));
        }
        else if (AgentManagerRef.GetAgent(soldierRef.aimedLocation))
        {
            Soldier soldierToAttack = (Soldier)AgentManagerRef.GetAgent(soldierRef.aimedLocation);
            soldierRef.AttackSoldierRunner(soldierToAttack);
        }
    }
    void BuildingPlacementCheckRunner(Building buildingToPlace)
    {
        PlacingBuilding = true;
        _buildingToPlace = buildingToPlace;
    }
    void ResetPlacingBuilding()
    {
        GridMapManagerRef.ResetGridHighlights();
        PlacingBuilding = false;
        _buildingToPlace = null;
    }
    void BuildingPlacementCheck()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //Checks if UI is on top of UI
        {
            return;
        }

        Vector2Int hoveringGridLocation = GridMapManagerRef.GetGridLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        bool isOccupied = GridMapManagerRef.isSpaceOccupied(hoveringGridLocation, _buildingToPlace.Dimensions);
        if (isOccupied)
        {
            GridMapManagerRef.HighlightGrids(hoveringGridLocation, _buildingToPlace.Dimensions, Color.red);
        }
        else
        {
            GridMapManagerRef.HighlightGrids(hoveringGridLocation, _buildingToPlace.Dimensions, Color.green);
        }

        if (Input.GetMouseButtonDown(0) && !isOccupied)
        {
            PlaceBuilding(hoveringGridLocation);
            PlacingBuilding = false;
        }
    }
    void SpawnMilitaryUnit(Building buildingRef, Agent agentRef)
    {
        AgentManagerRef.SpawnAgent(agentRef.Id, GridMapManagerRef.GetClosestUnoccupiedGrid(buildingRef.baseGrid), GridMapManagerRef.GetGrid(((Barracks)buildingRef).FlagPoint));
    }

    public void PlaceBuilding(Vector2Int startingLocation)
    {
        GridMapManagerRef.OccupyGrids(_buildingToPlace.BuildingID + "/" + _placedBuildings.Count, startingLocation, _buildingToPlace.Dimensions);
        _placedBuildings.Add(Instantiate(_buildingToPlace, GridMapManagerRef.GridToWorldLocation(startingLocation) + _buildingsParent.position, Quaternion.identity, _buildingsParent));
        GridMapManagerRef.ResetHighlightedGrids();
        _placedBuildings.Last().baseGrid = startingLocation;
        _placedBuildings.Last().onDestroyed += DestroyBuilding;
        UIManagerRef.ClearClickedItems();
    }
    void DestroyBuilding(Building buildingToDestroy)
    {
        if (_placedBuildings.Find(building => building == buildingToDestroy))
        {
            GameObject tempGO = _placedBuildings.Find(building => building == buildingToDestroy).gameObject;
            _placedBuildings.Remove(buildingToDestroy);
            GridMapManagerRef.UnOccupyGrids(buildingToDestroy.baseGrid, buildingToDestroy.Dimensions);
            Destroy(tempGO);
        }
    }

    /// <summary>
    /// Returns placed building in a grid , if given uniqID is correct.
    /// </summary>
    /// <param name="uniqID"></param>
    /// <returns></returns>
    public Building GetPlacedBuilding(string uniqID)
    {
        if (uniqID == null)
        {
            return null;
        }
        if (uniqID.Split('/').Count() == 1)
        {
            return null;
        }
        return _placedBuildings[int.Parse(uniqID.Split('/')[1])];
    }
}
