using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{

    public class GridMapManager : MonoBehaviour
    {
        //Keeping a reference List for my map. Now hardcoding it to 30x30 for simplicity's sake.
        private MapGrid[,] _grids = new MapGrid[30,30];
        private float _cellSize;
        public GameObject TilePrefab;
        private Vector2Int lastLightedGridPosition ;
        
        private void Awake() 
        {
            _cellSize = TilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            GenerateGrids();
        }
        void GenerateGrids()
        {
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    GameObject tempGrid = Instantiate(TilePrefab , new Vector3((i*_cellSize)+_cellSize/2,(j*_cellSize)+_cellSize/2,0f),Quaternion.identity,this.transform);
                    tempGrid.name = "Grid("+i+","+j+")"; 
                    _grids[i,j] = new MapGrid(
                        tempGrid
                        , 0 
                        ,new Vector2Int(i,j));
                }
            }
        }
        public void OccupyGrids(string id , Vector2Int startingPoint ,Vector2Int dimensions)
        {
            if(isSpaceOccupied(startingPoint , dimensions))
                return;
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    _grids[startingPoint.x + x , startingPoint.y + y].Occupation = id;
                    _grids[startingPoint.x + x , startingPoint.y + y].GameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
        bool isSpaceOccupied(Vector2Int startingPoint ,Vector2Int dimensions)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    if(_grids[startingPoint.x + x , startingPoint.y + y].Occupation != "")
                        return true;
                }
            }
            return false;
        }
        public byte GetGridWeight(Vector2Int position)
        {
            return _grids[position.x, position.y].Weight;
        }
        public Vector2Int GetGrid(Vector3 position)
        {
            return new Vector2Int(Mathf.Clamp((int)(position.x / _cellSize) , 0, 29), Mathf.Clamp((int)(position.y / _cellSize) , 0, 29));
        }
        public void LightGrid(Vector2Int position , Vector2Int dimension)
        {
            if(position == lastLightedGridPosition)
                return;
            if(isSpaceOccupied(position , dimension))
            {
                HighlightGrids(lastLightedGridPosition , dimension , Color.red);
                return;
            }
            if(_grids[lastLightedGridPosition.x, lastLightedGridPosition.y].Occupation == "")
            {
               HighlightGrids(lastLightedGridPosition , dimension , Color.white);
            }
            if(_grids[position.x , position.y].Occupation == "")
            {
                lastLightedGridPosition = position;
                HighlightGrids(lastLightedGridPosition , dimension , Color.green);
            }
        }

        public void HighlightGrids(Vector2Int position , Vector2Int dimension , Color color)
        {
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    _grids[position.x + x , position.y + y].GameObject.GetComponent<SpriteRenderer>().color = color;
                }
            }
        }
    }
}
