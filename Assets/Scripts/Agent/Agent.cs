using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Generic.Enums;

namespace AgentSystem
{
    public class Agent : MonoBehaviour
    {
        public string Id;
        public AgentType type;
        public float MoveSpeed;
        public Vector2Int gridLocation;
        public List<Vector2Int> currentRoute;
        public Action<Vector2Int , Vector2Int ,  Agent> onAgentMove;
        public Action<Vector2Int , Agent> onStartMovement;
        public Action<Vector2Int , Agent> onMovementEnd;
        Coroutine _actorMovement;
        public Sprite AgentImage;
        public string AgentName;
        public void Move(Vector2Int endPosition)
        {
            if(endPosition == null)
                return;
            currentRoute = GameManager.Instance.GridMapManagerRef.FindRouteAStar(gridLocation,endPosition);
            if(currentRoute == null)
            {
                return;
            }
            if(_actorMovement != null)
            {
                StopCoroutine(_actorMovement);
            }
            _actorMovement = StartCoroutine(ActorMovement(endPosition));
        }
        IEnumerator ActorMovement(Vector2Int endPosition)
        {
            onStartMovement?.Invoke(gridLocation ,this);
            while(currentRoute.Count > 1)
            {
                if(GameManager.Instance.GridMapManagerRef.GetGrid(currentRoute[1]).Occupation != "")
                {
                    currentRoute[1] = GameManager.Instance.GridMapManagerRef.GetClosestUnoccupiedGrid(currentRoute[1]).Position;
                }
                transform.position = GameManager.Instance.GridMapManagerRef.GridToWorldLocation(currentRoute[1]);           
                gridLocation = currentRoute[1];
                onAgentMove?.Invoke(currentRoute[0] , currentRoute[1] , this);
                currentRoute.RemoveAt(0);
                yield return new WaitForSeconds(1f/MoveSpeed + UnityEngine.Random.Range(0,0.1f));
            }
            transform.position = GameManager.Instance.GridMapManagerRef.GridToWorldLocation(currentRoute[0]);
            gridLocation = currentRoute[0];
            onMovementEnd?.Invoke(gridLocation , this);
        }
    }
}
