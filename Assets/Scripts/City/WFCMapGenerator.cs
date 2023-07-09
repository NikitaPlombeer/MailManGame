using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map.MapGeneration
{
    // public class WFCMapGenerator
    // {
    //     private readonly Vector2Int mapSize;
    //     private CityItemSuperposition[,] mapSuperposition;
    //     private int seed;
    //     private Dictionary<WFCItemType, ItemAdjacencyPattern> adjacencyPatterns = new();
    //
    //     private Stack<Vector2Int> updatedSuperpositionPositions = new();
    //
    //     public WFCMapGenerator(Vector2Int mapSize, int seed)
    //     {
    //         this.mapSize = mapSize;
    //         this.seed = seed;
    //     }
    //
    //     public WFCItemType[,] Generate()
    //     {
    //         Random.InitState(seed);
    //
    //         this.mapSuperposition = CreateEmptySuperposition();
    //         InitSuperposition();
    //         InitWFCAdjacencyRules();
    //
    //         Collapse();
    //
    //         return GetResultBySuperposition();
    //     }
    //
    //     private void Collapse()
    //     {
    //         var maxIterations = 20000;
    //         var iterations = 0;
    //         while (!CheckSuperpositionForCompletion() && iterations < maxIterations)
    //         {
    //             iterations++;
    //             CollapseSuperposition();
    //         }
    //
    //         if (iterations >= maxIterations)
    //         {
    //             Debug.LogError("Max iterations reached");
    //             FillNotCollapsedSuperpositions();
    //         }
    //     }
    //
    //     private void FillNotCollapsedSuperpositions()
    //     {
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 var superposition = mapSuperposition[x, y];
    //                 if (superposition.IsCollapsed) continue;
    //                 superposition.possibleItems =
    //                     new List<WFCItemType>() {WFCItemType.SIDE_WALK_FILLED}; // TODO: need proper placeholder
    //             }
    //         }
    //     }
    //
    //     private void CollapseSuperposition()
    //     {
    //         while (updatedSuperpositionPositions.Count > 0)
    //         {
    //             var position = updatedSuperpositionPositions.Pop();
    //             CollapseSuperpositionPosition(position);
    //         }
    //
    //         if (CheckSuperpositionForCompletion()) return;
    //
    //
    //         var positionWithSmallestEntropy = GetPositionWithSmallestEntropy();
    //         var superposition = mapSuperposition[positionWithSmallestEntropy.x, positionWithSmallestEntropy.y];
    //
    //         var collapseNearbyValues = GetAdjacent(positionWithSmallestEntropy)
    //             .Where(pos => mapSuperposition[pos.x, pos.y].IsCollapsed)
    //             .Select(pos => mapSuperposition[pos.x, pos.y].CollapsedValue)
    //             .ToList();
    //
    //         if (collapseNearbyValues.Count == 0)
    //         {
    //             Debug.LogError("No nearby values");
    //         }
    //
    //         var probabilityMap = superposition.possibleItems
    //             .ToDictionary(h => h, h => 0f);
    //
    //         foreach (var nearbyValue in collapseNearbyValues)
    //         {
    //             var pattern = adjacencyPatterns[nearbyValue];
    //             var rules = pattern.rules
    //                 .Where(r => superposition.possibleItems.Contains(r.hexType))
    //                 .ToList();
    //
    //             foreach (var rule in rules)
    //             {
    //                 probabilityMap[rule.hexType] += rule.probability;
    //             }
    //         }
    //
    //         var sum = probabilityMap.Values.Sum();
    //         var probabilityTuples = probabilityMap.ToList()
    //             .Select(kv => new WFCProbability() {hexType = kv.Key, probability = kv.Value / sum})
    //             .OrderBy(t => t.probability)
    //             .ToList();
    //
    //         for (int i = 1; i < probabilityTuples.Count; i++)
    //         {
    //             WFCProbability probabilityTuple = probabilityTuples[i];
    //             probabilityTuple.probability += probabilityTuples[i - 1].probability;
    //             probabilityTuples[i] = probabilityTuple;
    //         }
    //
    //         var randomValue = Random.Range(0f, 1f);
    //         for (int i = 0; i < probabilityTuples.Count; i++)
    //         {
    //             if (randomValue < probabilityTuples[i].probability)
    //             {
    //                 superposition.possibleItems = new List<WFCItemType> {probabilityTuples[i].hexType};
    //                 updatedSuperpositionPositions.Push(positionWithSmallestEntropy);
    //                 break;
    //             }
    //         }
    //     }
    //
    //     private void CollapseSuperpositionPosition(Vector2Int position)
    //     {
    //         if (!mapSuperposition[position.x, position.y].IsCollapsed) return;
    //         var adjacentPositions = GetAdjacent(position);
    //
    //         var collapsedValue = mapSuperposition[position.x, position.y].CollapsedValue;
    //         foreach (var adjacentPosition in adjacentPositions)
    //         {
    //             var adjacentSuperPosition = mapSuperposition[adjacentPosition.x, adjacentPosition.y];
    //             var pattern = adjacencyPatterns[collapsedValue];
    //             var acceptableTypes = pattern.rules.Select(r => r.hexType).ToList();
    //
    //             var newpossibleItems = adjacentSuperPosition.possibleItems
    //                 .Where(h => acceptableTypes.Contains(h))
    //                 .ToList();
    //
    //             if (newpossibleItems.Count != adjacentSuperPosition.possibleItems.Count)
    //             {
    //                 adjacentSuperPosition.possibleItems = newpossibleItems;
    //                 if (adjacentSuperPosition.IsCollapsed)
    //                 {
    //                     updatedSuperpositionPositions.Push(adjacentPosition);
    //                 }
    //             }
    //         }
    //     }
    //
    //     private Vector2Int GetPositionWithSmallestEntropy()
    //     {
    //         var minEntropy = int.MaxValue;
    //         var minEntropyPosition = new Vector2Int();
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 if (mapSuperposition[x, y].IsCollapsed) continue;
    //
    //                 var entropy = mapSuperposition[x, y].possibleItems.Count;
    //                 if (entropy < minEntropy)
    //                 {
    //                     minEntropy = entropy;
    //                     minEntropyPosition = new Vector2Int(x, y);
    //                 }
    //             }
    //         }
    //
    //         return minEntropyPosition;
    //     }
    //
    //
    //     private List<Vector2Int> GetAdjacent(Vector2Int pos)
    //     {
    //         var basePosition = new List<Vector2Int>();
    //         basePosition.Add(new Vector2Int(pos.x, pos.y + 1));
    //         basePosition.Add(new Vector2Int(pos.x + 1, pos.y));
    //         basePosition.Add(new Vector2Int(pos.x + 1, pos.y - 1));
    //         basePosition.Add(new Vector2Int(pos.x, pos.y - 1));
    //
    //         return basePosition
    //             .Where(p => p.x >= 0 && p.x < mapSize.x && p.y >= 0 && p.y < mapSize.y)
    //             .Where(p => p.x != pos.x || p.y != pos.y)
    //             .ToList();
    //     }
    //
    //     private bool CheckSuperpositionForCompletion()
    //     {
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 if (!mapSuperposition[x, y].IsCollapsed)
    //                 {
    //                     return false;
    //                 }
    //             }
    //         }
    //
    //         return true;
    //     }
    //
    //     private void InitWFCAdjacencyRules()
    //     {
    //         adjacencyPatterns.Clear();
    //         
    //         adjacencyPatterns.Add(WFCItemType.ROAD, new ItemAdjacencyPattern()
    //         {
    //             left = new List<ItemAdjacencyRule>()
    //             {
    //                 new ItemAdjacencyRule() {hexType = WFCItemType.ROAD, probability = 0.5f},
    //                 new ItemAdjacencyRule() {hexType = WFCItemType.SIDE_WALK_FILLED, probability = 0f},
    //                 new ItemAdjacencyRule() {hexType = WFCItemType.SIDE_WALK_FILLED, probability = 0f},
    //             }
    //         });
    //     }
    //
    //
    //     private void InitSuperposition()
    //     {
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 if ((x == 0 || y == 0 || x == mapSize.x - 1 || y == mapSize.y - 1))
    //                 {
    //                     var type = WFCItemType.SIDE_WALK_FILLED;
    //                     if (x == 0) type = WFCItemType.SIDE_WALK_RIGHT;
    //                     if (y == 0) type = WFCItemType.SIDE_WALK_TOP;
    //                     if (x == mapSize.x - 1) type = WFCItemType.SIDE_WALK_LEFT;
    //                     if (y == mapSize.y - 1) type = WFCItemType.SIDE_WALK_BOTTOM;
    //                     
    //                     mapSuperposition[x, y].possibleItems = new List<WFCItemType> { type };
    //                     updatedSuperpositionPositions.Push(new Vector2Int(x, y));
    //                 }
    //             }
    //         }
    //     }
    //
    //
    //     private CityItemSuperposition[,] CreateEmptySuperposition()
    //     {
    //         var mapSuperposition = new CityItemSuperposition[mapSize.x, mapSize.y];
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 mapSuperposition[x, y] = new CityItemSuperposition();
    //             }
    //         }
    //
    //         return mapSuperposition;
    //     }
    //
    //     private WFCItemType[,] GetResultBySuperposition()
    //     {
    //         var result = new WFCItemType[mapSize.x, mapSize.y];
    //         for (int x = 0; x < mapSize.x; x++)
    //         {
    //             for (int y = 0; y < mapSize.y; y++)
    //             {
    //                 result[x, y] = mapSuperposition[x, y].possibleItems[0];
    //             }
    //         }
    //
    //         return result;
    //     }
    //
    // }
    //
    // public class CityItemSuperposition
    // {
    //     public List<WFCItemType> possibleItems = Enum.GetValues(typeof(WFCItemType)).Cast<WFCItemType>().ToList();
    //
    //     public bool IsCollapsed => possibleItems.Count == 1;
    //     public WFCItemType CollapsedValue => possibleItems[0];
    // }
    //
    //
    // [Serializable]
    // public enum WFCItemType
    // {
    //     ROAD,
    //     SIDE_WALK_BOTTOM,
    //     SIDE_WALK_TOP,
    //     SIDE_WALK_LEFT,
    //     SIDE_WALK_RIGHT,
    //     SIDE_WALK_FILLED,
    //     SIDE_WALK_CORNER_BOTTOM_RIGHT,
    //     SIDE_WALK_CORNER_BOTTOM_LEFT,
    //     SIDE_WALK_CORNER_TOP_RIGHT,
    //     SIDE_WALK_CORNER_TOP_LEFT,
    // }
    //
    //
    // [Serializable]
    // public class ItemAdjacencyPattern
    // {
    //     public WFCItemType hexType;
    //
    //     public List<ItemAdjacencyRule> left;
    //     public List<ItemAdjacencyRule> right;
    //     public List<ItemAdjacencyRule> top;
    //     public List<ItemAdjacencyRule> bottom;
    // }
    //
    // [Serializable]
    // public class ItemAdjacencyRule
    // {
    //     public WFCItemType hexType;
    //     public float probability;
    // }
    //
    //
    // public struct WFCProbability
    // {
    //     public WFCItemType hexType;
    //     public float probability;
    // }
}