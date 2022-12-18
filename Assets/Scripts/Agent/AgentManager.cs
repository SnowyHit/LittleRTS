using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;


namespace AgentSystem
{
    public class AgentManager : MonoBehaviour
    {
        [SerializeField]
        private List<Agent> _availableAgents;
        [SerializeField]
        private List<Agent> _activeAgents;
        [SerializeField]
        private List<Agent> _selectedAgents;
        public Transform AgentsParent;

        private void Update() {
            // if(_selectedAgents.Count > 0 && Input.GetMouseButtonDown(0))
            // {
            //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     foreach (Agent agent in _selectedAgents)
            //     {
            //         Vector2Int endGridLocation = GameManager.Instance.GridMapManagerRef.GetGridLocation(mousePos);
            //         Vector2Int startGridLocation = GameManager.Instance.GridMapManagerRef.GetGridLocation(agent.transform.position);
            //         agent.Move(GameManager.Instance.GridMapManagerRef.FindRouteAStar(startGridLocation ,endGridLocation));
            //     }
            // }
        }
        public Agent GetAgent(Vector2Int location)
        {
            foreach (Agent agent in _activeAgents)
            {
                if(agent.gridLocation == location)
                {
                    return agent;
                }
            }
            return null;
        }
        public Agent GetAgentFromAllAgents(string id)
        {
            foreach (Agent agent in _availableAgents)
            {
                if(agent.Id == id)
                {
                    return agent;
                }
            }
            return null;
        }
        public void SetSelectedAgents(List<Agent> agents)
        {
            _selectedAgents = agents;
        }
        public void SpawnAgent(string id , MapGrid spawnGrid , MapGrid flagGrid = null)
        {
            foreach (var item in _availableAgents)
            {
                if(item.Id == id)
                {
                    Agent tempAgent = Instantiate(item ,GameManager.Instance.GridMapManagerRef.GridToWorldLocation(spawnGrid.Position),Quaternion.identity, AgentsParent);
                    
                    if(tempAgent.type == Generic.Enums.AgentType.Soldier)
                    {
                        ((Soldier)tempAgent).onDead += RemoveAgent;
                    }
                    GameManager.Instance.GridMapManagerRef.OccupyGrids(tempAgent.Id + "/" + spawnGrid.Position , spawnGrid.Position , Vector2Int.one);
                    tempAgent.gridLocation = spawnGrid.Position;
                    tempAgent.onAgentMove += AgentMovementSyncWithGrids;
                    tempAgent.onStartMovement += AgentMovementStartUnoccupyGrids;
                    _activeAgents.Add(tempAgent);
                    spawnGrid.isAgent = true;

                    if(flagGrid != null)
                    {
                        tempAgent.Move(flagGrid.Position);
                    }
                }
            }
        }
        public void AgentMovementSyncWithGrids(Vector2Int movementStart , Vector2Int movementEnd , Agent agent)
        {
            MapGrid lastGrid = GameManager.Instance.GridMapManagerRef.GetGrid(movementStart);
            GameManager.Instance.GridMapManagerRef.OccupyGrids(agent.Id + "/" + movementEnd ,movementEnd, Vector2Int.one);
            GameManager.Instance.GridMapManagerRef.UnOccupyGrids(movementStart, Vector2Int.one);
            GameManager.Instance.GridMapManagerRef.GetGrid(movementEnd).isAgent = true;
        }
        public void AgentMovementStartUnoccupyGrids(Vector2Int startPos, Agent agent)
        {
            GameManager.Instance.GridMapManagerRef.UnOccupyGrids(startPos, Vector2Int.one);
        }
        public void RemoveAgent(Agent agentToRemove)
        {
            GameObject tempGO = _activeAgents.Find(agent => agent == agentToRemove).gameObject;
            _activeAgents.Remove(agentToRemove);
            GameManager.Instance.GridMapManagerRef.UnOccupyGrids(agentToRemove.gridLocation, Vector2Int.one);
            Destroy(tempGO);
        }
    }

}
