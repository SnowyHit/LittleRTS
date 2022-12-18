using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgentSystem;

namespace GridSystem
{
    public class MapGrid
    {
        private byte weight;
        private Vector2Int position;
        private string occupation;
        private GameObject gameObject;
        public byte Weight{get{return weight;} set{weight = value;}}
        public Vector2Int Position{get{return position;} private set{ position = value;}}
        public bool isAgent;
        public string Occupation{get{return occupation;} set{occupation = value;}}
        public GameObject GameObject{get{return gameObject;} private set{ gameObject = value;}}
        public Color color;
        public MapGrid(GameObject go , byte weight , Vector2Int position)
        {
            GameObject = go ; 
            Weight = weight;
            Position = position ; 
            Occupation = "" ;
        }
    }
}

