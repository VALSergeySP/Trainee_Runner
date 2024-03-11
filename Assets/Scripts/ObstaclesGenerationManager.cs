using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesGenerationManager : MonoBehaviour
{
    [SerializeField] int _itemSpacingDistance = 10;
    [SerializeField] int _linesOnMap = 5; // Колл-во линий с препятствиями на одной карте
    [SerializeField] int _mapsToGenerateOnStart = 10;


    [SerializeField] float _mapStartOffset = 10f;
    float _laneOffset = 2.5f;
    float _mapSize;
    public float LaneOffset => _laneOffset;

    [SerializeField] GameObject ObstaclesTopPrefab;
    [SerializeField] GameObject ObstaclesLowPrefab;
    [SerializeField] GameObject ObstaclesFullPrefab;

    List<GameObject> generatedMaps = new();
    List<GameObject> activeMaps = new();

    struct MapObstacle
    {
        public void SetValues(GameObject obstacle, int lanePosition)
        {
            this.obstacle = obstacle;
            this.lanePosition = lanePosition;
        }

        public GameObject obstacle;
        public int lanePosition;
    }


    private void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetAllActiveMaps;
        _mapSize = _linesOnMap * _itemSpacingDistance;

        for (int i = 0; i < _mapsToGenerateOnStart; i++)
        {
            generatedMaps.Add(GenerateNewMap());
        }

        foreach (var map in generatedMaps)
        {
            map.SetActive(false);
        }
    }

    void Update()
    {
        float speed = Singleton.Instance.LevelGenerationManagerInstance.CurrentSpeed;

        if(speed == 0) { return; }

        foreach (var map in activeMaps)
        {
            map.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }
        if (activeMaps[0].transform.position.z < -_mapSize)
        {
            RemoveFirstActiveMap();
            AddNewMap();
        }
    }

    public void ResetAllActiveMaps()
    {
        while(activeMaps.Count>0)
        {
            RemoveFirstActiveMap();
        }

        AddNewMap();
        AddNewMap();
        AddNewMap();
    }

    void RemoveFirstActiveMap()
    {
        activeMaps[0].SetActive(false);
        generatedMaps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    void AddNewMap()
    {
        int randomNumber = Random.Range(0, generatedMaps.Count);
        GameObject newMap = generatedMaps[randomNumber];
        newMap.SetActive(true);

        foreach(Transform child in newMap.transform)
        {
            child.gameObject.SetActive(true);
        }

        newMap.transform.position = activeMaps.Count > 0 ? activeMaps[^1].transform.position + Vector3.forward * _mapSize : new Vector3(0, 0, _mapStartOffset);

        generatedMaps.RemoveAt(randomNumber);
        activeMaps.Add(newMap);
    }

    GameObject GenerateNewMap()
    {
        GameObject map = new GameObject("Map");
        map.transform.SetParent(transform);

        List<MapObstacle> mapObstacles = new();
        MapObstacle mapObstacle = new();
        int fullObstacles;
        int randomNumber;
        for (int i = 0; i < _linesOnMap; i++) // For every line on map
        {
            fullObstacles = 0;

            for (int j = -1; j <= 1; j++) // For every track on line
            {
                randomNumber = Random.Range(0, 4);

                switch(randomNumber)
                {
                    case 0:
                        mapObstacle.SetValues(ObstaclesTopPrefab, j);
                        break;
                    case 1:
                        mapObstacle.SetValues(ObstaclesLowPrefab, j);
                        break;
                    case 2:
                        mapObstacle.SetValues(ObstaclesFullPrefab, j);
                        fullObstacles++;
                        break;
                    default:
                        mapObstacle.SetValues(null, j);
                        break;
                }

                mapObstacles.Add(mapObstacle);
            }


            if (fullObstacles >= 3) // На случай спавна 3 непроходимых стен убираем случайную из них
            {
                int randomPosition = Random.Range(0, 3); 
                mapObstacles.RemoveAt(randomPosition);
            }

            while (mapObstacles.Count > 0)
            {
                Vector3 obstaclePosition = new(mapObstacles[0].lanePosition * _laneOffset, 0, i * _itemSpacingDistance);

                if (mapObstacles[0].obstacle != null)
                {
                    GameObject lastObstacle = Singleton.Instance.PoolManagerInstance.Spawn(mapObstacles[0].obstacle, obstaclePosition, Quaternion.identity);
                    lastObstacle.transform.SetParent(map.transform);
                }

                mapObstacles.RemoveAt(0);
            }
        }

        return map;
    }
}
