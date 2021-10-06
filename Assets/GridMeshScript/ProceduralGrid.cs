using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGrid : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    //grid settings
    public float cellSize = 1;
    public Vector3 gridoffset;
    public int gridSize;

    void Awake()
    {
        this.mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Start()
    {
        MakeContigurousDiscrepteProceduralGrid();
        UpdateMesh();
    }

    void MakeDiscrepteProceduralGrid()
    {
        //Set array size
        this.vertices = new Vector3[gridSize * gridSize * 4];
        this.triangles = new int[gridSize * gridSize * 6];

        //Set tracker integers
        int v = 0;
        int t = 0;

        //Set vertex offset
        float vertex_offset = this.cellSize * 0.5f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 cellOffset = new Vector3(x * cellSize , 0 , y * cellSize);

                this.vertices[v + 0] = new Vector3(-vertex_offset, x + y, -vertex_offset) + cellOffset + gridoffset;
                this.vertices[v + 1] = new Vector3(-vertex_offset, x + y, vertex_offset) + cellOffset + gridoffset;
                this.vertices[v + 2] = new Vector3(vertex_offset, x + y, -vertex_offset) + cellOffset + gridoffset;
                this.vertices[v + 3] = new Vector3(vertex_offset, x + y, vertex_offset) + cellOffset + gridoffset;

                this.triangles[t] = v;
                this.triangles[t + 1] = this.triangles[t + 4] = v + 1;
                this.triangles[t + 2] = this.triangles[t + 3] = v + 2;
                this.triangles[t + 5] = v + 3;

                v += 4;
                t += 6;
            }
        }
    }
    void MakeContigurousDiscrepteProceduralGrid()
    {
        //Set array size
        this.vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
        this.triangles = new int[gridSize * gridSize * 6];

        //Set tracker integers
        int v = 0;
        int t = 0;

        //Set vertex offset
        float vertex_offset = this.cellSize * 0.5f;

        for (int x = 0; x <= gridSize; x++)
        {
            for (int y = 0; y <= gridSize; y++)
            {
                this.vertices[v] = new Vector3((x * cellSize) - vertex_offset, 0, (y * cellSize) - vertex_offset);
                v++;
            }
        }

        // reset vertex tracker
        v = 0;

        //setting each cell's triangles
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                this.triangles[t] = v;
                this.triangles[t + 1] = this.triangles[t + 4] = v + 1;
                this.triangles[t + 2] = this.triangles[t + 3] = v + (gridSize + 1);
                this.triangles[t + 5] = v + (gridSize + 1) + 1;
                v++;
                t += 6;
            }
            v++;
        }
    }
    void UpdateMesh()
    {
        this.mesh.Clear();
        this.mesh.vertices = this.vertices;
        this.mesh.triangles = this.triangles;

        this.mesh.RecalculateNormals();
    }
}
