using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour {

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public TriangleList meshData;

    void init() {
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (meshFilter.sharedMesh == null) {
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.name = transform.parent.name + "Mesh";
        }
        if (meshData == null) {
            meshData = new TriangleList();
        }
    }

    public void generate(float chunkSize, int quads, Algorithm algorithm) {

        init();

        // Create "quads" quads in our mesh
        float quadSize = chunkSize / (float)quads;
        for (int i = 0; i < quads; i++) {
            for (int j = 0; j < quads; j++) {
                Chunk.createQuad((float)i * quadSize, (float)j * quadSize, quadSize, algorithm, meshData, transform);
            }
        }
    }

    public static void createQuad(float i, float j, float quadSize, Algorithm algorithm, TriangleList meshData, Transform transform) {

        // North
        Vertex a = new Vertex(i, 0, j);
        Vertex b = new Vertex(i, 0, j + quadSize);
        Vertex c = new Vertex(i + quadSize, 0, j + quadSize);
        Vertex d = new Vertex(i + quadSize, 0, j);

        // Apply terrain features based on the algorithm
        a.position = transform.TransformPoint(a.position);
        b.position = transform.TransformPoint(b.position);
        c.position = transform.TransformPoint(c.position);
        d.position = transform.TransformPoint(d.position);
        a.position = transform.InverseTransformPoint(a.position.normalized * algorithm.getHeight(a.position));
        b.position = transform.InverseTransformPoint(b.position.normalized * algorithm.getHeight(b.position));
        c.position = transform.InverseTransformPoint(c.position.normalized * algorithm.getHeight(c.position));
        d.position = transform.InverseTransformPoint(d.position.normalized * algorithm.getHeight(d.position));

        // Texture coordinates
        a.uv = calcUV(a.position);
        b.uv = calcUV(b.position);
        c.uv = calcUV(c.position);
        d.uv = calcUV(d.position);

        // Add to geometry
        meshData.Add(new Triangle(a, b, c));
        meshData.Add(new Triangle(c, d, a));
    }

    static Vector2 calcUV(Vector3 pos) {

        Vector3 norm = pos.normalized;
        float u = 0.5f + Mathf.Atan2(norm.z, norm.x) / 2 * Mathf.PI;
        float v = 0.5f - Mathf.Asin(norm.y) / Mathf.PI;
        return new Vector2(u, v);
    }

    public void createGeometry(Material material) {

        init();

        // Pass the data to the mesh
        meshRenderer.material = material;
        meshData.GenerateData(meshFilter.sharedMesh);
    }

    public void handleVisibility(Transform player, float horizon) {

        float dPlayerChunk = Vector3.Distance(player.position, transform.position);
        if (dPlayerChunk < horizon) {
            meshRenderer.enabled = true;
        } else {
            meshRenderer.enabled = false;
        }
    }
}