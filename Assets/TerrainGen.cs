using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class TerrainGen : MonoBehaviour {

    public int numberOfChunks = 5;
    public int quadsPerChunk = 5;
    public Algorithm algorithm;

    public MeshRenderer basicShapeRenderer;
    public FaceTerrain northFace;
    public FaceTerrain southFace;
    public FaceTerrain westFace;
    public FaceTerrain eastFace;
    public FaceTerrain upFace;
    public FaceTerrain downFace;
    public Material material;

    public Transform player;
    public GameObject lod0;
    public GameObject lod1;

    public void Start() {

        generateTerrain();
    }

    public void generateTerrain() {

        float radius = algorithm.getBaseRadius();

        // Update the basic shape
        basicShapeRenderer.transform.localScale = Vector3.one * radius * 1.8f;

        // Move the faces into place
        northFace.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        northFace.transform.position = new Vector3(-radius / 2f, radius / 2f, radius / 2f);

        southFace.transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);
        southFace.transform.position = new Vector3(-radius / 2f, -radius / 2f, -radius / 2f);

        westFace.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        westFace.transform.position = new Vector3(-radius / 2f, -radius / 2f, -radius / 2f);

        eastFace.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
        eastFace.transform.position = new Vector3(radius / 2f, radius / 2f, -radius / 2f);

        //upFace.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
        upFace.transform.position = new Vector3(-radius / 2f, radius / 2f, -radius / 2f);

        downFace.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        downFace.transform.position = new Vector3(radius / 2f, -radius / 2f, -radius / 2f);

        // Generate chunked world    
        northFace.generate(numberOfChunks, quadsPerChunk, algorithm);
        southFace.generate(numberOfChunks, quadsPerChunk, algorithm);
        westFace.generate(numberOfChunks, quadsPerChunk, algorithm);
        eastFace.generate(numberOfChunks, quadsPerChunk,  algorithm);
        upFace.generate(numberOfChunks, quadsPerChunk, algorithm);
        downFace.generate(numberOfChunks, quadsPerChunk, algorithm);

        // Create geometry
        basicShapeRenderer.material = material;
        northFace.createGeometry(material);
        southFace.createGeometry(material);
        westFace.createGeometry(material);
        eastFace.createGeometry(material);
        upFace.createGeometry(material);
        downFace.createGeometry(material);
    }

    void Update() {

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float horizon = Mathf.Sqrt(distanceToPlayer * distanceToPlayer + algorithm.getBaseRadius() * algorithm.getBaseRadius());

        //Debug.Log(distanceToPlayer);
        if (distanceToPlayer > 2000) {
            lod0.SetActive(true);
            lod1.SetActive(false);
        } else {
            // Handle within chunks
            lod0.SetActive(false);
            lod1.SetActive(true);
            northFace.handleVisibility(player, distanceToPlayer, horizon);
            southFace.handleVisibility(player, distanceToPlayer, horizon);
            westFace.handleVisibility(player, distanceToPlayer, horizon);
            eastFace.handleVisibility(player, distanceToPlayer, horizon);
            upFace.handleVisibility(player, distanceToPlayer, horizon);
            downFace.handleVisibility(player, distanceToPlayer, horizon);
            Chunk c = closestChunk(player.position);
            if (c != null) {
                c.gameObject.SetActive(true);
                c.handleVisibility(player, horizon);
            }
        }
    }

    Chunk closestChunk(Vector3 pos) {

        // Which face is closest?
        Dictionary<FaceTerrain, float> dictionary = new Dictionary<FaceTerrain, float>();
        dictionary.Add(northFace, Vector3.Distance(pos, northFace.transform.position));
        dictionary.Add(southFace, Vector3.Distance(pos, southFace.transform.position));
        dictionary.Add(westFace, Vector3.Distance(pos, westFace.transform.position));
        dictionary.Add(eastFace, Vector3.Distance(pos, eastFace.transform.position));
        dictionary.Add(upFace, Vector3.Distance(pos, upFace.transform.position));
        dictionary.Add(downFace, Vector3.Distance(pos, downFace.transform.position));
        List<KeyValuePair<FaceTerrain, float>> list = dictionary.ToList();
        list.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

        return list[0].Key.closestChunk(pos);
    }

    void enableDisableFace(FaceTerrain t, bool state) {
        t.gameObject.SetActive(state);
    }

    void OnDrawGizmos() {

        foreach (Vector3[] lines in northFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }
        foreach (Vector3[] lines in southFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }
        foreach (Vector3[] lines in westFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }
        foreach (Vector3[] lines in eastFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }
        foreach (Vector3[] lines in upFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }
        foreach (Vector3[] lines in downFace.getDebugLines(algorithm.getBaseRadius())) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }

        Chunk c = closestChunk(player.position);
        if (c != null) {
            Gizmos.DrawCube(transform.TransformPoint(c.transform.position), new Vector3(5f, 5f, 5f));
        }
    }
}

[CustomEditor(typeof(TerrainGen))]
public class TerrainGenInspector : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        TerrainGen myScript = (TerrainGen)target;
        if (GUILayout.Button("Create")) {
            myScript.generateTerrain();
        }
    }
}