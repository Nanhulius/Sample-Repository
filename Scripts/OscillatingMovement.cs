using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMovement
{
    private Vector3 initialPosition;
    private float moveSpeed, floatAmplitude, floatFrequency;

    public OscillatingMovement(Vector3 startPosition, float speed, float amplitude, float frequency)
    {
        UpdatePosition(startPosition);
        moveSpeed = speed;
        floatAmplitude = amplitude;
        floatFrequency = frequency;
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        initialPosition = newPosition;
    }

    public Vector3 GetNewPosition(float deltaTime, Vector3 currentPosition)
    {
        float floatYAxis = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 newPosition = new Vector3(currentPosition.x + moveSpeed * deltaTime, floatYAxis, initialPosition.z);
        return newPosition;
    }

}
