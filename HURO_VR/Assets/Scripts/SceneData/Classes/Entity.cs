using UnityEngine;

public interface Entity
{
    /// <summary>
    /// Draws a gizmo representation of the entity in the scene view.
    /// </summary>
    public void DrawGizmo();

    #region Serialized Variables
    // Add any [SerializeField] variables here if needed
    // Add Headers: [Header("Logging")]
    // Add Tips: [Tooltip("Runs the algorithm every X seconds.")]
    #endregion

    #region Public Variables
    // Add public variables here
    #endregion

    #region Private Variables
    // Add private variables here
    #endregion

    #region Public Methods
    // Add public methods here
    #endregion

    #region Private Methods
    // Add private methods here
    #endregion
}
