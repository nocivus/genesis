using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class TerrainGen2 : MonoBehaviour {

    public Algorithm2 algorithm;
    public Chunk2 upFace;
    public Transform player;
    public Material material;

    public void Start() {

        generateTerrain();
    }

    public void generateTerrain() {

        upFace.reset();

        float radius = algorithm.getBaseRadius();

        // Setup the size of the chunks
        upFace.transform.position = new Vector3(transform.position.x - radius/2f, 0, transform.position.z - radius/2f);
        upFace.startPoint = upFace.transform.position;
        upFace.endPoint = upFace.transform.position + new Vector3(radius, 0, radius);

        // Create geometry
        upFace.createGeometry(material, algorithm);
    }

    void Update() {

        upFace.update(player.position, algorithm);
    }

    void OnDrawGizmos() {

        foreach (Vector3[] lines in upFace.getDebugLines()) {
            Gizmos.DrawLine(lines[0], lines[1]);
        }

        // Chunk c = closestChunk(player.position);
        // if (c != null) {
        //     Gizmos.DrawCube(transform.TransformPoint(c.transform.position), new Vector3(5f, 5f, 5f));
        // }
    }
}

[CustomEditor(typeof(TerrainGen2))]
public class TerrainGen2Inspector : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        TerrainGen2 myScript = (TerrainGen2)target;
        if (GUILayout.Button("Create")) {
            myScript.generateTerrain();
        }
    }
}