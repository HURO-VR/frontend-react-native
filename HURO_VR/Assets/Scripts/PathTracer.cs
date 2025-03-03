using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PathTracer : MonoBehaviour
{
    private LineRenderer _line;
    private List<Vector3> _positions = new List<Vector3>();

    // How often to record a position (in seconds)
    public float recordInterval = 0.1f;
    private float _timer = 0f;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        // Optionally, customize the line here: width, color, etc.
        _line.startWidth = 0.3f;
        _line.endWidth = 0.3f;
        _line.startColor = Color.red;
        _line.endColor = Color.red;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= recordInterval)
        {
            _timer = 0f;
            // Record current position
            _positions.Add(transform.position);

            // Update LineRenderer vertex count
            _line.positionCount = _positions.Count;
            _line.SetPosition(_positions.Count - 1, transform.position);
        }
    }
}
