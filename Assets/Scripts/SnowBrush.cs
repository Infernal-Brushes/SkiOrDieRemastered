using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBrush : MonoBehaviour
{
    public CustomRenderTexture SnowHeightMap;
    public Material HeightMapUpdate;

    public float SecondsToRestore = 100;

    public GameObject[] Tires;
    public GameObject[] Paws;

    private Camera mainCamera;
    private int tireIndex;

    private float timeToRestoreOneTick;

    private static readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private static readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private static readonly int RestoreAmount = Shader.PropertyToID("_RestoreAmount");

    private void Start()
    {
        SnowHeightMap.Initialize();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // ���������������� ���� �� ���� �������, ����� ������� ����� ������� ����� ������ ����
                DrawWithMouse();
        //DrawWithTires();
        //        DrawWithPaws();

        // ������� ������ �� �������������� ������� ������� �������� �� �������� 
        timeToRestoreOneTick -= Time.deltaTime;
        if (timeToRestoreOneTick < 0)
        {
            // ���� � ���� update �� ����� ��������� ���� ���� �������� ����� ����� �� 1
            HeightMapUpdate.SetFloat(RestoreAmount, 1 / 250f);
            timeToRestoreOneTick = SecondsToRestore / 250f;
        }
        else
        {
            // ���� �� �����
            HeightMapUpdate.SetFloat(RestoreAmount, 0);
        }

        // ��������� �������� �������, ����� ��� ������ � ��������� Update Mode: Realtime
        SnowHeightMap.Update();
    }

    private void DrawWithMouse()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 hitTextureCoord = hit.textureCoord;

                HeightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
                HeightMapUpdate.SetFloat(DrawAngle, 45 * Mathf.Deg2Rad);
            }
        }
    }

    private void DrawWithTires()
    {
        GameObject tire = Tires[tireIndex++ % Tires.Length];

        Ray ray = new Ray(tire.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 hitTextureCoord = hit.textureCoord;
            float angle = tire.transform.rotation.eulerAngles.y;

            HeightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
            HeightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
        }
    }

    private void DrawWithPaws()
    {
        GameObject paw = Paws[tireIndex++ % Paws.Length];

        Ray ray = new Ray(paw.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.32f))
        {
            Vector2 hitTextureCoord = hit.textureCoord;
            float angle = 180 + paw.transform.rotation.eulerAngles.y;

            HeightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
            HeightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
        }
    }
}
