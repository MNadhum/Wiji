// Inspired By: https://github.com/GarnetKane99/RandomWalkerAlgo_YT


using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

/// <summary>
/// Generator Class
/// 
/// Generates walkers, which are used to create random maps
/// </summary>
public class WalkerGenerator : MonoBehaviour
{
    // Defines different types of possible tiles
    public enum Grid {
        Ground,
        wWallMiddle,
        wObstacle,
        EMPTY
    }
    
    // List of all active walkers
    private List<WalkerObject> wWalkers;
    // List of all wall positions
    private List<Vector3Int> wWalls = new List<Vector3Int>();
    // Tilemap of all Ground tiles
    public Tilemap wGroundTilemap;
    // Tilemap of all Wall tiles
    public Tilemap wWallTilemap;
    // Tilemap of all Obstacle tiles
    public Tilemap wObstacleTilemap;
    // Ground Tile - Design 1
    public Tile wGround1;
    // Ground Tile - Design 2
    public Tile wGround2;
    // Ground Tile - Design 3
    public Tile wGround3;
    // Wall Tile - Design 1
    public Tile wWallMiddle;
        // Wall Tile - Design 1
    public Tile wWallTop;
        // Wall Tile - Design 1
    public Tile wWallRight;
        // Wall Tile - Design 1
    public Tile wWallBottom;
        // Wall Tile - Design 1
    public Tile wWallLeft;
    // Obstacle Tile - Design 1
    public Tile wObstacle;

    // Width of the entire dungeon
    public int wMapWidth = 14;
    // Height of the entire dungeon
    public int wMapHeight = 8;

