using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Maps
{
    /// <summary>
    /// Генератор меша ландшафта
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        public GameObject barriersParent;

        private Mesh _mesh;
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;

        /// <summary>
        /// Вершины меша
        /// </summary>
        public Vector3[] Vertices { get; private set; }

        /// <summary>
        /// Треугольники поверхности
        /// </summary>
        public int[] Triangles { get; private set; }

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

        [SerializeField]
        private PoolManager _treePoolManager;

        [SerializeField]
        private PoolManager _stumpPoolManager;

        private List<GameObject> _trees = new();
        private List<GameObject> _stumps = new();

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
                Vertices = vertices;
                Triangles = triangles;
                UpdateShape();

                if (spawnBarriers)
                {
                    SpawnBarriers(false);
                }

                return;
            }

            Vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            Triangles = new int[xSize * zSize * 6];

            int index = 0;
            for (int z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    float y = Mathf.PerlinNoise(x * .3f, z * .3f) * surfaceCurve;
                    Vertices[index] = new Vector3(x, y, z);

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
                    Triangles[triangleIndex + 0] = vertexIndex + 0;
                    Triangles[triangleIndex + 1] = vertexIndex + xSize + 1;
                    Triangles[triangleIndex + 2] = vertexIndex + 1;
                    Triangles[triangleIndex + 3] = vertexIndex + 1;
                    Triangles[triangleIndex + 4] = vertexIndex + xSize + 1;
                    Triangles[triangleIndex + 5] = vertexIndex + xSize + 2;

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
            _mesh.vertices = Vertices;
            _mesh.triangles = Triangles;
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
                    Vertices[vertexIndex].y += lineX[x];
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
            FreeBarriers();

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
                int barrierChance = Random.Range(1, 24);
                if (barrierChance > 4 && barrierChance <= 8)
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

                    float y = Vertices[z * (xSize + 1) + x].y;

                    //если дерево появится на возвышенности то заного его подобрать
                    if (y > 1)
                    {
                        continue;
                    }

                    //чтобы деревья слишком рядом не спавнились
                    //for (int positionX = x - 4; positionX < 9; positionX++)
                    //{
                    //    xCoordinatesForTrees.Add(positionX);
                    //}

                    barrierChance = Random.Range(0, 5);
                    GameObject barrier;
                    if (barrierChance == 0)
                    {
                        barrier = _stumpPoolManager.GetFromPool();
                        _stumps.Add(barrier);
                        y -= 0.1f;
                    }
                    else
                    {
                        barrier = _treePoolManager.GetFromPool();
                        _trees.Add(barrier);
                        y += 1.6f;
                    }
                    barrier.transform.SetParent(barriersParent.transform);

                    int rotationX = Random.Range(-13, -6);
                    int rotationZ = Random.Range(-5, 6);
                    int rotationY = Random.Range(0, 180);

                    barrier.transform.SetLocalPositionAndRotation(
                        new Vector3(x, y, z),
                        Quaternion.Euler(rotationX, rotationY, rotationZ));

                }
            }
        }

        public void FreeBarriers()
        {
            _trees.ForEach(tree => _treePoolManager.ReturnObject(tree));
            _trees.Clear();
            _stumps.ForEach(stump => _stumpPoolManager.ReturnObject(stump));
            _stumps.Clear();
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
                        Vertices[index].y += height * heightLevel;
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
}