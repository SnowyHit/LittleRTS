using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{

    public class GridMapManager : MonoBehaviour
    {
        //Keeping a reference List for my map. Now hardcoding it to 30x30 for simplicity's sake.
        private Grid[,] Grids = new Grid[30,30];
        private float cellSize;
        public GameObject tilePrefab;

        private Vector2Int lastLightedGridPosition ;
        
        private void Awake() 
        {
            cellSize = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            GenerateGrids();
        }
        private void Update() 
        {
            LightGrid(GetGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        void GenerateGrids()
        {
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    Grids[i,j] = new Grid(
                        Instantiate(tilePrefab , new Vector3((i*cellSize)+cellSize/2,(j*cellSize)+cellSize/2,0f),Quaternion.identity,this.transform) 
                        , 0 
                        ,new Vector2Int(i,j));
                }
            }
        }
        public byte GetGridWeight(Vector2Int position)
        {
            return Grids[position.x, position.y].Weight;
        }
        public Vector2Int GetGrid(Vector3 position)
        {
            return new Vector2Int((int)(position.x / cellSize), (int)(position.y / cellSize));
        }

        public void LightGrid(Vector2Int position)
        {
            if(!(position.x >= 0 && position.x < 30))
                return;
            if(!(position.y >= 0 && position.y < 30))
                return;
            if(position == lastLightedGridPosition)
                return;
            if(lastLightedGridPosition != null)
            {
                Grids[lastLightedGridPosition.x, lastLightedGridPosition.y].GameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            lastLightedGridPosition = position;
            Grids[position.x , position.y].GameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
