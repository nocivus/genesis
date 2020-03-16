using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Triangle
{
    public static List<Vertex> vertexCache = new List<Vertex>();
    private static int counter = 0;

    public int id;
	public Vertex a;
	public Vertex b;
	public Vertex c;

    // Linked list stuff
    public Triangle previous;
    public Triangle next;

    public Triangle() { 

        this.id = counter++;
    }

	public Triangle(Vertex a, Vertex b, Vertex c) : this() {

		this.a = findOrCreate(new Vertex(new Vector3(a.position.x, a.position.y, a.position.z), new Vector2(a.uv.x, a.uv.y)));
		this.b = findOrCreate(new Vertex(new Vector3(b.position.x, b.position.y, b.position.z), new Vector2(b.uv.x, b.uv.y)));
		this.c = findOrCreate(new Vertex(new Vector3(c.position.x, c.position.y, c.position.z), new Vector2(c.uv.x, c.uv.y))); 
	}

    public Triangle(Vector3 a, Vector3 b, Vector3 c) : this(new Vertex(a), new Vertex(b), new Vertex(c)) {
    }

    private static Vertex findOrCreate(Vertex v) {

        if (!vertexCache.Contains(v)) {

            vertexCache.Add(v);
        }        
        return vertexCache[vertexCache.IndexOf(v)];
    }

    public static int GetVertexCount() {

        return vertexCache.Count;
    }

    public static void ReleaseVertex(Vertex v) {

        vertexCache.Remove(v);
    }

    public bool hasVertex(Vertex v) {

        return (v.position == a.position || v.position == b.position || v.position == c.position);
    }

    public Vector3 GetNormal() {

        return NormalVector(a.position, b.position, c.position);
    }

    public static Vector3 NormalVector(Vector3 p1, Vector3 p2, Vector3 p3) {

        Vector3 u = p2 - p1;
        Vector3 v = p3 - p1;

        return new Vector3(u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x);
    }

    public void generateNormals() {

        Vector3 normal = GetNormal().normalized;
        a.normal = normal;
        b.normal = normal;
        c.normal = normal;
    }

    public bool containsPoint(Vector3 p) {

        float A = 0.5f * (-b.position.y * c.position.x 
                + a.position.y * (-b.position.x + c.position.x) 
                + a.position.x * (b.position.y - c.position.y) 
                + b.position.x * c.position.y);
        int sign = A < 0 ? -1 : 1;
        float s = (a.position.y * c.position.x 
                - a.position.x * c.position.y 
                + (c.position.y - a.position.y) * p.x 
                + (a.position.x - c.position.x) * p.y) * sign;
        float t = (a.position.x * b.position.y 
                - a.position.y * b.position.x 
                + (a.position.y - b.position.y) * p.x 
                + (b.position.x - a.position.x) * p.y) * sign;
        
        return s > 0 && t > 0 && (s + t) < 2f * A * sign;
    }

	public string printUVs() {

		return a.uv + " " + b.uv + " " + c.uv;
	}

    public override string ToString() {

		return id + " [ " + a + "] [" + b + "] [" + c + "]";
	}

    public static int IdOf(Triangle t) {

        if (t == null) {
            return -1;
        }
        return t.id;
    }

    public override bool Equals(object o) {

        if (o == null || !o.GetType().Equals(typeof(Triangle))) {
            return false;
        }
        Triangle other = (Triangle) o;
        return other.id == id;
    }

    public override int GetHashCode() {

        return id;
    }
}

