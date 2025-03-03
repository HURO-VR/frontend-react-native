import { ObstacleData, RobotData, XYZ } from '@/firebase/models';
import React, { useState, useEffect, useRef } from 'react';
import { View, Text, StyleSheet, Dimensions, ScrollView } from 'react-native';
import Svg, { Circle, Path, Line, G, Text as SvgText, Rect } from 'react-native-svg';

interface RobotPathMapProps {
  robotDataList: RobotData[];
  obstacleDataList?: ObstacleData[];
  showCollisions?: boolean;
  showLabels?: boolean;
  mapSize?: { width: number; height: number };
}

const COLORS = [
  '#FF5733', // Red-Orange
  '#33FF57', // Green
  '#3357FF', // Blue
  '#F033FF', // Purple
  '#FF33F0', // Pink
  '#33FFF0', // Cyan
  '#F0FF33', // Yellow
  '#FF8C33', // Orange
  '#33FFAA', // Teal
  '#8C33FF', // Violet
];

const RobotPathMap = ({
  robotDataList,
  obstacleDataList = [],
  showCollisions = true,
  showLabels = true,
  mapSize = { width: 300, height: 300 },
}: RobotPathMapProps) => {
  const [bounds, setBounds] = useState({
    minX: 0, maxX: 0, minY: 0, maxY: 0, minZ: 0, maxZ: 0
  });
  const [scale, setScale] = useState(1);
  const svgRef = useRef(null);

  // Calculate bounds from all robot data and obstacles
  useEffect(() => {
    if (!robotDataList || robotDataList.length === 0) return;

    let minX = Infinity, maxX = -Infinity;
    let minY = Infinity, maxY = -Infinity;
    let minZ = Infinity, maxZ = -Infinity;

    // Process robot data
    robotDataList.forEach(robot => {
      // Check start, end, goal positions
      [robot.robotStart, robot.robotEnd, robot.goalPosition].forEach(pos => {
        minX = Math.min(minX, pos.x);
        maxX = Math.max(maxX, pos.x);
        minY = Math.min(minY, pos.y);
        maxY = Math.max(maxY, pos.y);
        minZ = Math.min(minZ, pos.z);
        maxZ = Math.max(maxZ, pos.z);
      });

      // Check path
      robot.robotPath.forEach(pos => {
        minX = Math.min(minX, pos.x);
        maxX = Math.max(maxX, pos.x);
        minY = Math.min(minY, pos.y);
        maxY = Math.max(maxY, pos.y);
        minZ = Math.min(minZ, pos.z);
        maxZ = Math.max(maxZ, pos.z);
      });

      // Check collisions
      if (showCollisions && robot.collisions) {
        robot.collisions.forEach(pos => {
          minX = Math.min(minX, pos.x);
          maxX = Math.max(maxX, pos.x);
          minY = Math.min(minY, pos.y);
          maxY = Math.max(maxY, pos.y);
          minZ = Math.min(minZ, pos.z);
          maxZ = Math.max(maxZ, pos.z);
        });
      }
    });

    // Process obstacle data
    if (obstacleDataList && obstacleDataList.length > 0) {
      obstacleDataList.forEach(obstacle => {
        // Include the obstacle position plus radius for bounds calculation
        minX = Math.min(minX, obstacle.position.x - obstacle.radius);
        maxX = Math.max(maxX, obstacle.position.x + obstacle.radius);
        minY = Math.min(minY, obstacle.position.y - obstacle.radius);
        maxY = Math.max(maxY, obstacle.position.y + obstacle.radius);
        minZ = Math.min(minZ, obstacle.position.z);
        maxZ = Math.max(maxZ, obstacle.position.z);
      });
    }

    // Add some padding
    const paddingFactor = 0.1;
    const xPadding = (maxX - minX) * paddingFactor;
    const yPadding = (maxY - minY) * paddingFactor;
    
    minX -= xPadding;
    maxX += xPadding;
    minY -= yPadding;
    maxY += yPadding;

    setBounds({ minX, maxX, minY, maxY, minZ, maxZ });

    // Calculate scale to fit in view
    const xScale = mapSize.width / (maxX - minX);
    const yScale = mapSize.height / (maxY - minY);
    setScale(Math.min(xScale, yScale));
  }, [robotDataList, obstacleDataList, showCollisions, mapSize]);

  // Transform coordinates to SVG space
  const transformPoint = (point: XYZ) => {
    const x = (point.x - bounds.minX) * scale;
    const y = mapSize.height - (point.y - bounds.minY) * scale; // Invert Y-axis
    return { x, y, z: point.z };
  };

  // Generate path string for SVG
  const generatePathString = (points: XYZ[]) => {
    if (!points || points.length === 0) return '';
    const transformedPoints = points.map(transformPoint);
    return transformedPoints.reduce((path, point, i) => {
      return path + (i === 0 ? `M ${point.x},${point.y}` : ` L ${point.x},${point.y}`);
    }, '');
  };

  // Render legend
  const renderLegend = () => {
    return (
      <View style={styles.legend}>
        {/* Robot legend items */}
        {robotDataList.map((robot, index) => (
          <View key={robot.name} style={styles.legendItem}>
            <View style={[styles.legendColor, { backgroundColor: COLORS[index % COLORS.length] }]} />
            <Text style={styles.legendText}>
              {robot.name} ({robot.goalReached ? 'Goal Reached' : 'Failed'})
            </Text>
          </View>
        ))}
        
        {/* Obstacle legend item */}
        {obstacleDataList && obstacleDataList.length > 0 && (
          <View style={styles.legendItem}>
            <View style={[styles.legendColor, { backgroundColor: '#777777' }]} />
            <Text style={styles.legendText}>Obstacles</Text>
          </View>
        )}
        
        {/* Collision legend item */}
        {showCollisions && (
          <View style={styles.legendItem}>
            <View style={[styles.legendColor, { backgroundColor: 'red' }]} />
            <Text style={styles.legendText}>Collisions</Text>
          </View>
        )}
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <Svg
        ref={svgRef}
        width={mapSize.width}
        height={mapSize.height}
        style={styles.svg}
      >
        {/* Grid lines (optional) */}
        <G stroke="#EEEEEE" strokeWidth="1">
          {Array.from({ length: 10 }).map((_, i) => (
            <React.Fragment key={`grid-${i}`}>
              <Line
                x1="0"
                y1={i * (mapSize.height / 10)}
                x2={mapSize.width}
                y2={i * (mapSize.height / 10)}
              />
              <Line
                x1={i * (mapSize.width / 10)}
                y1="0"
                x2={i * (mapSize.width / 10)}
                y2={mapSize.height}
              />
            </React.Fragment>
          ))}
        </G>

        {/* Obstacles */}
        {obstacleDataList && obstacleDataList.map((obstacle, index) => {
          const transformedPosition = transformPoint(obstacle.position);
          const scaledRadius = obstacle.radius * scale;
          
          return (
            <Circle
              key={`obstacle-${index}`}
              cx={transformedPosition.x}
              cy={transformedPosition.y}
              r={scaledRadius}
              fill="#777777"
              opacity="0.5"
              stroke="#555555"
              strokeWidth="1"
            />
          );
        })}

        {/* Robot paths and positions */}
        {robotDataList.map((robot, index) => {
          const color = COLORS[index % COLORS.length];
          const transformedStart = transformPoint(robot.robotStart);
          const transformedEnd = transformPoint(robot.robotEnd);
          const transformedGoal = transformPoint(robot.goalPosition);
          
          return (
            <G key={robot.name}>
              {/* Path line */}
              <Path
                d={generatePathString(robot.robotPath)}
                stroke={color}
                strokeWidth="2"
                fill="none"
              />
              
              {/* Start position */}
              <Circle
                cx={transformedStart.x}
                cy={transformedStart.y}
                r="5"
                fill="white"
                stroke={color}
                strokeWidth="2"
              />
              
              {/* End position */}
              <Circle
                cx={transformedEnd.x}
                cy={transformedEnd.y}
                r="5"
                fill={color}
              />
              
              {/* Goal position */}
              <Circle
                cx={transformedGoal.x}
                cy={transformedGoal.y}
                r="7"
                fill="none"
                stroke={robot.goalReached ? "green" : "red"}
                strokeWidth="2"
                strokeDasharray={robot.goalReached ? "0" : "5,5"}
              />
              
              {/* Collisions */}
              {showCollisions && robot.collisions && robot.collisions.map((collision, colIndex) => {
                const transformedCollision = transformPoint(collision);
                return (
                  <Circle
                    key={`${robot.name}-collision-${colIndex}`}
                    cx={transformedCollision.x}
                    cy={transformedCollision.y}
                    r="4"
                    fill="red"
                    opacity="0.7"
                  />
                );
              })}
              
              {/* Labels */}
              {showLabels && (
                <>
                  <SvgText
                    x={transformedStart.x + 8}
                    y={transformedStart.y - 8}
                    fontSize="10"
                    fill={color}
                  >
                    Start: {robot.name}
                  </SvgText>
                  
                  <SvgText
                    x={transformedGoal.x + 8}
                    y={transformedGoal.y - 8}
                    fontSize="10"
                    fill={robot.goalReached ? "green" : "red"}
                  >
                    Goal: {robot.name}
                  </SvgText>
                </>
              )}
            </G>
          );
        })}
      </Svg>

      {renderLegend()}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 10,
  },
  svg: {
    backgroundColor: '#f8f8f8',
    borderRadius: 8,
  },
  legend: {
    marginTop: 10,
    padding: 10,
    backgroundColor: '#f0f0f0',
    borderRadius: 5,
  },
  legendItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 5,
  },
  legendColor: {
    width: 12,
    height: 12,
    borderRadius: 6,
    marginRight: 8,
  },
  legendText: {
    fontSize: 12,
  },
});

export default RobotPathMap;