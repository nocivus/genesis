using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FaceTerrain : MonoBehaviour {

    public GameObject chunkPrefab;
    public Chunk[,] chunks;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    TriangleList meshData;

    void init() {
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (meshFilter.sharedMesh == null) {
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.name = transform.name + "Mesh";
        }
        if (meshData == null) {
            meshData = new TriangleList();
        }
    }

    public void generate(int numberOfChunks, int quadsPerChunk, Algorithm algorithm) {

        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }
        meshFilter.sharedMesh = null;
        meshData = null;
        init();

        // Delete all existing chunks
        for (int i = 0; i < transform.childCount; i++) {
            DestroyImmediate(transform.GetChild(i).gameObject);
            i--;
        }

        // Generate chunk data
        chunks = new Chunk[numberOfChunks, numberOfChunks];
        float chunkSize = algorithm.getBaseRadius() / (float)numberOfChunks;
        for (int i = 0; i < numberOfChunks; i++) {
            for (int j = 0; j < numberOfChunks; j++) {

                float g = (float)i * chunkSize;
                float h = (float)j * chunkSize;
                Vector3 chunkPos = new Vector3(g, 0, h);
                chunkPos = transform.TransformPoint(chunkPos);

                chunks[i, j] = (Instantiate(chunkPrefab, chunkPos, transform.rotation) as GameObject).GetComponent<Chunk>();
                chunks[i, j].transform.parent = transform;
                chunks[i, j].name = "Chunk " + i + "_" + j;
                chunks[i, j].generate(chunkSize, quadsPerChunk, algorithm);
            }
        }

        // Generate simplified data
        float quads = 10f;
        float quadSize = algorithm.getBaseRadius() / quads;
        for (int i = 0; i < quads; i++) {
            for (int j = 0; j < quads; j++) {
                Chunk.createQuad((float)i * quadSize, (float)j * quadSize, quadSize, algorithm, meshData, transform);
            }
        }
    }

    public void createGeometry(Material material) {

        init();

        // Create geometry for the chunks
        foreach (Chunk c in chunks) {
            c.createGeometry(material);
        }

        // Create simplified geometry
        meshRenderer.material = material;
        meshData.GenerateData(meshFilter.sharedMesh);
    }

    public void handleVisibility(Transform player, float distanceToCenter, float horizon) {

        /*gameObject.SetActive(true);
        meshRenderer.enabled = true;
        for (int i = 0; i < chunks.GetLength(0); i++) {
            for (int j = 0; j < chunks.GetLength(1); j++) {
                chunks[i, j].gameObject.SetActive(false);
            }
        }

        return;*/

        if (chunks != null) {
            if (distanceToCenter > 200) {
                meshRenderer.enabled = true;
                for (int i = 0; i < chunks.GetLength(0); i++) {
                    for (int j = 0; j < chunks.GetLength(1); j++) {
                        chunks[i, j].gameObject.SetActive(false);
                    }
                }
            } else {
                // Handle within chunks
                meshRenderer.enabled = false;
                for (int i = 0; i < chunks.GetLength(0); i++) {
                    for (int j = 0; j < chunks.GetLength(1); j++) {
                        chunks[i, j].gameObject.SetActive(true);
                        chunks[i, j].handleVisibility(player, horizon);
                    }
                }
            }
        }
    }

    public Chunk closestChunk(Vector3 pos) {

        Chunk closest = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < chunks.GetLength(0); i++) {
            for (int j = 0; j < chunks.GetLength(1); j++) {
                float d = Vector3.Distance(pos, chunks[i, j].transform.position);
                if (d < closestDistance) {
                    closestDistance = d;
                    closest = chunks[i, j];
                }
            }
        }
        return closest;
    }

    public List<Vector3[]> getDebugLines(float radius) {

        List<Vector3[]> lines = new List<Vector3[]>();

        lines.Add(new Vector3[] { transform.position, transform.position + transform.right * radius });
        lines.Add(new Vector3[] { transform.position, transform.position + transform.forward * radius });
        lines.Add(new Vector3[] { transform.position + transform.right * radius, transform.position + (transform.forward + transform.right) * radius });
        lines.Add(new Vector3[] { transform.position + transform.forward * radius, transform.position + (transform.forward + transform.right) * radius });
        
        return lines;
    }
}
