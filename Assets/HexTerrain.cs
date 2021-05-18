﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexTerrain : MonoBehaviour
{
    public int seed;

    [Range(1, 100)]
    public int width = 10;
    [Range(1, 100)]
    public int length = 10;

    public float scale = 1;
    public int octaves = 4;
    public float persistance = 1f;
    public float lacunarity = 1f;
    public Vector2 offset;

    public TerrainColorData terrainColorData;

    public float edgeHeight;

    public int selectionX = 0;
    public int selectionY = 0;

    Mesh mesh;

    public HexArray hexArray;

    private void Start()
    {
        CreateMesh();
    }

    public void CreateMesh()
    {
        float time = Time.realtimeSinceStartup;
        float[,] noiseMap = Noise.Generate2DNoiseMap(seed, width * 2, length * 2, scale, octaves, persistance, lacunarity, offset);
        Debug.Log("Noise generated");
        hexArray = HexArray.NoiseToHexTerrain(noiseMap, width, length, terrainColorData ?? TerrainColorData.EmptyDataObject);
        Debug.Log("Hex array generated");
        mesh = MeshGenerator.GenerateMesh(width, length, hexArray, edgeHeight);
        GetComponent<MeshFilter>().mesh = mesh;
        Debug.Log("Finished creating terrain in " + (Time.realtimeSinceStartup - time).ToString() + " seconds");
    }

    public static Vector3 HexPositionToWorldPosition(Vector3 hexPosition)
    {
        return new Vector3(hexPosition.x + (hexPosition.z % 2 == 0 ? 0f : -0.5f), hexPosition.y, hexPosition.z * HexArray.HALF_HEX_RADIUS * 3);
    }

    public static Quaternion HexDirectionToWorldRotation(int direction)
    {
        return Quaternion.Euler(0, 60 * direction - 90, 0);
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    private void OnDrawGizmos()
    {
        if (hexArray != null)
        {
            Hex hex = hexArray[selectionX, selectionY];
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hex.position + transform.position, 0.25f);
            Gizmos.color = Color.yellow;
            for (int i = 0; i < 6; i++)
            {
                Gizmos.DrawSphere(hexArray.GetNeighbour(selectionX, selectionY, (HexDirection)i).position + transform.position, 0.1f);
            }
        }
    }
}
