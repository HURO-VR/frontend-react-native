using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MakeMaker : MonoBehaviour
{

    [SerializeField] private MazeCell mazeCellPrefab;

    [SerializeField] private int mazeWidth;

    [SerializeField] private int mazeDepth;

    private MazeCell[,] mazeGrid;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for (int i = 0; i < mazeWidth; i++) 
        {
            for (int j = 0; j < mazeDepth; j++) 
            {
                mazeGrid[i, j] = Instantiate(mazeCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, mazeGrid[0,0]);

    }

    private IEnumerator GenerateMaze(MazeCell prevCell, MazeCell currCell) 
    {
        currCell.Visit();
        clearWalls(prevCell, currCell);

        yield return new WaitForSeconds(0.05f);

        MazeCell nextCell;

        do
        {
            nextCell = getNextUnvisitedCell(currCell);

            if (nextCell != null) 
            {
                yield return GenerateMaze(currCell, nextCell);
            } 
        } while (nextCell != null);
    }

    private MazeCell getNextUnvisitedCell(MazeCell currCell) 
    {
        var unvisitedCell = GetunvisitedCells(currCell);
        
        return unvisitedCell.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetunvisitedCells(MazeCell currCell) 
    {
        int x = (int)currCell.transform.position.x;
        int z = (int)currCell.transform.position.z;

        if (x + 1 < mazeWidth) 
        {
            var cRight = mazeGrid[x + 1, z];

            if (cRight.isVisited == false) 
            {
                yield return cRight;
            }
        }

        if (x - 1 >= 0) 
        {
            var cLeft = mazeGrid[x - 1, z];

            if (cLeft.isVisited == false) 
            {
                yield return cLeft;
            }
        }

        if (z + 1 < mazeDepth) 
        {
            var cFront = mazeGrid[x, z + 1];

            if (cFront.isVisited == false) 
            {
                yield return cFront;
            }
        }
        
        if (z - 1 >= 0) 
        {
            var cBack = mazeGrid[x, z - 1];

            if (cBack.isVisited == false) 
            {
                yield return cBack;
            }
        }
    }

    private void clearWalls(MazeCell prevCell, MazeCell currCell) 
    {
        if (prevCell == null) 
        {
            return;
        }

        if (prevCell.transform.position.x < currCell.transform.position.x) 
        {
            prevCell.ClearRightWall();
            currCell.ClearLeftWall();
            return;
        }

        if (prevCell.transform.position.x > currCell.transform.position.x) 
        {
            prevCell.ClearLeftWall();
            currCell.ClearRightWall();
            return;
        }

        if (prevCell.transform.position.z < currCell.transform.position.z) 
        {
            prevCell.ClearFrontWall();
            currCell.ClearBackWall();
            return;
        }

        if (prevCell.transform.position.z > currCell.transform.position.z) 
        {
            prevCell.ClearBackWall();
            currCell.ClearFrontWall();
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
