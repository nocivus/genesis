using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriangleList {

    public Triangle head;
    public Triangle tail;
    public int Count = 0;

    public List<Vertex> vertices {
        get {

            List<Vertex> verts = new List<Vertex>();
            for (Triangle t=head; t!=null; t=t.next) {
                verts.Add(t.a);
                verts.Add(t.b);
                verts.Add(t.c);
            }
            return verts;
        }
    }

    public TriangleList() {
    }

    public TriangleList(Vertex[] vertices) : this() {

        for (int i = 0; i < vertices.Length - 2; i += 3) {

            Add(new Triangle(vertices[i], vertices[i + 1], vertices[i + 2]));
        }
    }

    public TriangleList(TriangleList data) : this() {

        for (Triangle t=head; t!=null; t=t.next) {

            Add(t);
        }
    }

    public void Add(Vertex v1, Vertex v2, Vertex v3) {

        Add(new Triangle(v1, v2, v3));
    }

    public void Add(Vector3 v1, Vector3 v2, Vector3 v3) {

        Add(new Triangle(new Vertex(v1), new Vertex(v2), new Vertex(v3)));
    }

    public void Add(TriangleList triangles) {

        for (Triangle t=triangles.head; t!=null; t=t.next) {

            Add(t);
        }
    }

    public void Add(Triangle[] triangles) {

        for (int i=0; i<triangles.Length; i++) {

            Add(triangles[i]);
        }
    }

    public void Remove(Triangle tri) {

        for (Triangle t=head; t!=null; t=t.next) {

            if (t == tri) {

                if (t.previous != null) {
                    t.previous.next = t.next;
                }
                if (t.next != null) {
                    t.next.previous = t.previous;
                }
                break;
            }
        }
    }

    public void Clear() {

        head = null;
    }

    public void InsertAfter(Triangle refOne, Triangle newOne) {

        newOne.next = refOne.next;
        newOne.previous = refOne;
        if (refOne.next != null) {
            refOne.next.previous = newOne;
        }
        refOne.next = newOne;        
        Count++;

        if (refOne == tail) {
            tail = newOne;
        }
    }

    public void Add(Triangle t) {

        if (tail != null) {
            tail.next = t;
        }
        t.previous = tail;
        tail = t;
        Count++;

        if (Count == 1) {
            head = t;
        }
    }

    public void GenerateData(Mesh mesh) {

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        GenerateData(vertices, triangles, normals, uvs);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
    }

    public void GenerateData(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs) {

        // Vertices and UV
        for (Triangle t=head; t!=null; t=t.next) {

            if (!vertices.Contains(t.a.position)) {
                vertices.Add(t.a.position);
                normals.Add(t.a.normal);
                uvs.Add(t.a.uv);
            }

            if (!vertices.Contains(t.b.position)) {
                vertices.Add(t.b.position);
                normals.Add(t.b.normal);
                uvs.Add(t.b.uv);
            }

            if (!vertices.Contains(t.c.position)) {
                vertices.Add(t.c.position);
                normals.Add(t.c.normal);
                uvs.Add(t.c.uv);
            }
        }

        // Faces
        for (Triangle t=head; t!=null; t=t.next) {

            triangles.Add(vertices.IndexOf(t.a.position));
            triangles.Add(vertices.IndexOf(t.b.position));
            triangles.Add(vertices.IndexOf(t.c.position));
        }
    }

    public Triangle findTriangleAt(Vector3 pos) {

        for (Triangle t=head; t!=null; t=t.next) {

            if (t.containsPoint(pos)) {

                //Debug.Log("Hit: " + t.id);
                return t;
            }
        }
        return null;
    }

    public override string ToString() {

        string result = "";
        int i = 0;
        for (Triangle tri=head; tri!=null; tri=tri.next) {

            result += i + ": " + tri.ToString() + "\r\n";
            i++;
        }
        return result;
    }

    public string ShowLinkedList() {

        string result = "";
        for (Triangle t=head; t!=null; t=t.next) {

            result += Triangle.IdOf(t) + " -> ";
        }
        result += "\r\n";
        for (Triangle t=head; t!=null; t=t.next) {

            result += t + "\r\n";
        }
        /*for (Triangle t=tail; t!=null; t=t.previous) {

            result += " <- " + IdOf(t);
        }*/
        /*for (Triangle t=head; t!=null; t=t.next) {

            result += IdOf(t) + " (p: " + IdOf(t.previous) + ", n: " + IdOf(t.next) + ")\r\n";
        }*/
        result += "Head: " + Triangle.IdOf(head) + " Tail: " + Triangle.IdOf(tail) + "\r\n";
        return result;
    }
}

