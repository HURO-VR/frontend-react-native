import React, { useEffect, useState } from 'react';
import { View, StyleSheet, Text, Dimensions, TouchableOpacity } from 'react-native';
import Svg, { Circle, Line, Polygon, Path, G, Text as SvgText } from 'react-native-svg';
import { MaterialIcons } from '@expo/vector-icons';
import { 
  XYZ, 
  RobotData, 
  ObstacleData, 
  SimulationRunData, 
  SimulationRun 
} from '@/firebase/models';
import { useSimulationAnimation } from './useSimulationAnimationHook';

interface SceneVisualizationProps {
  simulationRun: SimulationRun;
  width?: number;
  height?: number;
}

const SceneVisualization: React.FC<SceneVisualizationProps> = ({ 
  simulationRun, 
  width = 400,
  height = 400
}) => {
  const [bounds, setBounds] = useState({ minX: 0, maxX: 0, minZ: 0, maxZ: 0 });
  const [scale, setScale] = useState(1);
  const [offset, setOffset] = useState({ x: 0, z: 0 });
  const [robotColors, setRobotColors] = useState<Record<string, string>>({});
  
  // Use animation hook
  const { 
    isPlaying,
    playbackProgress,
    animationSpeed,
    robotPositions,
    elapsedTime,
    playAnimation,
    pauseAnimation,
    stopAnimation,
    resetAnimation,
    changeSpeed
  } = useSimulationAnimation(simulationRun);
  
  // Calculate bounds and scale for the visualization
  useEffect(() => {
    if (!simulationRun?.data) return;

    const { robotData, obstacleData } = simulationRun.data;
    
    let minX = Infinity, maxX = -Infinity, minZ = Infinity, maxZ = -Infinity;
    
    // Add robot path points to bounds calculation
    robotData.forEach(robot => {
      robot.robotPath.forEach(point => {
        minX = Math.min(minX, point.x);
        maxX = Math.max(maxX, point.x);
        minZ = Math.min(minZ, point.z);
        maxZ = Math.max(maxZ, point.z);
      });
      
      // Add start and end points
      [robot.robotStart, robot.robotEnd, robot.goalPosition].forEach(point => {
        minX = Math.min(minX, point.x);
        maxX = Math.max(maxX, point.x);
        minZ = Math.min(minZ, point.z);
        maxZ = Math.max(maxZ, point.z);
      });
    });
    
    // Add obstacles to bounds calculation
    obstacleData.forEach(obstacle => {
      minX = Math.min(minX, obstacle.position.x - obstacle.radius);
      maxX = Math.max(maxX, obstacle.position.x + obstacle.radius);
      minZ = Math.min(minZ, obstacle.position.z - obstacle.radius);
      maxZ = Math.max(maxZ, obstacle.position.z + obstacle.radius);
    });
    
    // Add padding
    const padding = Math.max((maxX - minX) * 0.1, (maxZ - minZ) * 0.1, 1);
    minX -= padding;
    maxX += padding;
    minZ -= padding;
    maxZ += padding;
    
    // Calculate scale and offset
    const xScale = width / (maxX - minX);
    const zScale = height / (maxZ - minZ);
    const newScale = Math.min(xScale, zScale);
    
    setBounds({ minX, maxX, minZ, maxZ });
    setScale(newScale);
    setOffset({ x: minX, z: minZ });
    
    // Generate colors for each robot
    const colors: Record<string, string> = {};
    const baseColors = ['#FF5733', '#33FF57', '#3357FF', '#F033FF', '#FF9033', '#33FFF9'];
    
    robotData.forEach((robot, index) => {
      colors[robot.name] = baseColors[index % baseColors.length];
    });
    
    setRobotColors(colors);
  }, [simulationRun, width, height]);
  
  // Convert world coordinates to screen coordinates
  const worldToScreen = (point: XYZ) => {
    return {
      x: (point.x - offset.x) * scale,
      z: height - (point.z - offset.z) * scale, // Flip Z axis for screen coordinates
    };
  };
  
  // Render robot paths, obstacles, etc.
  const renderSimulation = () => {
    if (!simulationRun?.data) return null;
    
    const { robotData, obstacleData } = simulationRun.data;
    
    return (
      <Svg width={width} height={height} style={styles.map}>
        {/* Render obstacles */}
        {obstacleData.map((obstacle, index) => {
          const position = worldToScreen(obstacle.position);
          const radius = obstacle.radius * scale;
          
          return (
            <Circle
              key={`obstacle-${index}`}
              cx={position.x}
              cy={position.z}
              r={radius}
              fill="rgba(150, 150, 150, 0.5)"
              stroke="#666"
              strokeWidth={1}
            />
          );
        })}
        
        {/* Render robot paths */}
        {robotData.map((robot, robotIndex) => {
          const color = robotColors[robot.name] || '#FF5733';
          const pathPoints = robot.robotPath.map(point => {
            const screenPoint = worldToScreen(point);
            return `${screenPoint.x},${screenPoint.z}`;
          }).join(' ');
          
          const startPoint = worldToScreen(robot.robotStart);
          const endPoint = worldToScreen(robot.robotEnd);
          const goalPoint = worldToScreen(robot.goalPosition);
          
          // Get current position for animation
          const currentPosition = robotPositions[robot.name] 
            ? worldToScreen(robotPositions[robot.name])
            : startPoint;
          
          return (
            <React.Fragment key={`robot-${robotIndex}`}>
              {/* Robot path */}
              <Path
                d={`M ${pathPoints}`}
                fill="none"
                stroke={color}
                strokeWidth={2}
                strokeOpacity={0.5}
              />
              
              {/* Robot start position */}
              <Circle
                cx={startPoint.x}
                cy={startPoint.z}
                r={4}
                fill="white"
                stroke={color}
                strokeWidth={2}
              />
              
              {/* Robot end position */}
              <Circle
                cx={endPoint.x}
                cy={endPoint.z}
                r={4}
                fill={color}
                stroke="white"
                strokeWidth={1}
              />
              
              {/* Goal position */}
              <Polygon
                points={`${goalPoint.x},${goalPoint.z - 8} ${goalPoint.x - 7},${goalPoint.z + 4} ${goalPoint.x + 7},${goalPoint.z + 4}`}
                fill={robot.goalReached ? "green" : "red"}
                stroke="black"
                strokeWidth={1}
              />
              
              {/* Current robot position (animated) */}
              <Circle
                cx={currentPosition.x}
                cy={currentPosition.z}
                r={6}
                fill={color}
                stroke="white"
                strokeWidth={2}
              />
              
              {/* Robot name */}
              <G>
                <Circle
                  cx={currentPosition.x}
                  cy={currentPosition.z - 15}
                  r={8}
                  fill="white"
                  stroke={color}
                  strokeWidth={1}
                />
                <SvgText
                  x={currentPosition.x}
                  y={currentPosition.z - 12}
                  fontSize={10}
                  textAnchor="middle"
                  fill={color}
                >
                  {robotIndex + 1}
                </SvgText>
              </G>
              
              {/* Collision points */}
              {robot.collisions.map((collision, collisionIndex) => {
                const collisionPoint = worldToScreen(collision);
                return (
                  <Circle
                    key={`collision-${robotIndex}-${collisionIndex}`}
                    cx={collisionPoint.x}
                    cy={collisionPoint.z}
                    r={4}
                    fill="red"
                    stroke="black"
                    strokeWidth={1}
                  />
                );
              })}
            </React.Fragment>
          );
        })}
      </Svg>
    );
  };
  
  // Render progress bar
  const renderProgressBar = () => {
    if (!simulationRun?.data) return null;
    
    return (
      <View style={styles.progressBarContainer}>
        <View style={styles.progressBar}>
          <View style={[styles.progressFill, { width: `${playbackProgress * 100}%` }]} />
        </View>
        <Text style={styles.timeText}>
          {(elapsedTime / 1000).toFixed(2)}s / {(simulationRun.data.timeToComplete / 1000).toFixed(2)}s
        </Text>
      </View>
    );
  };
  
  // Render playback controls
  const renderPlaybackControls = () => {
    return (
      <View style={styles.controlsContainer}>
        <TouchableOpacity style={styles.controlButton} onPress={stopAnimation}>
          <MaterialIcons name="stop" size={24} color="#333" />
        </TouchableOpacity>
        
        {isPlaying ? (
          <TouchableOpacity style={styles.controlButton} onPress={pauseAnimation}>
            <MaterialIcons name="pause" size={24} color="#333" />
          </TouchableOpacity>
        ) : (
          <TouchableOpacity style={styles.controlButton} onPress={playAnimation}>
            <MaterialIcons name="play-arrow" size={24} color="#333" />
          </TouchableOpacity>
        )}
        
        <TouchableOpacity style={styles.controlButton} onPress={resetAnimation}>
          <MaterialIcons name="replay" size={24} color="#333" />
        </TouchableOpacity>
        
        <View style={styles.speedControls}>
          <Text style={styles.speedLabel}>Speed:</Text>
          {[0.5, 1, 2, 5].map((speed) => (
            <TouchableOpacity 
              key={`speed-${speed}`} 
              style={[
                styles.speedButton, 
                animationSpeed === speed && styles.speedButtonActive
              ]}
              onPress={() => changeSpeed(speed)}
            >
              <Text style={[
                styles.speedButtonText,
                animationSpeed === speed && styles.speedButtonTextActive
              ]}>
                {speed}x
              </Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>
    );
  };

  // Render legend
  const renderLegend = () => {
    if (!simulationRun?.data) return null;
    
    const { robotData } = simulationRun.data;
    
    return (
      <View style={styles.legendContainer}>
        {robotData.map((robot, index) => (
          <View key={`legend-${index}`} style={styles.legendItem}>
            <View style={[styles.legendColor, { backgroundColor: robotColors[robot.name] || '#FF5733' }]} />
            <Text style={styles.legendText}>
              {robot.name} {robot.goalReached ? '✓' : '✗'} 
              ({robot.collisions.length} collisions)
            </Text>
          </View>
        ))}
        <View style={styles.legendItem}>
          <View style={[styles.legendColor, { backgroundColor: 'rgba(150, 150, 150, 0.5)' }]} />
          <Text style={styles.legendText}>Obstacles</Text>
        </View>
        <View style={styles.legendItem}>
          <View style={[styles.legendColor, { backgroundColor: 'red' }]} />
          <Text style={styles.legendText}>Collisions</Text>
        </View>
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Simulation: {simulationRun?.name}</Text>
      {simulationRun?.data && (
        <Text style={styles.subtitle}>
          {simulationRun.data.deadlock ? 'Status: Deadlock' : 'Status: Complete'} | 
          Time: {(simulationRun.data.timeToComplete / 1000).toFixed(2)}s
        </Text>
      )}
      {renderSimulation()}
      {renderProgressBar()}
      {renderPlaybackControls()}
      {renderLegend()}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 10,
    backgroundColor: '#f5f5f5',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 5,
  },
  subtitle: {
    fontSize: 14,
    color: '#666',
    marginBottom: 10,
  },
  map: {
    backgroundColor: '#fff',
    borderRadius: 4,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  legendContainer: {
    marginTop: 10,
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
  legendItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginRight: 15,
    marginBottom: 5,
  },
  legendColor: {
    width: 12,
    height: 12,
    borderRadius: 6,
    marginRight: 5,
    borderWidth: 1,
    borderColor: '#666',
  },
  legendText: {
    fontSize: 12,
  },
  progressBarContainer: {
    marginTop: 10,
    marginBottom: 5,
  },
  progressBar: {
    height: 10,
    backgroundColor: '#ddd',
    borderRadius: 5,
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    backgroundColor: '#3357FF',
  },
  timeText: {
    fontSize: 12,
    color: '#666',
    marginTop: 2,
    textAlign: 'right',
  },
  controlsContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 10,
    marginBottom: 10,
  },
  controlButton: {
    width: 40,
    height: 40,
    backgroundColor: '#fff',
    borderRadius: 20,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 10,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  speedControls: {
    flexDirection: 'row',
    alignItems: 'center',
    marginLeft: 10,
  },
  speedLabel: {
    fontSize: 12,
    marginRight: 5,
  },
    speedButton: {
        paddingHorizontal: 8,
        paddingVertical: 4,
        borderRadius: 4,
        backgroundColor: '#eee',
        marginRight: 5,
        },
        speedButtonActive: {
        backgroundColor: '#3357FF',
        },
        speedButtonText: {
        fontSize: 12,
        color: '#333',
        },
        speedButtonTextActive: {
        color: '#fff',
        },
});

export default SceneVisualization;