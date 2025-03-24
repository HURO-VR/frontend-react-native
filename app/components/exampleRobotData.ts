const exampleRobotDataList = [
    {
      robotStart: { x: 10, y: 10, z: 0 },
      robotEnd: { x: 90, y: 90, z: 0 },
      robotPath: [
        { x: 10, y: 10, z: 0 },
        { x: 25, y: 20, z: 0 },
        { x: 40, y: 35, z: 0 },
        { x: 55, y: 50, z: 0 },
        { x: 70, y: 70, z: 0 },
        { x: 80, y: 85, z: 0 },
        { x: 90, y: 90, z: 0 }
      ],
      name: "Robot-A",
      timeToComplete: 5200,
      collisions: [
        { x: 55, y: 50, z: 0 }
      ],
      goalPosition: { x: 95, y: 95, z: 0 },
      goalReached: false
    },
    {
      robotStart: { x: 90, y: 10, z: 0 },
      robotEnd: { x: 20, y: 80, z: 0 },
      robotPath: [
        { x: 90, y: 10, z: 0 },
        { x: 80, y: 15, z: 0 },
        { x: 70, y: 20, z: 0 },
        { x: 60, y: 30, z: 0 },
        { x: 50, y: 40, z: 0 },
        { x: 40, y: 50, z: 0 },
        { x: 30, y: 65, z: 0 },
        { x: 20, y: 80, z: 0 }
      ],
      name: "Robot-B",
      timeToComplete: 4800,
      collisions: [],
      goalPosition: { x: 20, y: 80, z: 0 },
      goalReached: true
    },
    {
      robotStart: { x: 50, y: 10, z: 0 },
      robotEnd: { x: 45, y: 85, z: 0 },
      robotPath: [
        { x: 50, y: 10, z: 0 },
        { x: 52, y: 25, z: 0 },
        { x: 50, y: 40, z: 0 },
        { x: 48, y: 55, z: 0 },
        { x: 46, y: 70, z: 0 },
        { x: 45, y: 85, z: 0 }
      ],
      name: "Robot-C",
      timeToComplete: 3500,
      collisions: [
        { x: 48, y: 55, z: 0 },
        { x: 46, y: 70, z: 0 }
      ],
      goalPosition: { x: 45, y: 85, z: 0 },
      goalReached: true
    },
    {
      robotStart: { x: 10, y: 80, z: 0 },
      robotEnd: { x: 75, y: 15, z: 0 },
      robotPath: [
        { x: 10, y: 80, z: 0 },
        { x: 20, y: 70, z: 0 },
        { x: 30, y: 60, z: 0 },
        { x: 40, y: 50, z: 0 },
        { x: 50, y: 40, z: 0 },
        { x: 60, y: 30, z: 0 },
        { x: 75, y: 15, z: 0 }
      ],
      name: "Robot-D",
      timeToComplete: 6100,
      collisions: [],
      goalPosition: { x: 80, y: 10, z: 0 },
      goalReached: false
    }
  ];
  
  export default exampleRobotDataList;