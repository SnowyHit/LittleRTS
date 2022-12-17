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
        // Start is called before the first frame update
        public void Move(List<Vector2Int> route)
        {
            currentRoute = route;
            if(_actorMovement != null)
            {
                StopCoroutine(_actorMovement);
            }
            _actorMovement = StartCoroutine(ActorMovement());
        }
        IEnumerator ActorMovement()
        {
            onStartMovement?.Invoke(gridLocation ,this);
            while(currentRoute.Count > 1)
            {
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
