using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Генератор меша ландшафта
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public GameObject triggerForGenerator;
    public GameObject barriersParent;
    public GameObject treePrefab;
    public GameObject stumpPrefab;

    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private MeshFilter _meshFilter;
    
    /// <summary>
    /// Вершины меша
    /// </summary>
    private Vector3[] _vertices;

    /// <summary>
    /// Треугольники поверхности
    /// </summary>
    private int[] _triangles;

    /// <summary>
    /// Ширина поля
    /// </summary>
    public int xSize;

    /// <summary>
    /// Длина поля
    /// </summary>
    public int zSize;

    /// <summary>
    /// Коеффициент кривизны поверхности
    /// </summary>
    public float surfaceCurve = 0.3f;

    /// <summary>
    /// Координата по Z с которой начнут расти преграды на стартовом меше
    /// </summary>
    [Tooltip("Координата по Z с которой начнут расти преграды на стартовом меше")]
    [SerializeField]
    private int startZLineToGenerateTrees = 42;

    /// <summary>
    /// Линия искревлений по X
    /// </summary>
    float[] lineX = null;

    private void Awake()
    {
        _mesh = new Mesh { name = "Snow" };
        _meshCollider = GetComponent<MeshCollider>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    

    private enum CurveTendention
    {
        none,
        goingDown,
        goingUp,
        goingStraight
    }

    /// <summary>
    /// Создать меш поверхности
    /// </summary>
    /// <param name="spawnBarriers"></param>
    /// <param name="vertices"></param>
    /// <param name="triangles"></param>
    public void CreateShape(bool spawnBarriers = true, Vector3[] vertices = null, int[] triangles = null)
    {
        if (vertices is not null && triangles is not null)
        {
            _vertices = vertices;
            _triangles = triangles;
            UpdateShape();

            if (spawnBarriers)
            {
                SpawnBarriers(false);
            }

            return;
        }

        _vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        _triangles = new int[xSize * zSize * 6];

        int index = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * surfaceCurve;
                _vertices[index] = new Vector3(x, y, z);

                index++;
            }
        }

        SetCurveOfSurface();

        if (spawnBarriers)
        {
            SpawnBarriers(true);
        }

        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                _triangles[triangleIndex + 0] = vertexIndex + 0;
                _triangles[triangleIndex + 1] = vertexIndex + xSize + 1;
                _triangles[triangleIndex + 2] = vertexIndex + 1;
                _triangles[triangleIndex + 3] = vertexIndex + 1;
                _triangles[triangleIndex + 4] = vertexIndex + xSize + 1;
                _triangles[triangleIndex + 5] = vertexIndex + xSize + 2;

                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }

        UpdateShape();
    }

    /// <summary>
    /// Обновить меш поверхности
    /// </summary>
    private void UpdateShape()
    {
        _mesh.Clear();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = _mesh;
        _meshFilter.mesh = _mesh;
    }

    /// <summary>
    /// Искривить поверхность меша
    /// </summary>
    private void SetCurveOfSurface()
    {
        lineX = new float[xSize + 1];

        int lengthOfIncreaseTendention = 33;
        int lengthOfStraightTendention = 15;
        int lengthOfCurrentDuration = 0;

        float minIncreaseStep = 0.04f;
        float maxIncreaseStep = 0.08f;
        float minStraightStep = 0.01f;
        float maxStraightStep = 0.02f;

        CurveTendention tendention = Random.Range(0, 2) == 0
            ? CurveTendention.goingDown
            : CurveTendention.goingUp;

        CurveTendention lastTendention = tendention;

        lineX[0] = 0;
        for (int index = 1; index < xSize + 1; index++)
        {
            if (tendention == CurveTendention.goingDown)
            {
                lineX[index] = lineX[index - 1] - Random.Range(minIncreaseStep, maxIncreaseStep);
            }
            else if (tendention == CurveTendention.goingUp)
            {
                lineX[index] = lineX[index - 1] + Random.Range(minIncreaseStep, maxIncreaseStep);
            }
            else
            {
                float step = Random.Range(minStraightStep, maxStraightStep);
                if (Random.Range(0, 2) == 0)
                {
                    lineX[index] = lineX[index - 1] - step;
                }
                else
                {
                    lineX[index] = lineX[index - 1] + step;
                }
            }

            lengthOfCurrentDuration++;

            if ((tendention == CurveTendention.goingDown ||
                tendention == CurveTendention.goingUp) &&
                lengthOfCurrentDuration > lengthOfIncreaseTendention)
            {
                tendention = CurveTendention.goingStraight;
                lengthOfCurrentDuration = 0;

                continue;
            }

            if (tendention == CurveTendention.goingStraight &&
                lengthOfCurrentDuration > lengthOfStraightTendention)
            {
                if (lastTendention == CurveTendention.goingDown)
                {
                    tendention = CurveTendention.goingUp;
                }
                else
                {
                    tendention = CurveTendention.goingDown;
                }
                lastTendention = tendention;
                lengthOfCurrentDuration = 0;
            }
        }

        int vertexIndex = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                _vertices[vertexIndex].y += lineX[x];
                vertexIndex++;
            }
        }
    }

    /// <summary>
    /// Заспавнить ёлок и пеньков
    /// </summary>
    /// <param name="isFirstMesh"></param>
    private void SpawnBarriers(bool isFirstMesh = false)
    {
        //кроме ближней линии стыка
        int z = 1;
        if (isFirstMesh)
        {
            z = startZLineToGenerateTrees;
        }

        //и кроме задней линии стыка
        for (; z < zSize; z++)
        {
            List<int> xCoordinatesForTrees = new();

            //такой шанс что на этой линии будут деревья
            int chanceToTree = Random.Range(0, 12);
            if (chanceToTree != 0)
            {
                continue;
            }

            // шансы сколько будет деревьев на линии
            int countOfTreesForThisLine = 1;
            int barrierChance = Random.Range(1, 16);
            if (barrierChance > 4 && barrierChance <= 10)
            {
                countOfTreesForThisLine = 2;
            }
            else if (barrierChance > 10)
            {
                countOfTreesForThisLine = 3;
            }

            for (int i = 0; i < countOfTreesForThisLine; i++)
            {
                int x;
                do
                {
                    x = Random.Range(1, xSize - 1);
                }
                while (xCoordinatesForTrees.Contains(x));

                float y = _vertices[z * (xSize + 1) + x].y;

                //если дерево появится на возвышенности то заного его подобрать
                if (y > 1)
                {
                    continue;
                }

                //чтобы деревья слишком рядом не спавнились
                for (int positionX = x - 4; positionX < 9; positionX++)
                {
                    xCoordinatesForTrees.Add(positionX);
                }

                barrierChance = Random.Range(0, 5);
                GameObject barrier;
                if (barrierChance == 0)
                {
                    barrier = Instantiate(stumpPrefab, barriersParent.transform);
                    y -= 0.1f;
                }
                else
                {
                    barrier = Instantiate(treePrefab, barriersParent.transform);
                    y += 1.6f;
                }

                int rotationX = Random.Range(-13, -6);
                int rotationZ = Random.Range(-5, 6);
                int rotationY = Random.Range(0, 180);

                barrier.transform.SetLocalPositionAndRotation(
                    new Vector3(x, y, z),
                    Quaternion.Euler(rotationX, rotationY, rotationZ));
            }
        }
    }

    /// <summary>
    /// Создать следующую поверхность
    /// </summary>
    /// <param name="spawnBarriers"></param>
    public void GenerateNext(bool spawnBarriers)
    {
        Vector3 originalPosition = transform.position;
        Vector3 direction = transform.rotation * Vector3.forward;
        // Немного меньше чтобы спрятать шов
        Vector3 displacement = direction * (zSize - 0.08f);
        Vector3 newPosition = originalPosition + displacement;

        var _barriersParent = new GameObject();

        var nextMeshGenerator = Instantiate(this,
            newPosition,
            transform.rotation);
        nextMeshGenerator.name = name;

        Destroy(nextMeshGenerator.barriersParent);
        _barriersParent.transform.parent = nextMeshGenerator.transform;
        _barriersParent.transform.localPosition = Vector3.zero;
        _barriersParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        nextMeshGenerator.barriersParent = _barriersParent;

        nextMeshGenerator.triggerForGenerator = nextMeshGenerator.GetComponentInChildren<TriggerForGeneratorController>().gameObject;
        nextMeshGenerator.CreateShape(spawnBarriers, _vertices, _triangles);
        nextMeshGenerator.triggerForGenerator.GetComponent<TriggerForGeneratorController>().previousMeshGenerator = gameObject;
    }

    /// <summary>
    /// Заспавнить трамплины
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnHills()
    {
        int result = UnityEngine.Random.Range(0, 17);
        int hillsCount = 0;
        if (result >= 3 && result <= 7)
        {
            hillsCount = 1;
        }
        else if (result >= 8 && result <= 11)
        {
            hillsCount = 2;
        }
        else if (result >= 12 || result <= 14)
        {
            hillsCount = 3;
        }
        else if (result >= 15 && result <= 16)
        {
            hillsCount = 4;
        }

        float height = 0.4f;
        while (hillsCount > 0)
        {
            int z = UnityEngine.Random.Range(2, zSize - 34);
            int x = UnityEngine.Random.Range(9, xSize - 9);
            int index = z * (xSize + 1) + x;
            int lengthZ = UnityEngine.Random.Range(12, 32);
            int lengthX = UnityEngine.Random.Range(3, 6);
            int thirdPartOfLengthZ = lengthZ / 3;
            float heightLevel = 1;

            //вот это добавил если что удали и в цикле чтоб ++ было
            index = index + lengthX;
            for (int line = 1; line <= lengthZ; line++)
            {
                for (int indexX = 0; indexX < lengthX; indexX++)
                {
                    _vertices[index].y += height * heightLevel;
                    //index++;
                    index--;
                }


                int deltaX;
                //возрастание в начале
                if (line <= thirdPartOfLengthZ)
                {
                    deltaX = UnityEngine.Random.Range(0, 4);
                    lengthX += deltaX;
                    heightLevel += height;
                }
                //убавание в конце
                else if (line >= lengthZ - thirdPartOfLengthZ)
                {
                    deltaX = UnityEngine.Random.Range(0, 4);
                    lengthX += deltaX;
                    heightLevel -= height;
                }
                //середина
                else
                {
                    deltaX = UnityEngine.Random.Range(-1, 3);
                    lengthX += deltaX;
                    heightLevel -= height;
                }
                if (heightLevel < 0)
                    heightLevel = 0;

                z++;
                if (deltaX < 0)
                    deltaX = 0;
                x -= UnityEngine.Random.Range(1, deltaX + 1);
                index = z * (xSize + 1) + x;
            }

            hillsCount--;
        }
        yield return null;
    }
}
