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
        public Action<Vector2Int , Vector2Int , Agent> onAgentMove;
        public Action<Vector2Int , Agent> onStartMovement;
        Coroutine _actorMovement;
        public Sprite AgentImage;
        public string AgentName;
        public void Move(Vector2Int endPosition)
        {
            if(endPosition == null)
                return;
            currentRoute = GameManager.Instance.GridMapManagerRef.FindRouteAStar(gridLocation,endPosition);
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
                currentRoute = GameManager.Instance.GridMapManagerRef.FindRouteAStar(gridLocation,endPosition);

                transform.position = GameManager.Instance.GridMapManagerRef.GridToWorldLocation(currentRoute[0]);                
                gridLocation = currentRoute[0];
                onAgentMove?.Invoke(currentRoute[0] , currentRoute[1] , this);
                currentRoute.RemoveAt(0);
                yield return new WaitForSeconds(1f/MoveSpeed);
            }
            transform.position = GameManager.Instance.GridMapManagerRef.GridToWorldLocation(currentRoute[0]);
            gridLocation = currentRoute[0];
        }
    }
}
