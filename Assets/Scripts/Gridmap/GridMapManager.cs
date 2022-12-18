using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GridSystem
{
    public class GridMapManager : MonoBehaviour
    {
        //Keeping a reference List for my map. Now hardcoding it to 30x30 for simplicity's sake.
        private MapGrid[,] _grids = new MapGrid[30,30];
        private float _cellSize;
        public GameObject TilePrefab;
        public List<MapGrid> _highligtedGrids= new List<MapGrid>();
        
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
                    GameObject tempGrid = Instantiate(TilePrefab , new Vector3((i*_cellSize)+_cellSize/2,(j*_cellSize)+_cellSize/2, 2f),Quaternion.identity,this.transform);
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
            {
                ResetGridHighlights();
                return;
            }
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    _grids[startingPoint.x + x , startingPoint.y + y].Occupation = id;
                    _grids[startingPoint.x + x , startingPoint.y + y].Weight = byte.MaxValue;
                    _grids[startingPoint.x + x , startingPoint.y + y].GameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
         public void UnOccupyGrids(Vector2Int startingPoint ,Vector2Int dimensions)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    _grids[startingPoint.x + x , startingPoint.y + y].Occupation = "";
                    _grids[startingPoint.x + x , startingPoint.y + y].Weight = byte.MinValue;
                    _grids[startingPoint.x + x , startingPoint.y + y].GameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    _grids[startingPoint.x + x , startingPoint.y + y].isAgent = false;
                }
            }
        }
        public bool isSpaceOccupied(Vector2Int startingPoint ,Vector2Int dimensions)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    bool integrityOfLocation = (startingPoint.x + x > 29 || startingPoint.x + x <0)
                    || (startingPoint.y + y > 29 || startingPoint.y + y <0);
                    if(integrityOfLocation)
                        return true;
                    if(_grids[startingPoint.x + x , startingPoint.y + y].Occupation != "")
                        return true;
                }
            }
            return false;
        }
        public bool isGridTraversable(Vector2Int gridLocation)
        {
            if((gridLocation.x < 0 || gridLocation.x > 29)||(gridLocation.y < 0 || gridLocation.y > 29))
                return false;
            return _grids[gridLocation.x, gridLocation.y].Weight != byte.MaxValue;
        }
        public byte GetGridWeight(Vector2Int position)
        {
            return _grids[position.x, position.y].Weight;
        }
        public Vector2Int GetGridLocation(Vector3 position)
        {
            return new Vector2Int(Mathf.Clamp((int)(position.x / _cellSize) , 0, 29), Mathf.Clamp((int)(position.y / _cellSize) , 0, 29));
        }
        public MapGrid GetGrid(Vector3 position)
        {
            Vector2Int location = GetGridLocation(position);
            return _grids[location.x, location.y];
        }
        public MapGrid GetGrid(Vector2Int location)
        {
            if(location.x > 29 || location.x <0)
                return null;
            if(location.y > 29 || location.y <0)
                return null;
            return _grids[location.x, location.y];
        }

        public List<MapGrid> GetAllGrids(MapGrid startGrid , MapGrid endGrid )
        {
            List<MapGrid> tempList = new List<MapGrid>();
            int YStart = startGrid.Position.y;
            int YEnd = endGrid.Position.y;
            int XStart = startGrid.Position.x;
            int XEnd = endGrid.Position.x;
            if(YStart > YEnd)
            {
                YStart = endGrid.Position.y;
                YEnd = startGrid.Position.y;
            }
            if(XStart > XEnd)
            {
                XStart = endGrid.Position.x;
                XEnd = startGrid.Position.x;
            }
            for (int x = XStart ; x <= XEnd; x++)
            {
                for (int y = YStart; y <= YEnd; y++)
                {
                    tempList.Add(_grids[x,y]);
                }
            }
            return tempList;
        }
        public void HighlightGrids(Vector2Int position , Vector2Int dimension , Color color)
        {
            ResetGridHighlights();
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    if(_grids[position.x + x , position.y + y].Occupation == "")
                    {
                        _grids[position.x + x , position.y + y].GameObject.GetComponent<SpriteRenderer>().color = color;
                        _highligtedGrids.Add(_grids[position.x + x , position.y + y]);
                    }
                }
            }
        }
        public void ResetGridHighlights()
        {
            foreach (MapGrid grid in _highligtedGrids)
            {
                grid.GameObject.GetComponent<SpriteRenderer>().color = Color.white;
                grid.Occupation = "";
            }
        }
        public void ResetHighlightedGrids()
        {
            _highligtedGrids.Clear();
        }
        public Vector3 GridToWorldLocation(Vector2Int location)
        {
            return new Vector3(location.x * _cellSize , location.y * _cellSize , 0f);
        }
        public MapGrid GetClosestUnoccupiedGrid(Vector2Int grid)
        {
            HashSet<Vector2Int> pool = new HashSet<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); 
            Dictionary<Vector2Int, string> parents = new Dictionary<Vector2Int, string>();

            pool.Add(grid);
            while (pool.Count > 0)
            {
                var bestCell = pool.First();
                visited.Add(bestCell);
                pool.Remove(bestCell);
                var candidates = new List<Vector2Int> {
                    new Vector2Int(bestCell.x + 1, bestCell.y),
                    new Vector2Int(bestCell.x - 1, bestCell.y),
                    new Vector2Int(bestCell.x, bestCell.y + 1),
                    new Vector2Int(bestCell.x, bestCell.y - 1),
                };
                foreach (var candidate in candidates)
                {
                    if (visited.Contains(candidate))
                        continue;
                    if (!isSpaceOccupied(candidate , Vector2Int.one))
                        return GetGrid(candidate);
                    pool.Add(candidate);
                }
            }
            return null;            
        }
        public List<Vector2Int> FindRouteAStar(Vector2Int start, Vector2Int end)
        {
            if(!isGridTraversable(end))
            {
                end = GetClosestUnoccupiedGrid(end).Position;
            }
            HashSet<Vector2Int> pool = new HashSet<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); 
            Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();

            pool.Add(start);
            while (pool.Count > 0)
            {
                var bestCell = pool
                    .OrderBy(c => (c - start).sqrMagnitude +(5f * (end - c).sqrMagnitude))
                    .First();
                visited.Add(bestCell);
                pool.Remove(bestCell);
                var candidates = new List<Vector2Int> {
                    new Vector2Int(bestCell.x + 1, bestCell.y),
                    new Vector2Int(bestCell.x - 1, bestCell.y),
                    new Vector2Int(bestCell.x, bestCell.y + 1),
                    new Vector2Int(bestCell.x, bestCell.y - 1),
                };
                foreach (var candidate in candidates)
                {
                    if (visited.Contains(candidate) || !isGridTraversable(candidate))
                        continue;
                    parents[candidate] = bestCell;
                    if (candidate == end)
                        break;
                    pool.Add(candidate);
                }
                if (parents.ContainsKey(end))
                    break;
            }

            if (!parents.ContainsKey(end))
                return null;
            var route = new List<Vector2Int>();
            var cell = end;
            while (cell != start)
            {
                route.Insert(0, cell);
                cell = parents[cell];
            }
            return route;
        }
    }
}
