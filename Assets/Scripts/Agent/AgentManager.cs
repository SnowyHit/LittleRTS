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

        public Transform AgentsParent;

        /// <summary>
        /// Returns an agent active on the field.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Agent GetAgent(Vector2Int location)
        {
            foreach (Agent agent in _activeAgents)
            {
                if (agent.gridLocation == location)
                {
                    return agent;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns and agent from available Agents list(just class references.)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Agent GetAgentFromAllAgents(string id)
        {
            foreach (Agent agent in _availableAgents)
            {
                if (agent.Id == id)
                {
                    return agent;
                }
            }
            return null;
        }

        /// <summary>
        /// Spawns agent with a given id , on the closest grid possible to spawn grid. Also moves them if flagGrid is given.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spawnGrid"></param>
        /// <param name="flagGrid"></param>
        public void SpawnAgent(string id, MapGrid spawnGrid, MapGrid flagGrid = null)
        {
            foreach (var item in _availableAgents)
            {
                if (item.Id == id)
                {
                    Agent tempAgent = Instantiate(item, GameManager.Instance.GridMapManagerRef.GridToWorldLocation(spawnGrid.Position), Quaternion.identity, AgentsParent);

                    if (tempAgent.type == Generic.Enums.AgentType.Soldier)
                    {
                        ((Soldier)tempAgent).onDead += RemoveAgent;
                    }
                    GameManager.Instance.GridMapManagerRef.OccupyGrids(tempAgent.Id + "-" + spawnGrid.Position, spawnGrid.Position, Vector2Int.one);
                    tempAgent.gridLocation = spawnGrid.Position;
                    tempAgent.onAgentMove += AgentMovementSyncWithGrids;
                    _activeAgents.Add(tempAgent);
                    spawnGrid.isAgent = true;

                    if (flagGrid != null)
                    {
                        tempAgent.Move(flagGrid.Position);
                    }
                }
            }
        }

        /// <summary>
        /// On Every AgentMove , set occupations.
        /// </summary>
        /// <param name="movementStart"></param>
        /// <param name="movementEnd"></param>
        /// <param name="agent"></param>
        public void AgentMovementSyncWithGrids(Vector2Int movementStart, Vector2Int movementEnd, Agent agent)
        {
            GameManager.Instance.GridMapManagerRef.OccupyGrids(agent.Id + "-" + movementEnd, movementEnd, Vector2Int.one);
            GameManager.Instance.GridMapManagerRef.UnOccupyGrids(movementStart, Vector2Int.one);
            GameManager.Instance.GridMapManagerRef.GetGrid(movementEnd).isAgent = true;
        }

        /// <summary>
        /// Removes agent from the game.
        /// </summary>
        /// <param name="agentToRemove"></param>
        public void RemoveAgent(Agent agentToRemove)
        {
            if (_activeAgents.Find(agent => agent == agentToRemove))
            {
                GameObject tempGO = _activeAgents.Find(agent => agent == agentToRemove).gameObject;
                _activeAgents.Remove(agentToRemove);
                GameManager.Instance.GridMapManagerRef.UnOccupyGrids(agentToRemove.gridLocation, Vector2Int.one);
                Destroy(tempGO);
            }
        }
    }

}
