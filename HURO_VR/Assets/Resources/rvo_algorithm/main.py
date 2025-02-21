import sys
from RVO import RVO_update, compute_V_des
from huro_vr import get_obstacles, get_boundary, get_robot_radius, Velocity, apply_velocity, get_robots


#------------------------------
#define workspace model
ws_model = dict()

#robot radius
ws_model['robot_radius'] = get_robot_radius()

#circular obstacles, format [x,y,rad]
ws_model['circular_obstacles'] = get_obstacles()
# with obstacles
# ws_model['circular_obstacles'] = [[-0.3, 2.5, 0.3], [1.5, 2.5, 0.3], [3.3, 2.5, 0.3], [5.1, 2.5, 0.3]]

#rectangular boundary, format [x,y,width/2,heigth/2]
ws_model['boundary'] = get_boundary()

#------------------------------
#initialization for robot 
# position of [x,y]
robots = get_robots()

#------------------------------
# compute desired vel to goal
robot_positions = [robot.position for robot in robots]
goal_positions = [robot.goal for robot in robots]
maxVelocities = [robot.maxVelocity for robot in robots]
currVelocity = [robot.curr_velocity for robot in robots]

V_des = compute_V_des(robot_positions, goal_positions, maxVelocities)
# compute the optimal vel to avoid collision
new_velocities = RVO_update(robot_positions, V_des, currVelocity, ws_model)

# update velocity
for i in range(len(robots)):
    velocity = Velocity(new_velocities[i][0], new_velocities[i][1])
    apply_velocity(robots[i], velocity)    
