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


public class WalkerGenerator : MonoBehaviour
{
    public enum Grid {
        Ground,
        WALL,
        OBSTACLE,
        EMPTY
    }
    private List<WalkerObject> Walkers;
    private List<Vector3Int> Walls = new List<Vector3Int>();
    public Tilemap GroundTilemap;
    public Tilemap WallTileMap;
    public Tilemap ObstacleTilemap;
    public Tile Ground1;
    public Tile Ground2;
    public Tile Ground3;
    public Tile Wall;
    public Tile Obstacle;

    public int MapWidth = 14;
    public int MapHeight = 8;

    private int MaximumWalkers = 2;
    private int TileCount = 0;
    private float FillPercent = 0.5f;

    


    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid() {

        Walkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(MapWidth / 2, MapHeight / 2, 0);

        WalkerObject currentWalker = new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), RandomDirection(), 0.5f);

        int groundNum = Random.Range(1,3);
        switch (groundNum) {
            case 1:
                GroundTilemap.SetTile(tileCenter, Ground1);
                break;
            case 2:
                GroundTilemap.SetTile(tileCenter, Ground2);
                break;
            case 3:
                GroundTilemap.SetTile(tileCenter, Ground3);
                break;
        }
        
        TileCount++;
        Walkers.Add(currentWalker);
        
        CreateGround();
    }

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

    void CreateGround() {

        while (((float)TileCount / (float)(MapWidth * MapHeight)) < FillPercent) {

            foreach (WalkerObject currentWalker in Walkers) {
                Vector3Int curPos = new Vector3Int((int)currentWalker.GetPos().x, (int)currentWalker.GetPos().y, 0);
                if (GroundTilemap.GetTile<Tile>(curPos) == null) {
                    int groundNum = Random.Range(1,3);
                    switch (groundNum) {
                        case 1:
                            GroundTilemap.SetTile(curPos, Ground1);
                            break;
                        case 2:
                            GroundTilemap.SetTile(curPos, Ground2);
                            break;
                        case 3:
                            GroundTilemap.SetTile(curPos, Ground3);
                            break;
                    }
                    
                    TileCount++;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
        }
        CreateWalls();
    }

    void ChanceToRemove() {
        for (int i = 0; i < Walkers.Count; i++) {
            if (UnityEngine.Random.value < Walkers[i].GetChance() && Walkers.Count > 1) {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }


    void ChanceToRedirect() {
        for (int i = 0; i < Walkers.Count; i++) {
            if (UnityEngine.Random.value < Walkers[i].GetChance()) {
                WalkerObject currentWalker = Walkers[i];
                currentWalker.SetDir(RandomDirection());
                Walkers[i] = currentWalker;
            }
        }
    }

    void ChanceToCreate() {
        int countSave = Walkers.Count;
        for (int i = 0; i < countSave; i++) {
            if (UnityEngine.Random.value < Walkers[i].GetChance() && Walkers.Count < MaximumWalkers) {
                Vector2 newDir = RandomDirection();
                Vector2 newPos = Walkers[i].GetPos();

                WalkerObject nextWalker = new WalkerObject(newPos, newDir, Walkers[i].GetChance());
                Walkers.Add(nextWalker);               
            }
        }
    }

    void UpdatePosition() {
        for (int i = 0; i < Walkers.Count; i++) {
            WalkerObject currentWalker = Walkers[i];
            Vector2 nextPos = currentWalker.GetPos() + currentWalker.GetDir();
            
            nextPos.x = Mathf.Clamp(nextPos.x, 0, MapWidth - 2);
            nextPos.y = Mathf.Clamp(nextPos.y, 0, MapHeight - 2);
            currentWalker.SetPos(nextPos);

            Walkers[i] = currentWalker;
        }
    }

    void CreateWalls() {
        for (int x = 0; x < MapWidth- 1; x++) {
            for (int y = 0; y < MapHeight - 1; y++) {
                Vector3Int thisPos = new Vector3Int(x,y,0);
                if (GroundTilemap.GetTile<Tile>(thisPos) != null) {
                    thisPos.x = x + 1;
                    if (GroundTilemap.GetTile(thisPos) == null) {
                        WallTileMap.SetTile(thisPos, Wall);
                        Walls.Add(thisPos);
                    }
                    thisPos.x = x - 1;
                    if (GroundTilemap.GetTile(thisPos) == null) {
                        WallTileMap.SetTile(thisPos, Wall);
                        Walls.Add(thisPos);
                    }
                    thisPos.x = x;
                    thisPos.y = y + 1;
                    if (GroundTilemap.GetTile(thisPos) == null) {
                        WallTileMap.SetTile(thisPos, Wall);
                        Walls.Add(thisPos);
                    }
                    thisPos.y = y - 1;
                    if (GroundTilemap.GetTile(thisPos) == null) {
                        WallTileMap.SetTile(thisPos, Wall);
                        Walls.Add(thisPos);
                    }
                    
                }
            }
        }
        CreateObstacles();
    }


    bool DirectionCheck(Vector3Int pos1, Vector3Int pos2, Vector3Int pos3) {
        if (ObstacleTilemap.GetTile<Tile>(pos1) != null || GroundTilemap.GetTile<Tile>(pos1) != null) {
            return true;
        }
        if (ObstacleTilemap.GetTile<Tile>(pos2) != null || GroundTilemap.GetTile<Tile>(pos1) != null) {
            return true;
        }
        if (ObstacleTilemap.GetTile<Tile>(pos3) != null || GroundTilemap.GetTile<Tile>(pos1) != null) {
            return true;
        }
        return false;
    }

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

    void CreateObstacles() {
        int checks;
        foreach (Vector3Int wallPos in Walls) {
            checks = 0;

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

            if (checks >= 3) {
                ObstacleTilemap.SetTile(wallPos, Obstacle);
            } else {
                WallTileMap.SetTile(wallPos, Wall);
            }

        }
    }

}
