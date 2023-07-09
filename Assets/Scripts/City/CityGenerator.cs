using System.Collections.Generic;
using City;
using DwarfTrains.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.City
{
    public class CityGenerator: SerializedMonoBehaviour
    {

        [SerializeField] private int mazeWidth = 20;
        [SerializeField] private int mazeHeight = 20;
        [SerializeField] private int seed = 1;

        public float blockSize;
        
        public RectangleGridGenerator rectangleGridGenerator;

        public List<GameObject> buildings;
        public List<GameObject> carObstacles;
        public List<GameObject> sideWalkObstacles;
        
        public Dictionary<CityEnvType, GameObject> cityEnvPrefabs = new Dictionary<CityEnvType, GameObject>();

        private CityEnvType[,] cityGrid;
        
        public List<FoodCollectable> foodCollectablePrefabs;
        
        public float carsSpawnProbability = 0.1f;
        public float sideWalkSpawnProbability = 0.3f;
        public float foodSpawnProbability = 0.3f;
        
        [Button("Make City")]
        void MakeCity()
        {
            ClearCity();

            Random.InitState(seed);

            cityGrid = new CityEnvType[mazeWidth, mazeHeight];
            var rectangles = rectangleGridGenerator.Generate(mazeWidth, mazeHeight);
            Debug.Log("Maze generated");

            
            foreach (var rectangle in rectangles)
            {
                
                for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
                {
                    for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                    {
                        
                        var coord = new Vector2Int(x, y);
                        if (x == rectangle.X + rectangle.Width - 1) MakeCityItem(CityEnvType.ROAD, coord); else
                        if (y == rectangle.Y) MakeCityItem(CityEnvType.ROAD, coord); else
                        if (x == rectangle.X && y == rectangle.Y + 1) MakeCityItem(CityEnvType.SIDE_WALK_CORNER_BOTTOM_LEFT, coord); else
                        if (x == rectangle.X && y == rectangle.Y + rectangle.Height - 1) MakeCityItem(CityEnvType.SIDE_WALK_CORNER_TOP_LEFT, coord); else
                        if (x == rectangle.X + rectangle.Width - 2 && y == rectangle.Y + 1) MakeCityItem(CityEnvType.SIDE_WALK_CORNER_BOTTOM_RIGHT, coord); else
                        if (x == rectangle.X + rectangle.Width - 2 && y == rectangle.Y + rectangle.Height - 1) MakeCityItem(CityEnvType.SIDE_WALK_CORNER_TOP_RIGHT, coord); else 
                        if (x == rectangle.X) MakeCityItem(CityEnvType.SIDE_WALK_LEFT, coord); else
                        if (x == rectangle.X + rectangle.Width - 2) MakeCityItem(CityEnvType.SIDE_WALK_RIGHT, coord); else
                        if (y == rectangle.Y + 1) MakeCityItem(CityEnvType.SIDE_WALK_BOTTOM, coord); else
                        if (y == rectangle.Y + rectangle.Height - 1) MakeCityItem(CityEnvType.SIDE_WALK_TOP, coord); else 
                        SpawnBuilding(rectangle, coord);
                      
                    }
                }
                
            }

            SpawnObstacles();

        }

        private void SpawnBuilding(Rectangle rectangle, Vector2Int coord)
        {
            float angle = 0f;
            if (isBottomLeftBuildingCorner(rectangle, coord)) angle = 180f; else 
            if (isBottomRightBuildingCorner(rectangle, coord)) angle = 180f; else 
            if (isTopLeftBuildingCorner(rectangle, coord)) angle = 0f; else 
            if (isTopRightBuildingCorner(rectangle, coord)) angle = 0f; else 
            
            if (isBottomBuildingSide(rectangle, coord)) angle = 180f; else 
            if (isTopBuildingSide(rectangle, coord)) angle = 0f; else 
            if (isLeftBuildingSide(rectangle, coord)) angle = 90f; else 
            if (isRightBuildingSide(rectangle, coord)) angle = -90f;
            else
            {
                MakeCityItem(CityEnvType.SIDE_WALK_FILLED, coord);
                return;
            }

            cityGrid[coord.x, coord.y] = CityEnvType.BUILDING;
            var worldPosition = new Vector3(coord.x * blockSize, 0, coord.y * blockSize);
            Instantiate(buildings[0], worldPosition, Quaternion.Euler(0f, angle, 0f), transform);
        }

        public void SpawnObstacles()
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    var coord = new Vector2Int(x, y);

                    if (isType(coord, CityEnvType.ROAD)
                        && isType(coord + Vector2Int.down, CityEnvType.SIDE_WALK_TOP)
                        && isType(coord + Vector2Int.up, CityEnvType.SIDE_WALK_BOTTOM)
                        && Random.value < carsSpawnProbability
                       )
                    {
                        SpawnCarObstacle(coord, Random.value < 0.5f ? 0f : 180f);
                    } else if (isType(coord, CityEnvType.ROAD)
                               && isType(coord + Vector2Int.left, CityEnvType.SIDE_WALK_RIGHT)
                               && isType(coord + Vector2Int.right, CityEnvType.SIDE_WALK_LEFT)
                               && Random.value < carsSpawnProbability
                              )
                    {
                        SpawnCarObstacle(coord, Random.value < 0.5f ? 90f : -90f);
                    } else if (
                        (isType(coord, CityEnvType.SIDE_WALK_LEFT) || isType(coord, CityEnvType.SIDE_WALK_RIGHT))
                        && Random.value < sideWalkSpawnProbability
                    )
                    {
                        SpawnSideWalkObstacle(coord, Random.value < 0.5f ? 0f : 180f);
                    }else if (
                        (isType(coord, CityEnvType.SIDE_WALK_TOP) || isType(coord, CityEnvType.SIDE_WALK_BOTTOM))
                        && Random.value < sideWalkSpawnProbability
                    )
                    {
                        SpawnSideWalkObstacle(coord, Random.value < 0.5f ? 90f : -90f);
                    } else if (!isType(coord, CityEnvType.BUILDING)
                               && Random.value < foodSpawnProbability
                               )
                    {
                        SpawnFood(coord);
                    }
                }
            }
        }
        
        private void SpawnFood(Vector2Int coord)
        {
            var worldPosition = GetWorldPosition(coord);
            Instantiate(foodCollectablePrefabs[Random.Range(0, foodCollectablePrefabs.Count)], worldPosition, Quaternion.identity, transform);
        }
        
        private void SpawnCarObstacle(Vector2Int coord, float angle)
        {
            var r1 = GetWorldPosition(coord);
            Instantiate(carObstacles[Random.Range(0, carObstacles.Count)], (r1), Quaternion.Euler(0f, angle, 0f), transform);
        }
        
        private void SpawnSideWalkObstacle(Vector2Int coord, float angle)
        {
            var worldPosition = GetWorldPosition(coord);
            Instantiate(sideWalkObstacles[Random.Range(0, sideWalkObstacles.Count)], worldPosition, Quaternion.Euler(0f, angle, 0f), transform);
        }
        
        private Vector3 GetWorldPosition(Vector2Int coord)
        {
            return new Vector3(coord.x * blockSize, 0, coord.y * blockSize);
        }
        
        private bool isInBounds(Vector2Int coord)
        {
            return coord.x >= 0 && coord.x < mazeWidth && coord.y >= 0 && coord.y < mazeHeight;
        }
        
        private bool isType(Vector2Int coord, CityEnvType type)
        {
            return isInBounds(coord) && cityGrid[coord.x, coord.y] == type;
        }

        public bool isBottomLeftBuildingCorner(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x == rectangle.X + 1 && coord.y == rectangle.Y + 2;
        }
        
        public bool isBottomRightBuildingCorner(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x == rectangle.X + rectangle.Width - 3 && coord.y == rectangle.Y + 2;
        }
        
        public bool isTopLeftBuildingCorner(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x == rectangle.X + 1 && coord.y == rectangle.Y + rectangle.Height - 2;
        }
        
        public bool isTopRightBuildingCorner(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x == rectangle.X + rectangle.Width - 3 && coord.y == rectangle.Y + rectangle.Height - 2;
        }
        
        public bool isBottomBuildingSide(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x > rectangle.X + 1 && coord.x < rectangle.X + rectangle.Width - 3 && coord.y == rectangle.Y + 2;
        }
        
        public bool isTopBuildingSide(Rectangle rectangle, Vector2Int coord)
        {
            return coord.x > rectangle.X + 1 && coord.x < rectangle.X + rectangle.Width - 3 && coord.y == rectangle.Y + rectangle.Height - 2;
        }
        
        public bool isLeftBuildingSide(Rectangle rectangle, Vector2Int coord)
        {
            return coord.y > rectangle.Y + 2 && coord.y < rectangle.Y + rectangle.Height - 2 && coord.x == rectangle.X + 1;
        }
        
        public bool isRightBuildingSide(Rectangle rectangle, Vector2Int coord)
        {
            return coord.y > rectangle.Y + 2 && coord.y < rectangle.Y + rectangle.Height - 2 && coord.x == rectangle.X + rectangle.Width - 3;
        }
        
        private void MakeCityItem(CityEnvType cityEnvType, Vector2Int position)
        {
            var cityEnvPrefab = cityEnvPrefabs[cityEnvType];
            var worldPosition = new Vector3(position.x * blockSize, 0, position.y * blockSize);
            Instantiate(cityEnvPrefab, worldPosition, Quaternion.identity, transform);
            cityGrid[position.x, position.y] = cityEnvType;
        }
    
        
        [Button("Clear City")]
        public void ClearCity()
        {
            GameObjectUtils.ClearChildren(transform);
        }
    }
    
    public enum CityEnvType
    {
        ROAD,
        SIDE_WALK_BOTTOM,
        SIDE_WALK_TOP,
        SIDE_WALK_LEFT,
        SIDE_WALK_RIGHT,
        SIDE_WALK_FILLED,
        SIDE_WALK_CORNER_BOTTOM_RIGHT,
        SIDE_WALK_CORNER_BOTTOM_LEFT,
        SIDE_WALK_CORNER_TOP_RIGHT,
        SIDE_WALK_CORNER_TOP_LEFT,
        BUILDING
    }
}