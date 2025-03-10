from __future__ import division

from pyorca import Agent, get_avoidance_velocity, orca, normalized, perp
from numpy import array, rint, linspace, pi, cos, sin
import random


def step(scene_data):
    agents = []
    N_AGENTS = len(scene_data.robots)
    for i in range(N_AGENTS):
        robot = scene_data.robots[i]
        theta = 2 * pi * i / N_AGENTS
        x = robot.radius * array((cos(theta), sin(theta))) #+ random.uniform(-1, 1)
        vel = normalized(-x) * scene_data.max_velocity
        pos = (robot.position.x, robot.position.z)
        agents.append(Agent(pos, (0., 0.), 1., robot.max_velocity, vel))

    tau = 5
    new_vels = [None] * len(agents)
    for i, agent in enumerate(agents):
        candidates = agents[:i] + agents[i + 1:]
        new_vels[i] = orca(agent, candidates, tau, scene_data.delta_time)

    return new_vels
