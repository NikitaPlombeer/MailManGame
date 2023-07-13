using System;
using System.Collections.Generic;
using City;
using DwarfTrains.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        public GameObject borderPrefab;
        
        public Transform envParent;
        public Transform obstaclesParent;

        public DeliveryBox boxPrefab;
        private List<Rectangle> rectangles;

        public Transform humanAndBox;
        
        public void GenerateCity()
        {
            this.seed = DateTime.Now.Millisecond;
            MakeCity();
            SpawnObstacles();
        }

        [Button("Clear obstacles")]
        public void ClearObstacles()
        {
            GameObjectUtils.ClearChildren(obstaclesParent);
        }
        
        [Button("Make City")]
        void MakeCity()
        {
            Debug.Log("Making city");
            ClearCity();

            Random.InitState(seed);

            cityGrid = new CityEnvType[mazeWidth, mazeHeight];
            this.rectangles = rectangleGridGenerator.Generate(mazeWidth, mazeHeight);
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

            SpawnBorders();
        }

        private void SpawnBorders()
        {

            for (int y = 0; y < mazeHeight; y += 3)
            {
                Instantiate(borderPrefab, new Vector3(0f, 0, -blockSize / 2f + y * blockSize), Quaternion.Euler(0f, 90f, 0f), envParent);
                Instantiate(borderPrefab, new Vector3(blockSize * mazeWidth - blockSize, 0, blockSize + y * blockSize), Quaternion.Euler(0f, -90f, 0f), envParent);
            }
            
            for (int x = 0; x < mazeWidth; x += 3)
            {
                Instantiate(borderPrefab, new Vector3(2.5f * blockSize + x * blockSize, 0, 0f ), Quaternion.Euler(0f, 0f, 0f), envParent);
                Instantiate(borderPrefab, new Vector3(x * blockSize - blockSize / 2f, 0, blockSize * mazeHeight - blockSize ), Quaternion.Euler(0f, 180f, 0f), envParent);
            }
            
            
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
            Instantiate(buildings[Random.Range(0, buildings.Count)], worldPosition, Quaternion.Euler(0f, angle, 0f), envParent);
        }

        [Button("SpawnObstacles")]
        public void SpawnObstacles()
        {
            Debug.Log("Making Obstacles");

            ClearObstacles();
            SpawnBox();
            SetHumanPosition();
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    var coord = new Vector2Int(x, y);

                    if (coord == boxCoord1 || coord == boxCoord2 || coord == humanCoord) continue;
                    
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

        public void SetHumanPosition()
        {
            var list = new List<Vector2Int>();
            for (int x = 3; x < mazeWidth / 2; x++)
            {
                for (int y = 3; y < mazeHeight / 2; y++)
                {
                    var coord = new Vector2Int(x, y);
                    if (
                        isType(coord, CityEnvType.ROAD) && 
                        isType(coord + Vector2Int.right, CityEnvType.ROAD) && 
                        isType(coord + Vector2Int.up, CityEnvType.ROAD) && 
                        isType(coord + Vector2Int.down, CityEnvType.ROAD))
                    {
                        list.Add(coord);
                    }
                }
            }
            
            this.humanCoord = list[Random.Range(0, list.Count)];
            var humanWorldPosition = GetWorldPosition(humanCoord);
            // humanWorldPosition.y = 0.5f;
            humanAndBox.position = humanWorldPosition;
        }

        private Vector2Int humanCoord;
        private Vector2Int boxCoord1;
        private Vector2Int boxCoord2;

        private void SpawnBox()
        {
            Debug.Log("Making box");
            var list = new List<Vector2Int>();
            for (int x = mazeWidth / 2; x < mazeWidth - 3; x++)
            {
                for (int y = mazeHeight / 2; y < mazeHeight - 3; y++)
                {
                    var coord = new Vector2Int(x, y);
                    if (isType(coord, CityEnvType.ROAD) && isType(coord + Vector2Int.right, CityEnvType.ROAD))
                    {
                        list.Add(coord);
                   
                    }
                }
            }
            
            this.boxCoord1 = list[Random.Range(0, list.Count)];
            this.boxCoord2 = boxCoord1 + Vector2Int.right;
            var w1 = GetWorldPosition(boxCoord1);
            var w2 = GetWorldPosition(boxCoord2 + Vector2Int.right);
                        
            var boxPos = (w1 + w2)/2f;
            boxPos.y = -2.3f;
            if (GameManager.Instanse != null)
            {
                GameManager.Instanse.deliveryBox = Instantiate(boxPrefab, boxPos, Quaternion.Euler(0f, 90f, 0), obstaclesParent);
                Debug.Log("Box spawned at " + GameManager.Instanse.deliveryBox.transform.position);
            }
        }
        
        private void SpawnFood(Vector2Int coord)
        {
            var worldPosition = GetWorldPosition(coord);
            Instantiate(foodCollectablePrefabs[Random.Range(0, foodCollectablePrefabs.Count)], worldPosition, Quaternion.identity, obstaclesParent);
        }
        
        private void SpawnCarObstacle(Vector2Int coord, float angle)
        {
            var r1 = GetWorldPosition(coord);
            Instantiate(carObstacles[Random.Range(0, carObstacles.Count)], (r1), Quaternion.Euler(0f, angle, 0f), obstaclesParent);
        }
        
        private void SpawnSideWalkObstacle(Vector2Int coord, float angle)
        {
            var worldPosition = GetWorldPosition(coord);
            Instantiate(sideWalkObstacles[Random.Range(0, sideWalkObstacles.Count)], worldPosition, Quaternion.Euler(0f, angle, 0f), obstaclesParent);
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
            Instantiate(cityEnvPrefab, worldPosition, Quaternion.identity, envParent);
            cityGrid[position.x, position.y] = cityEnvType;
        }
    
        
        [Button("Clear City")]
        public void ClearCity()
        {
            GameObjectUtils.ClearChildren(envParent);
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