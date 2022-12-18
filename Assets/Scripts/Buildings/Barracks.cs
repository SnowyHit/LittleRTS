using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generic.Enums;

namespace Buildings
{
    public class Barracks : Building
    {
        public List<string> ProducableAgentIDs;
        [SerializeField]
        private Vector2Int _flagPoint;
        public Vector2Int FlagPoint {get{return _flagPoint;} private set {_flagPoint = value;}}

        public void SetFlagPoint(Vector2Int point)
        {
            FlagPoint = point;
        }
    }

}
