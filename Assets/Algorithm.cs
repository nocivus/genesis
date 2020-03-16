using UnityEngine;
using System.Collections;
using SimplexNoise;

public class Algorithm : MonoBehaviour {

    public float radius = 100f;
    public float mountainFrequency = 0.001f;
    public float depressionFrequency = 0.001f;
    public float maxAltitude = 110f;
    public float minAltitude = 90f;

    public float getBaseRadius() {

        return radius;
    }

    public float getHeight(Vector3 position) {

        float height = radius;

        // Mountains
        height += GetNoise(position.x, position.y, position.z, mountainFrequency, maxAltitude);
        if (height > maxAltitude) {
            height = maxAltitude;
        }

        // Depressions
        height -= GetNoise(position.x, position.y, position.z, depressionFrequency, radius);
        if (height < minAltitude) {
            height = minAltitude;
        }

        return height;
    }

    public float GetNoise(float x, float y, float z, float scale, float max) {

        return (Noise.Generate(x * scale, y * scale + 1f, z * scale) + 1f) * (max / 2f);
    }
}