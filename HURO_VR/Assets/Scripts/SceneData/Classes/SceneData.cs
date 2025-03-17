using Newtonsoft.Json;
using UnityEngine;

// Part of SceneDataManager that defines how the algorithm receives input.
public partial class SceneDataManager : MonoBehaviour
{
    /// <summary>
    /// A stripped-down version of SceneData for algorithm output.
    /// </summary>
    public struct SceneDataOutput
    {
        public float robot_radius;
        public Circle[] obstacles;
        public Boundary boundary;
        public Robot[] robots;
    }

    public float robot_radius;
    public Obstacle[] obstacles;
    public Boundary boundary;
    public Robot[] robots;
    private SceneDataOutput output;

    /// <summary>
    /// Loads and returns the output data structure for the scene.
    /// </summary>
    /// <returns>A SceneDataOutput struct containing relevant scene information.</returns>
    public SceneDataOutput LoadOutput()
    {
        output.robot_radius = robot_radius;
        output.robots = robots;
        output.boundary = boundary;
        output.obstacles = Obstacle.UnpackAbstractions(obstacles);
        return output;
    }

    /// <summary>
    /// Serializes the scene data into a JSON string for algorithm input.
    /// </summary>
    /// <returns>A JSON string representing the scene data.</returns>
    public string GetAlgorithmInput()
    {
        return JsonConvert.SerializeObject(LoadOutput());
    }
}
