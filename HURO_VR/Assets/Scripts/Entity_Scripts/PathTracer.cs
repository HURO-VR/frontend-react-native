using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Records the positions of a GameObject over time and visualizes its path using a LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class PathTracer : MonoBehaviour
{
    #region Private Variables

    /// <summary>
    /// Reference to the LineRenderer component used for drawing the path.
    /// </summary>
    private LineRenderer _line;

    /// <summary>
    /// A list to store recorded positions.
    /// </summary>
    private List<Vector3> _positions = new List<Vector3>();

    /// <summary>
    /// Timer to accumulate time between recordings.
    /// </summary>
    private float _timer = 0f;

    #endregion

    #region Public Variables

    /// <summary>
    /// Interval in seconds between each recorded position.
    /// </summary>
    public float recordInterval = 0.1f;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the LineRenderer properties.
    /// </summary>
    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.startWidth = 0.3f;
        _line.endWidth = 0.3f;
        _line.startColor = Color.red;
        _line.endColor = Color.red;
    }

    /// <summary>
    /// Called once per frame.
    /// Records the GameObject's position at defined intervals and updates the LineRenderer.
    /// </summary>
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= recordInterval)
        {
            _timer = 0f;
            // Record current position.
            _positions.Add(transform.position);

            // Update LineRenderer vertex count.
            _line.positionCount = _positions.Count;
            _line.SetPosition(_positions.Count - 1, transform.position);
        }
    }

    #endregion
}