    // Maximum number of active walkers to use
    private int wMaximumWalkers = 2;
    // Number of tiles current placed
    private int wTileCount = 0;
    // Percent of map area to fill 
    private float wFillPercent = 0.5f;

    


    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
    }

    /// <summary>
    /// Starts the process of creating a random map
    /// </summary>
    void InitializeGrid() {

        wWalkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(wMapWidth / 2, wMapHeight / 2, 0);

        // Create the first walker, starting at the center tile, and give it a random direction to move in
        WalkerObject currentWalker = new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), RandomDirection(), 0.5f);

        // Randomly assign Ground design to use
        int groundNum = Random.Range(1,3);
        switch (groundNum) {
            case 1:
                wGroundTilemap.SetTile(tileCenter, wGround1);
                break;
            case 2:
                wGroundTilemap.SetTile(tileCenter, wGround2);
                break;
            case 3:
                wGroundTilemap.SetTile(tileCenter, wGround3);
                break;
        }
        
        wTileCount++;
        wWalkers.Add(currentWalker);
        
        CreateGround();
    }

    /// <summary>
    /// Gives a random direction, used by Walkers to create a random map
    /// 
    /// Return: Direction for walker to move in (up, down, left, right)
    /// </summary>
    Vector2 RandomDirection() {
        int dir = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (dir) {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    /// <summary>
    /// Randomly generates all of the ground tiles in the map.
    /// </summary>
    void CreateGround() {

        while (((float)wTileCount / (float)(wMapWidth * wMapHeight)) < wFillPercent) {

            // If the walker is on a null tile, make it a ground tile
            foreach (WalkerObject currentWalker in wWalkers) {
                Vector3Int curPos = new Vector3Int((int)currentWalker.GetPos().x, (int)currentWalker.GetPos().y, 0);
                if (wGroundTilemap.GetTile<Tile>(curPos) == null) {
                    int groundNum = Random.Range(1,3);
                    switch (groundNum) {
                        case 1:
                            wGroundTilemap.SetTile(curPos, wGround1);
                            break;
                        case 2:
                            wGroundTilemap.SetTile(curPos, wGround2);
                            break;
                        case 3:
                            wGroundTilemap.SetTile(curPos, wGround3);
                            break;
                    }
                    
                    wTileCount++;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
        }
        CreateWalls();
    }

    /// <summary>
    /// Determines whether a Walker should be removed (helps with randomization)
    /// </summary>
    void ChanceToRemove() {
        for (int i = 0; i < wWalkers.Count; i++) {
            if (UnityEngine.Random.value < wWalkers[i].GetChance() && wWalkers.Count > 1) {
                wWalkers.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Determines whether Walker should change directions (helps with randomization)
    /// </summary>
    void ChanceToRedirect() {
        for (int i = 0; i < wWalkers.Count; i++) {
            if (UnityEngine.Random.value < wWalkers[i].GetChance()) {
                WalkerObject currentWalker = wWalkers[i];
                currentWalker.SetDir(RandomDirection());
                wWalkers[i] = currentWalker;
            }
        }
    }

    /// <summary>
    /// Determines whether a new Walker should be created (helps with randomization)
    /// </summary>
    void ChanceToCreate() {
        int countSave = wWalkers.Count;
        for (int i = 0; i < countSave; i++) {
            if (UnityEngine.Random.value < wWalkers[i].GetChance() && wWalkers.Count < wMaximumWalkers) {
                Vector2 newDir = RandomDirection();
                Vector2 newPos = wWalkers[i].GetPos();

                WalkerObject nextWalker = new WalkerObject(newPos, newDir, wWalkers[i].GetChance());
                wWalkers.Add(nextWalker);               
            }
        }
    }

    /// <summary>
    /// "Moves" the Walkers to a new location based on current location + direction of the walker
    /// </summary>
    void UpdatePosition() {
        for (int i = 0; i < wWalkers.Count; i++) {
            WalkerObject currentWalker = wWalkers[i];
            Vector2 nextPos = currentWalker.GetPos() + currentWalker.GetDir();
            
            nextPos.x = Mathf.Clamp(nextPos.x, 0, wMapWidth - 2);
            nextPos.y = Mathf.Clamp(nextPos.y, 0, wMapHeight - 2);
            currentWalker.SetPos(nextPos);

            wWalkers[i] = currentWalker;
        }
    }

    /// <summary>
    /// Creates the walls in the game, which act as boundaries for the player
    /// </summary>
    void CreateWalls() {
        // Checks if current tile is a Ground tile, if it is, then makes walls around it
        for (int x = 0; x < wMapWidth- 1; x++) {
            for (int y = 0; y < wMapHeight - 1; y++) {
                Vector3Int thisPos = new Vector3Int(x,y,0);
                if (wGroundTilemap.GetTile<Tile>(thisPos) != null) {
                    thisPos.x = x + 1;
                    if (wGroundTilemap.GetTile(thisPos) == null) {
                        wWallTilemap.SetTile(thisPos, wWallMiddle);
                        wWalls.Add(thisPos);
                    }
                    thisPos.x = x - 1;
                    if (wGroundTilemap.GetTile(thisPos) == null) {
                        wWallTilemap.SetTile(thisPos, wWallMiddle);
                        wWalls.Add(thisPos);
                    }
                    thisPos.x = x;
                    thisPos.y = y + 1;
                    if (wGroundTilemap.GetTile(thisPos) == null) {
                        wWallTilemap.SetTile(thisPos, wWallMiddle);
                        wWalls.Add(thisPos);
                    }
                    thisPos.y = y - 1;
                    if (wGroundTilemap.GetTile(thisPos) == null) {
                        wWallTilemap.SetTile(thisPos, wWallMiddle);
                        wWalls.Add(thisPos);
                    }
                    
                }
            }
        }
        CreateObstacles();
    }

    /// <summary>
    /// Checks if any of the three given tiles are an Obstacle or a Wall, which is used to create obstacles
    /// </summary>
    /// <param name="pos1"> Position of first tile to check </param>
    /// <param name="pos2"> Position of second tile to check </param>
    /// <param name="pos3"> Position of third tile to check </param>
    /// <returns> True if any of the three tiles are either an obstacle or a wall</returns>
    bool DirectionCheck(Vector3Int pos1, Vector3Int pos2, Vector3Int pos3) {
        if (wObstacleTilemap.GetTile<Tile>(pos1) != null || wWallTilemap.GetTile<Tile>(pos1) != null || wGroundTilemap.GetTile<Tile>(pos1) != null) {
            return true;
        }
        if (wObstacleTilemap.GetTile<Tile>(pos2) != null || wWallTilemap.GetTile<Tile>(pos2) != null || wGroundTilemap.GetTile<Tile>(pos2) != null) {
            return true;
        }
        if (wObstacleTilemap.GetTile<Tile>(pos3) != null || wWallTilemap.GetTile<Tile>(pos3) != null || wGroundTilemap.GetTile<Tile>(pos3) != null) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the position of the three tiles ABOVE the wall tile, used to determine if the wall tile should be an obstacle
    /// 1 1 1 <- Gets the position of these
    /// 1 W 1
    /// 1 1 1
    /// </summary>
    /// <param name="pos"> Position of a wall tile</param>
    /// <returns>True if any of top 3 tiles are a wall or obstacle</returns>
    bool UpperThree(Vector3Int pos) {
        // x-1,y+1    x,y+1     x+1,y+1
        Vector3Int pos1 = pos;
        pos1.x -= 1;
        pos1.y += 1;

        Vector3Int pos2 = pos;
        pos2.y += 1;

        Vector3Int pos3 = pos;
        pos3.x += 1;
        pos3.y += 1;

        bool check = DirectionCheck(pos1, pos2, pos3);

        return check;
    }
    /// <summary>
    /// Gets the position of the three tiles BELOW the wall tile, used to determine if the wall tile should be an obstacle
    /// 1 1 1 
    /// 1 W 1
    /// 1 1 1 <- Gets the position of these
    /// </summary>
    /// <param name="pos"> Position of a wall tile</param>
    /// <returns>True if any of bottom 3 tiles are a wall or obstacle</returns>
    bool BottomThree(Vector3Int pos) {
        // x-1,y-1   x,y-1     x+1,y-1
        Vector3Int pos1 = pos;
        pos1.x -= 1;
        pos1.y -= 1;

        Vector3Int pos2 = pos;
        pos2.y -= 1;

        Vector3Int pos3 = pos;
        pos3.x += 1;
        pos3.y -= 1;

        bool check = DirectionCheck(pos1, pos2, pos3);

        return check;
    }
    /// <summary>
    /// Gets the position of the three tiles TO THE LEFT of the wall tile, used to determine if the wall tile should be an obstacle
    /// Gets the position of these tiles
    /// |
    /// |
    /// v
    /// 1 1 1 
    /// 1 W 1
    /// 1 1 1
    /// </summary>
    /// <param name="pos"> Position of a wall tile</param>
    /// <returns>True if any of left 3 tiles are a wall or obstacle</returns>
    bool LeftThree(Vector3Int pos) {
        // x-1,y+1  x-1,y      x-1,y-1
        Vector3Int pos1 = pos;
        pos1.x -= 1;
        pos1.y += 1;

        Vector3Int pos2 = pos;
        pos2.x -= 1;

        Vector3Int pos3 = pos;
        pos3.x -= 1;
        pos3.y -= 1;

        bool check = DirectionCheck(pos1, pos2, pos3);

        return check;
    }
    /// <summary>
    /// Gets the position of the three tiles TO THE RIGHT of the wall tile, used to determine if the wall tile should be an obstacle
    /// Gets the position of these tiles
    ///     |
    ///     |
    ///     v
    /// 1 1 1 
    /// 1 W 1
    /// 1 1 1
    /// </summary>
    /// <param name="pos"> Position of a wall tile</param>
    /// <returns>True if any of right 3 tiles are a wall or obstacle</returns>
    bool RightThree(Vector3Int pos) {
        // x+1,y+1   x+1,y     x+1,y-1
        Vector3Int pos1 = pos;
        pos1.x += 1;
        pos1.y += 1;

        Vector3Int pos2 = pos;
        pos2.x += 1;

        Vector3Int pos3 = pos;
        pos3.x += 1;
        pos3.y -= 1;

        bool check = DirectionCheck(pos1, pos2, pos3);

        return check;
    }

    bool SoloWall(Vector3Int pos) {
        // x+1,y  x-1,y  x,y+1  x,y-1

        Vector3Int above = new Vector3Int(pos.x, pos.y + 1);
        Vector3Int below = new Vector3Int(pos.x, pos.y - 1);
        Vector3Int left = new Vector3Int(pos.x - 1, pos.y);
        Vector3Int right = new Vector3Int(pos.x + 1, pos.y);

        Tile tileAbove = wWallTilemap.GetTile<Tile>(above);
        Tile tileBelow = wWallTilemap.GetTile<Tile>(below);
        Tile tileLeft = wWallTilemap.GetTile<Tile>(left);
        Tile tileRight = wWallTilemap.GetTile<Tile>(right);

        if (tileAbove == null && tileBelow == null &&
            tileLeft == null && tileRight == null) {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Turns some wall tiles into obstacles, based on their location within the map
    /// </summary>
    void CreateObstacles() {
        int checks;
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (Vector3Int wallPos in wWalls) {
            checks = 0;

            if (SoloWall(wallPos)) {
                wObstacleTilemap.SetTile(wallPos, wObstacle);
                wWallTilemap.SetTile(wallPos, null);
                toRemove.Add(wallPos);
                continue;
            }
            if (UpperThree(wallPos)) {
                checks++;
            }
            if (BottomThree(wallPos)) {
                checks++;
            }
            if (LeftThree(wallPos)) {
                checks++;
            }
            if (RightThree(wallPos)) {
                checks++;
            }

            if (checks > 3) {
                wObstacleTilemap.SetTile(wallPos, wObstacle);
                wWallTilemap.SetTile(wallPos, null);
                toRemove.Add(wallPos);
            }
        }
        
        foreach (Vector3Int removing in toRemove) {
            wWalls.Remove(removing);
        }
    }
    void SetWalls() {
    foreach (Vector3Int wallPos in wWalls) {
        
    }

}

}


// Set proper walls

// Set proper obstacles
// Create Doors
