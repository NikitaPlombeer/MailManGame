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
        
        public Dictionary<CityEnvType, GameObject> cityEnvPrefabs = new Dictionary<CityEnvType, GameObject>();

        [Button("Make City")]
        void MakeCity()
        {
            ClearCity();

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

            var worldPosition = new Vector3(coord.x * blockSize, 0, coord.y * blockSize);
            Instantiate(buildings[0], worldPosition, Quaternion.Euler(0f, angle, 0f), transform);
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
    }
}