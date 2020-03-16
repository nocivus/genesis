using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex {

    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;

    public Vertex(float x, float y, float z, float u, float v) {

        this.position = new Vector3(x, y, z);
        this.uv = new Vector2(u, v);
    }

    public Vertex(float x, float y, float z) : this(x, y, z, 0 ,0) { }

    public Vertex(Vector3 position, Vector2 uv) : this(position.x, position.y, position.z, uv.x, uv.y) { }

    public Vertex(Vector3 position) : this(position, new Vector2(0,0)) { }

    public override string ToString() {

        return position.x + "," + position.y + "," + position.z + "  uv:" + uv;
    }

    public override bool Equals(object o) {

        if (o == null || !o.GetType().Equals(typeof(Vertex))) {
            return false;
        }
        Vertex other = (Vertex)o;
        return other.position.x == position.x && other.position.y == position.y && other.position.z == position.z;
    }

    public override int GetHashCode() {

        return base.GetHashCode();
    }
}
