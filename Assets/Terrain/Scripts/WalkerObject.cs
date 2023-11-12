// Inspired By: https://github.com/GarnetKane99/RandomWalkerAlgo_YT


using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WalkerObject
{

    private Vector2 Position;
    private Vector2 Direction;
    private float ChanceToChange;

    // Constructor
    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange) {
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
    }

    public Vector2 GetPos() {
        return Position;
    }
    public void SetPos(Vector2 pos) {
        Position = pos;
    }

    public Vector2 GetDir() {
        return Direction;
    }
    public void SetDir(Vector2 dir) {
        Direction = dir;
    }

    public float GetChance() {
        return ChanceToChange;
    }

}
