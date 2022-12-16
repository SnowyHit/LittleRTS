using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Grid
    {
        public byte Weight;
        public Vector2Int Position;
        public byte Occupation;
        public GameObject GameObject;
        public Grid(GameObject go , byte weight , Vector2Int position)
        {
            GameObject = go ; 
            Weight = weight;
            Position = position ; 
            Occupation = 0 ;
        }
    }
}

