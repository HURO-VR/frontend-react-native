import { useState, useEffect, useRef } from 'react';
import { XYZ, SimulationRun } from '@/firebase/models';

export const useSimulationAnimation = (simulationRun: SimulationRun) => {
  // Animation state
  const [isPlaying, setIsPlaying] = useState(false);
  const [playbackProgress, setPlaybackProgress] = useState(0);
  const animationRef = useRef<NodeJS.Timeout | null>(null);
  const [animationSpeed, setAnimationSpeed] = useState(1);
  const [robotPositions, setRobotPositions] = useState<Record<string, XYZ>>({});
  const [elapsedTime, setElapsedTime] = useState(0);

  // Initialize robot positions when simulation data changes
  useEffect(() => {
    if (!simulationRun?.data) return;
    
    // Reset animation state
    resetAnimation();
    
    // Initialize robot positions
    const initialPositions: Record<string, XYZ> = {};
    simulationRun.data.robotData.forEach(robot => {
      if (robot.robotStart) initialPositions[robot.name] = { ...robot.robotStart };
    });
    setRobotPositions(initialPositions);
  }, [simulationRun]);

  // Animation effect
  useEffect(() => {
    if (!isPlaying || !simulationRun?.data) return;

    const maxTime = Math.max(...simulationRun.data.robotData.map(robot => robot.timeToComplete));
    const interval = 16; // ~60fps for smoother animation
    
    animationRef.current = setInterval(() => {
      setElapsedTime(prev => {
        const newTime = prev + interval * animationSpeed;
        
        // If we've reached the end of the animation
        if (newTime >= maxTime) {
          setIsPlaying(false);
          if (animationRef.current) clearInterval(animationRef.current);
          return maxTime;
        }
        
        return newTime;
      });
      
      // Update progress based on elapsed time
      setPlaybackProgress(prev => {
        const newProgress = Math.min(elapsedTime / maxTime, 1);
        return newProgress;
      });
      
      // Update robot positions
      setRobotPositions(prevPositions => {
        const newPositions: Record<string, XYZ> = {};
        simulationRun.data.robotData.forEach(robot => {
          const pathLength = robot.robotPath.length;
          if (pathLength <= 1) {
            newPositions[robot.name] = { ...robot.robotStart };
            return;
          }
          
          const currentTime = elapsedTime;
          const normalizedTime = Math.min(currentTime / robot.timeToComplete, 1);
          
          // Find the segment of the path we're on
          const segmentIndex = Math.min(
            Math.floor(normalizedTime * (pathLength - 1)),
            pathLength - 2
          );
          
          // Calculate how far along this segment we are (0-1)
          const segmentProgress = (normalizedTime * (pathLength - 1)) - segmentIndex;
          
          const point1 = robot.robotPath[segmentIndex];
          const point2 = robot.robotPath[segmentIndex + 1];
          
          if (point2) {
                // Linear interpolation between points
                newPositions[robot.name] = {
                    x: point1.x + (point2.x - point1.x) * segmentProgress,
                    y: point1.y + (point2.y - point1.y) * segmentProgress,
                    z: point1.z + (point2.z - point1.z) * segmentProgress
                };
            } else {
                newPositions[robot.name] = { ...robot.robotEnd };
            }
        });
        
        return newPositions;
      });
    }, interval);
    
    return () => {
      if (animationRef.current) clearInterval(animationRef.current);
    };
  }, [isPlaying, animationSpeed, elapsedTime, simulationRun]);
  
  // Animation controls
  const playAnimation = () => {
    if (playbackProgress >= 1) {
      resetAnimation();
    }
    setIsPlaying(true);
  };
  
  const pauseAnimation = () => {
    setIsPlaying(false);
  };
  
  const stopAnimation = () => {
    setIsPlaying(false);
    resetAnimation();
  };
  
  const resetAnimation = () => {
    setPlaybackProgress(0);
    setElapsedTime(0);
    
    // Reset robot positions to starting positions
    if (simulationRun?.data) {
      const initialPositions: Record<string, XYZ> = {};
      simulationRun.data.robotData.forEach(robot => {
        initialPositions[robot.name] = { ...robot.robotStart };
      });
      setRobotPositions(initialPositions);
    }
  };
  
  const changeSpeed = (newSpeed: number) => {
    setAnimationSpeed(newSpeed);
  };

  return {
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
  };
};