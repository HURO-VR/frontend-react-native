from typing import NamedTuple


ROBOT_SCRIPT = "RobotController"
ROBOT_TAG = "Robot"
FLOOR_TAG = "Floor"
OBSTACLE_TAG = "Obstacle"


def get_obstacles():
    # Find all objects with the tag "Obstacle"
    obstacles = UnityEngine.GameObject.FindGameObjectsWithTag(OBSTACLE_TAG)

    obstacle_data = []
    for obj in obstacles:
        position = obj.transform.position
        collider = obj.GetComponent[UnityEngine.Collider]()  # Use Collider for 3D

        if collider:
            bounds = collider.bounds
            radius = max(bounds.extents.x, bounds.extents.y, bounds.extents.z)  # Largest extent as radius

            data = [position.x, position.y, radius]
            obstacle_data.append(data)
    return obstacle_data



def get_boundary():
    floor = UnityEngine.GameObject.FindGameObjectWithTag(FLOOR_TAG)

    # Extract required properties: [xPos, yPos, width/2, height/2]
    position = floor.transform.position
    scale = floor.transform.localScale  # Assuming scale represents the full width/height
    return [position.x, position.y, scale.x / 2, scale.y / 2]


def get_robot_radius():
    robot = UnityEngine.GameObject.FindGameObjectWithTag(ROBOT_TAG)
    collider = robot.GetComponent[UnityEngine.Collider]()  # Use Collider for 3D
    if collider:
        bounds = collider.bounds
        radius = max(bounds.extents.x, bounds.extents.y, bounds.extents.z)  # Largest extent as radius
        return radius
    return 0



def get_inital_robot_locations():
    # Find all objects with the tag "Obstacle"
    robots = UnityEngine.GameObject.FindGameObjectsWithTag(ROBOT_TAG)
    robot_data = []
    for obj in robots:
        position = obj.transform.position
        data = [position.x, position.y]
        robot_data.append(data)

    return robot_data

class Robot(NamedTuple):
    position: list[float]
    goal: list[float]
    maxVelocity: float
    curr_velocity: list[float]


def get_robots():
    # Find all objects with the tag "Obstacle"
    robots = UnityEngine.GameObject.FindGameObjectsWithTag(ROBOT_TAG)
    robot_data: list[Robot] = []
    for robot in robots:
        controller = robots.GetComponent(ROBOT_SCRIPT) 
        position_2d = [robot.transform.position.x, robot.transform.position.y]
        goal_position_2d = [controller.goal.transform.position.x, controller.goal.transform.position.y]
        velocity = robot.GetComponent[UnityEngine.Rigidbody]().velocity
        curr_vel = [velocity.x, velocity.y]
        robot = Robot(position_2d, goal_position_2d, controller.maxVelocity, curr_vel)
        robot_data.append(robot)
    return robot_data

class Velocity(NamedTuple):
    x: float
    y: float

def apply_velocity(robot, velocity):
    rigidbody = robot.GetComponent[UnityEngine.Rigidbody]()  # For 3D physics
    
    if rigidbody:
        # Set linear velocity (example: move right with speed 5)
        rigidbody.velocity = UnityEngine.Vector3(velocity.x, velocity.y, 0)
