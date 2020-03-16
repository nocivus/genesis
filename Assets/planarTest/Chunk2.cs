using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk2 : MonoBehaviour {

  public Vector3 startPoint;
  public Vector3 endPoint;
  int quads = 5;

  public MeshFilter meshFilter;
  public MeshRenderer meshRenderer;
  public TriangleList meshData;
  public GameObject chunkPrefab;
  public List<Chunk2> children;

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
    if (children == null) {
      children = new List<Chunk2>();
    }
  }

  public void reset() {

    if (children != null) {
      foreach (Chunk2 c in children) {
        if (c != null) {
          c.reset();
        }
      }
    }

    meshFilter = null;
    meshRenderer = null;
    meshData = null;
    children = null;

    init();

    // Delete all existing children
    for (int i = 0; i < transform.childCount; i++) {
      DestroyImmediate(transform.GetChild(i).gameObject);
      i--;
    }
  }

  public float getDiagonal() {
    return (endPoint - startPoint).magnitude;
  }

  public Vector3 getCenter() {
    return (endPoint + startPoint) / 2f;
  }

  public float getCenterDistance(Vector3 reference) {
    return Vector3.Distance(reference, getCenter());
  }

  public void update(Vector3 player, Algorithm2 algorithm) {

    init();

    //Debug.Log(getDiagonal());
    //Debug.Log(getCenterDistance(player));

    // Split the chunk if the distance to the center is less than half of our diagonal
    if (getDiagonal() / 2f > getCenterDistance(player) && children.Count == 0) {

      split(algorithm);
      meshRenderer.enabled = false;

    } else if (getDiagonal() / 2f < getCenterDistance(player)) {

      merge();
      meshRenderer.enabled = true;
    }

    // Update children as well
    foreach (Chunk2 c in children) {
      c.update(player, algorithm);
    }
  }

  void split(Algorithm2 algorithm) {

    Vector3 center = getCenter();

    // Create 4 children 
    children.Add(createChild(startPoint, getCenter(), algorithm));
    children.Add(createChild(new Vector3(startPoint.x, 0, center.z), new Vector3(center.x, 0, endPoint.z), algorithm));
    children.Add(createChild(center, endPoint, algorithm));
    children.Add(createChild(new Vector3(center.x, 0, startPoint.z), new Vector3(endPoint.x, 0, center.z), algorithm));
  }

  Chunk2 createChild(Vector3 start, Vector3 end, Algorithm2 algorithm) {

    Chunk2 child = (Instantiate(chunkPrefab, start, Quaternion.identity) as GameObject).GetComponent<Chunk2>();
    child.transform.parent = transform;
    child.name = "Chunk " + start.x + "_" + start.z;
    child.startPoint = start;
    child.endPoint = end;
    child.createGeometry(meshRenderer.sharedMaterial, algorithm);
    return child;
  }

  void merge() {

    // Merge back
    // Delete all existing children
    for (int i = 0; i < transform.childCount; i++) {
      DestroyImmediate(transform.GetChild(i).gameObject);
      i--;
    }
    children.Clear();
  }

  public void createGeometry(Material material, Algorithm2 algorithm) {

    init();

    // Generate the quads
    float quadSize = (new Vector3(startPoint.x, 0, endPoint.z) - startPoint).magnitude / (float)quads;
    for (int i = 0; i < quads; i++) {
      for (int j = 0; j < quads; j++) {
        Chunk2.createQuad((float)i * quadSize, (float)j * quadSize, quadSize, algorithm, meshData, transform);
      }
    }

    // Pass the data to the mesh
    meshRenderer.sharedMaterial = material;
    meshData.GenerateData(meshFilter.sharedMesh);

    // Recalculate bounds and normals
    meshFilter.sharedMesh.RecalculateBounds();
    NormalSolver.RecalculateNormals(meshFilter.sharedMesh, 60);
  }

  public static void createQuad(float i, float j, float quadSize, Algorithm2 algorithm, TriangleList meshData, Transform transform) {

    // North
    Vertex a = new Vertex(i, 0, j);
    Vertex b = new Vertex(i, 0, j + quadSize);
    Vertex c = new Vertex(i + quadSize, 0, j + quadSize);
    Vertex d = new Vertex(i + quadSize, 0, j);

    // Apply terrain features based on the algorithm
    /*a.position = transform.TransformPoint(a.position);
    b.position = transform.TransformPoint(b.position);
    c.position = transform.TransformPoint(c.position);
    d.position = transform.TransformPoint(d.position);
    a.position = transform.InverseTransformPoint(a.position.normalized * algorithm.getHeight(a.position));
    b.position = transform.InverseTransformPoint(b.position.normalized * algorithm.getHeight(b.position));
    c.position = transform.InverseTransformPoint(c.position.normalized * algorithm.getHeight(c.position));
    d.position = transform.InverseTransformPoint(d.position.normalized * algorithm.getHeight(d.position));*/

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

    /*Vector3 norm = pos.normalized;
    float u = 0.5f + Mathf.Atan2(norm.z, norm.x) / 2 * Mathf.PI;
    float v = 0.5f - Mathf.Asin(norm.y) / Mathf.PI;
    return new Vector2(u, v);*/
    return new Vector2(pos.x, pos.z);
  }

  public List<Vector3[]> getDebugLines() {

    List<Vector3[]> lines = new List<Vector3[]>();

    lines.AddRange(squareLines(startPoint, endPoint));

    if (children != null) {
      foreach (Chunk2 c in children) {
        lines.AddRange(c.getDebugLines());
      }
    }

    return lines;
  }

  public List<Vector3[]> squareLines(Vector3 start, Vector3 end) {

    List<Vector3[]> lines = new List<Vector3[]>();
    lines.Add(new Vector3[] { startPoint, new Vector3(startPoint.x, 0, endPoint.z) });
    lines.Add(new Vector3[] { startPoint, new Vector3(endPoint.x, 0, startPoint.z) });
    lines.Add(new Vector3[] { endPoint, new Vector3(startPoint.x, 0, endPoint.z) });
    lines.Add(new Vector3[] { endPoint, new Vector3(endPoint.x, 0, startPoint.z) });
    return lines;
  }
}