// Inspired By: https://github.com/GarnetKane99/RandomWalkerAlgo_YT


using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Walker Class
/// 
/// Defines Walkers, which are used to create random maps
/// </summary>
public class WalkerObject
{
    // Position of the walker 
    private Vector2 wPosition;
    // Direction in which the walker object will move
    private Vector2 wDirection;
    // Odds of the walker "changing" (Removed, Redirected, Create a new Walker)
    private float wChanceToChange;

    // Constructor
    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange) {
        wPosition = pos;
        wDirection = dir;
        wChanceToChange = chanceToChange;
    }

    /// <summary>
    /// Get current position of the walker
    /// </summary>
    /// <returns> Position of the walker </returns>
    public Vector2 GetPos() {
        return wPosition;
    }

    /// <summary>
    /// Set the new position of the walker
    /// </summary>
    /// <param name="pos"> Walker's new position </param>
    public void SetPos(Vector2 pos) {
        wPosition = pos;
    }

    /// <summary>
    /// Get current direction of the walker
    /// </summary>
    /// <returns> Direciton of the walker </returns>
    public Vector2 GetDir() {
        return wDirection;
    }
    /// <summary>
    /// Set the new direction of the walker
    /// </summary>
    /// <param name="dir"> Walker's new direction </param>
    public void SetDir(Vector2 dir) {
        wDirection = dir;
    }

    /// <summary>
    /// Get the number representing the odds of the walker having a "change"
    /// </summary>
    /// <returns> Chance that walker has a change </returns>
    public float GetChance() {
        return wChanceToChange;
    }

}
