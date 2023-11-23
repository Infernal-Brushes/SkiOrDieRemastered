using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public GameObject triggerForGenerator;
    public GameObject barriersParent;
    public GameObject treePrefab;
    public GameObject stumpPrefab;

    Mesh _mesh;
    MeshCollider _meshCollider;
    MeshFilter _meshFilter;

    Vector3[] _vertices;
    int[] _triangles;

    public int xSize;
    public int zSize;
    public float surfaceCurve = 0.3f;

    /// <summary>
    /// координата по Z с которой начнёт расти деревья на стартовом меше
    /// </summary>
    private int startZLineToGenerateTrees = 42;

    /// <summary>
    /// линия кривой по X
    /// </summary>
    float[] lineX;
    CurveTendention firstTendention;
    int curvesCounter = 0;


    private void Awake()
    {
        lineX = new float[xSize + 1];

        _mesh = new Mesh { name = "Snow" };
        _meshCollider = GetComponent<MeshCollider>();
        _meshFilter = GetComponent<MeshFilter>();
    }

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

    private enum CurveTendention
    {
        none,
        goingDown,
        goingUp,
        goingStraight
    }
    private void SetCurveOfSurface()
    {
        curvesCounter++;

        int lengthOfIncreaseTendention = 33;
        int lengthOfStraightTendention = 15;
        int lengthOfCurrentDuration = 0;

        float minIncreaseStep = 0.04f;
        float maxIncreaseStep = 0.08f;
        float minStraightStep = 0.01f;
        float maxStraightStep = 0.02f;

        float minIncreaseStepIfNotFirst = 0.02f;
        float maxIncreaseStepIfNotFirst = 0.04f;
        float minStraightStepIfNotFirst = 0.005f;
        float maxStraightStepIfNotFirst = 0.01f;

        CurveTendention tendention;

        //if (lineX == null)
        //{

        //}
        bool firstCurve = false;
        if (firstTendention == CurveTendention.none)
        {
            firstCurve = true;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                tendention = CurveTendention.goingDown;
                firstTendention = CurveTendention.goingDown;
            }
            else
            {
                tendention = CurveTendention.goingUp;
                firstTendention = CurveTendention.goingUp;
            }
        }
        else
        {
            tendention = firstTendention;
        }
       
        CurveTendention lastTendention = tendention;

        //смена тенденций кривых по Z
        if (curvesCounter == 4)
        {
            //Debug.Log("Смена тенденции кривых по Z");
            curvesCounter = 0;
            if (tendention == CurveTendention.goingDown)
            {
                lastTendention = CurveTendention.goingUp;
                tendention = CurveTendention.goingStraight;
            }
            else if (tendention == CurveTendention.goingUp)
            {
                lastTendention = CurveTendention.goingDown;
                tendention = CurveTendention.goingStraight;
            }
            else if (tendention == CurveTendention.goingStraight)
            {
                if (firstTendention == CurveTendention.goingDown)
                    tendention = CurveTendention.goingUp;
                else if (firstTendention == CurveTendention.goingDown)
                    tendention = CurveTendention.goingDown;

                firstTendention = tendention;
            }
        }



        lineX[0] = 0;
        int index = 1;
        for (; index < xSize + 1; index++)
        {
            if (tendention == CurveTendention.goingDown)
            {
                lineX[index] = lineX[index - 1] - UnityEngine.Random.Range(minIncreaseStep, maxIncreaseStep);
            }
            else if (tendention == CurveTendention.goingUp)
            {
                lineX[index] = lineX[index - 1] + UnityEngine.Random.Range(minIncreaseStep, maxIncreaseStep);
            }
            else
            {
                float step = UnityEngine.Random.Range(minStraightStep, maxStraightStep);
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    lineX[index] = lineX[index - 1] -step;
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
            }
            else if (tendention == CurveTendention.goingStraight &&
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

        //строка с которой начинаем
        int z = 0;
        if (!firstCurve)
            z = 2;
        //начинаем с 0 элемента строки z = 1, то есть строку z = 0 пропускаем чтоб шов не сломать
        //то есть xSize+1 это первый элемент второй строки
        index = (xSize+1) * z;
        for(; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                _vertices[index].y += lineX[x];
                index++;
            }
        }
    }

    /// <summary>
    /// Заспавнить ёлок и пеньков
    /// </summary>
    /// <param name="isFirstMesh"></param>
    private void SpawnBarriers(bool isFirstMesh = false)
    {
        List<GameObject> barriers = new List<GameObject>();
        //кроме ближней линии стыка
        int z = 1;
        if (isFirstMesh)
            z = startZLineToGenerateTrees;

        //и кроме задней линии стыка
        for (; z < zSize; z++)
        {
            List<int> xCoordinatesForTrees = new List<int>();

            //такой шанс что на этой линии будут деревья
            int chanceToTree = UnityEngine.Random.Range(0, 16);
            if (chanceToTree != 0)
            {
                continue;
            }

            //Debug.Log(chanceToTree);
            int result = UnityEngine.Random.Range(1, 12);
            int countOfTreesForThisLine;
            //такой шанс что будет 1 дерево на линии
            if (result >= 1 && result <= 7)
            {
                countOfTreesForThisLine = 1;
            }
            else if (result >= 7 && result <= 10)
            {
                countOfTreesForThisLine = 2;
            }
            else if (result >= 10 && result <= 11)
            {
                countOfTreesForThisLine = 3;
            }
            else
            {
                countOfTreesForThisLine = 4;
            }


            for (int i = 0; i < countOfTreesForThisLine; i++)
            {
                int x;
                do
                {
                    x = UnityEngine.Random.Range(1, 130);
                }
                while (xCoordinatesForTrees.Contains(x));

                float y = _vertices[z * (xSize + 1) + x].y;

                //если дерево появится на снежном надуве то заного его подобрать
                if (y > 1)
                {
                    continue;
                }

                //чтобы деревья слишком рядом не спавнились
                for (int positionX = x -4; positionX < 9; positionX++)
                {
                    xCoordinatesForTrees.Add(positionX);
                }
       

                int rotationX = UnityEngine.Random.Range(-13, -6);
                int rotationZ = UnityEngine.Random.Range(-5, 6);

                    


                result = UnityEngine.Random.Range(0, 5);
                GameObject tree;
                if (result == 0)
                {
                    tree = Instantiate(stumpPrefab, barriersParent.transform);
                    //y += 0.1f;
                    y -= 0.1f;
                }
                else
                {
                    tree = Instantiate(treePrefab, barriersParent.transform);
                    y += 1.6f;
                }

                tree.transform.localPosition = new Vector3(x, y, z);
                tree.transform.localRotation = Quaternion.Euler(rotationX, UnityEngine.Random.Range(0, 180), rotationZ);
                barriers.Add(tree);
            }
        }
    }


    public void CreateShape(float[] _lineX = null, Vector3[] lastLine = null, bool spawnBarriers = true)
    {
        _vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        _triangles = new int[xSize * zSize * 6];

        int index = 0;
        for (int z = 0; z <= zSize; z++)
        {
            //генерируем точки меша по x
            for (int x = 0; x <= xSize; x++)
            {
                float y;
                //для первого ряда ставим Y как у последнего ряда предыдущего мэша
                if (z == 0 && lastLine != null)
                {
                    y = lastLine[index].y;
                }
                else
                {
                    y = Mathf.PerlinNoise(x * .3f, z * .3f) * surfaceCurve;
                }

                _vertices[index] = new Vector3(x, y, z);

                index++;
            }
        }

        //модификации спуска всякие
        SetCurveOfSurface();

        if (spawnBarriers)
        {
            SpawnBarriers(lastLine == null);
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

        _mesh.Clear();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = _mesh;
        _meshFilter.mesh = _mesh;
    }

    public void GenerateNext(bool spawnBarriers)
    {
        var _barriersParent = new GameObject();
    
        
        //_barriersParent.name = "Barriers";

        var nextMeshGenerator = Instantiate(this,
            new Vector3(transform.position.x + 112.434f, transform.position.y -64.932f, transform.position.z), 
            //new Vector3(transform.position.x + 109.8f, transform.position.y -63.53f, transform.position.z),
            transform.rotation);
        nextMeshGenerator.firstTendention = this.firstTendention;
        nextMeshGenerator.curvesCounter = this.curvesCounter;

        Destroy(nextMeshGenerator.barriersParent);
        _barriersParent.transform.parent = nextMeshGenerator.transform;
        _barriersParent.transform.localPosition = Vector3.zero;
        _barriersParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        nextMeshGenerator.barriersParent = _barriersParent;

        nextMeshGenerator.triggerForGenerator = nextMeshGenerator.GetComponentInChildren<TriggerForGeneratorController>().gameObject;

        Vector3[] lastLine = _vertices.TakeLast(xSize + 1).ToArray();
        nextMeshGenerator.CreateShape(lineX, lastLine, spawnBarriers);

        nextMeshGenerator.triggerForGenerator.GetComponent<TriggerForGeneratorController>().previousMeshGenerator = this.gameObject;
    }
}
