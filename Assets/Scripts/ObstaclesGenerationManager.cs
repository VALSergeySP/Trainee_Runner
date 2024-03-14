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

    [SerializeField] GameObject[] _obstaclesTopPrefabs;
    [SerializeField] GameObject[] _obstaclesLowPrefabs;
    [SerializeField] GameObject[] _obstaclesFullPrefabs;

    List<GameObject> generatedMaps = new();
    List<GameObject> activeMaps = new();

    enum ObjectsStyle { Line, Jump, Empty };

    [SerializeField] GameObject[] _objectPrefabs;
    [SerializeField] int _objectsSpawnProbability;
    [SerializeField] int _objectsInItem;
    [SerializeField] float _objectsSpawnHeight;

    struct MapObstacle
    {
        public void SetValues(GameObject obstacle, int lanePosition, ObjectsStyle type)
        {
            this.obstacle = obstacle;
            this.lanePosition = lanePosition;
            this.type = type;
        }

        public GameObject obstacle;
        public int lanePosition;
        public ObjectsStyle type;
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
        int variantNum;
        for (int i = 0; i < _linesOnMap; i++) // For every line on map
        {
            fullObstacles = 0;

            for (int j = -1; j <= 1; j++) // For every track on line
            {
                randomNumber = Random.Range(0, 4);

                switch(randomNumber)
                {
                    case 0:
                        variantNum = Random.Range(0, _obstaclesTopPrefabs.Length);
                        mapObstacle.SetValues(_obstaclesTopPrefabs[variantNum], j, ObjectsStyle.Line);
                        break;
                    case 1:
                        variantNum = Random.Range(0, _obstaclesLowPrefabs.Length);
                        mapObstacle.SetValues(_obstaclesLowPrefabs[variantNum], j, ObjectsStyle.Jump);
                        break;
                    case 2:
                        variantNum = Random.Range(0, _obstaclesFullPrefabs.Length);
                        mapObstacle.SetValues(_obstaclesFullPrefabs[variantNum], j, ObjectsStyle.Empty);
                        fullObstacles++;
                        break;
                    default:
                        mapObstacle.SetValues(null, j, ObjectsStyle.Line);
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

                CreateObjects(mapObstacles[0].type, obstaclePosition, map.transform);

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

    void CreateObjects(ObjectsStyle style, Vector3 position, Transform parentObject)
    {
        Vector3 objectPosition = Vector3.zero;
        Quaternion objectRotation;
        Vector2 randomRotationAngles;
        int randomNum;

        if (style == ObjectsStyle.Empty)
        {
            return;
        } else if(style == ObjectsStyle.Line)
        {
            if (_objectsSpawnProbability > Random.Range(0, 100))
            {
                for (int i = -_objectsInItem / 2; i < _objectsInItem / 2; i++)
                {

                    randomNum = Random.Range(0, _objectPrefabs.Length);
                    objectPosition.y = _objectsSpawnHeight;
                    objectPosition.z = i * ((float)_itemSpacingDistance / _objectsInItem);

                    randomRotationAngles.x = Random.Range(0f, 50f);
                    randomRotationAngles.y = Random.Range(0f, 360f);

                    objectRotation = Quaternion.Euler(randomRotationAngles.x, randomRotationAngles.y, 0);

                    GameObject createdObject = Singleton.Instance.PoolManagerInstance.Spawn(_objectPrefabs[randomNum], objectPosition + position, objectRotation);
                    createdObject.transform.SetParent(parentObject);
                }
            }
        }
        else if (style == ObjectsStyle.Jump)
        {
            if (_objectsSpawnProbability > Random.Range(0, 100))
            {
                for (int i = -_objectsInItem / 2; i < _objectsInItem / 2; i++)
                {

                    randomNum = Random.Range(0, _objectPrefabs.Length);
                    objectPosition.y = Mathf.Max(_objectsSpawnHeight, -1f / 2f * Mathf.Pow(i, 2) + 3);
                    objectPosition.z = i * ((float)_itemSpacingDistance / _objectsInItem);

                    randomRotationAngles.x = Random.Range(0f, 50f);
                    randomRotationAngles.y = Random.Range(0f, 360f);

                    objectRotation = Quaternion.Euler(randomRotationAngles.x, randomRotationAngles.y, 0);

                    GameObject createdObject = Singleton.Instance.PoolManagerInstance.Spawn(_objectPrefabs[randomNum], objectPosition + position, objectRotation);
                    createdObject.transform.SetParent(parentObject);
                }
            }
        }
    }
}
