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

    Mesh mesh;
    MeshCollider meshCollider;

    Vector3[] vertices;
    int[] triangles;

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
                    vertices[index].y += height * heightLevel;
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
                vertices[index].y += lineX[x];
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

                float y = vertices[z * (xSize + 1) + x].y;

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


    public void CreateShape(float[] _lineX = null, Vector3[] firstLine = null)
    {
        mesh = new Mesh();
        mesh.name = "Snow";
        GetComponent<MeshFilter>().mesh.Clear();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int index = 0;
        int z = 0;


        for (; z <= zSize; z++)
        {
            //генерируем точки меша по x
            for (int x = 0; x <= zSize; x++)
            {
               
                float y;
                //для первого ряда ставим Y как у последнего ряда предыдущего мэша
                if ((z == 0 || z == 1/* ||  z == 2 || z == 3*/) && firstLine != null)
                    y = firstLine[index].y;
                else
                {
                    y = Mathf.PerlinNoise(x * .3f, z * .3f) * surfaceCurve;
                }

                vertices[index] = new Vector3(x, y, z);

                index++;
            }
            //if (z == 0)
            //    Debug.Log($"z0 last x index = {index}"); //131
        }


        //модификации спуска всякие
        SetCurveOfSurface();
        //if (firstLine != null)
        //{
        //    StartCoroutine(SpawnHills());
        //}

        // TODO: верни потом
        SpawnBarriers(firstLine == null);



        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

                //yield return new WaitForSeconds(0.00000001f);
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void GenerateNext()
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

        //коприуем две линии предыдущего меша чтоб был ровный стык
        Vector3[] lastLine = new Vector3[xSize*2+2];
        Array.Copy(vertices, vertices.Length - xSize*2 - 2, lastLine, 0, xSize*2 + 2);

        nextMeshGenerator.CreateShape(lineX, lastLine);

        nextMeshGenerator.triggerForGenerator.GetComponent<TriggerForGeneratorController>().previousMeshGenerator = this.gameObject;
    }
}
