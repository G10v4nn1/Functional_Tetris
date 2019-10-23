using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockControl : MonoBehaviour
{
    static void blk_solo_move(Block blk, int _direction)
    {
        blk.index += _direction;
        blk.gameObject.transform.parent = blk.gameField.GetField()[blk.index].space.transform;
        blk.gameObject.transform.localPosition = Vector3.zero;
    }
    static void blk_solo_moveInField(Block blk, int _direction)
    {
        blk.gameField.GetField()[blk.index].child = null;
        blk.gameField.GetField()[blk.index + _direction].child = blk;
        blk_solo_move(blk, _direction);
    }
    static bool blk_solo_checkDirectionHorizontal(Block blk, int direction)
    {
        bool directionCheck = direction < 0 ? blk.index % blk.gameField.GetDimentions().width == 0 : (blk.index + direction) % blk.gameField.GetDimentions().width == 0;
        if (blk.index + direction < 0
        || directionCheck
        || blk.gameField.GetField()[blk.index + direction].child != null) return false; //if one of the pieces is in the first index of the row, don't go any further
        return true;
    }
    static bool blk_solo_checkDirectionVertical(Block blk, int direction)
    {
        if (blk.index + direction < 0 || blk.gameField.GetField()[blk.index + direction].child != null) return false; //if one of the pieces is in the first index of the row, don't go any further
        return true;
    }
    public static void blk_solo_clear(Block blk)
    {
        Destroy(blk.gameObject);
    }
    public static void blk_solo_moveLeft(Block blk)
    {
        blk_solo_move(blk, -1);
    }
    public static void blk_solo_moveRight(Block blk)
    {
        blk_solo_move(blk, 1);
    }
    public static void blk_solo_moveDown(Block blk)
    {
        blk_solo_move(blk, -blk.gameField.GetDimentions().width);
    }
    public static void blk_solo_moveDownInField(Block blk)
    {
        blk_solo_moveInField(blk, -blk.gameField.GetDimentions().width);
    }
    public static void blk_solo_moveUp(Block blk)
    {
        blk_solo_move(blk, blk.gameField.GetDimentions().width);
    }
    public static void blk_cnj_foreach(Action<Block> fn, List<Block> blks, int i)
    {
        if (i < blks.Count)
        {
            fn(blks[i]);
            blk_cnj_foreach(fn, blks, i + 1);
        }
    }
    public static bool blk_cnj_foreach(Func<Block, bool> fn, List<Block> blks, int i)
    {
        if (i < blks.Count)
        {
            bool val = fn(blks[i]);
            return val == false ? false : blk_cnj_foreach(fn, blks, i + 1);
        }
        return true;
    }
    public static void blk_cnj_clear(List<Block> blks)
    {
        blk_cnj_foreach(blk_solo_clear, blks, 0);
        blks.Clear();
    }
    public static bool blk_solo_checkLeft(Block blk)
    {
        return blk_solo_checkDirectionHorizontal(blk, -1);
    }
    public static bool blk_solo_checkRight(Block blk)
    {
        return blk_solo_checkDirectionHorizontal(blk, 1);
    }
    public static bool blk_solo_checkDown(Block blk)
    {
        return blk_solo_checkDirectionVertical(blk, -blk.gameField.GetDimentions().width);
    }
    public static void blk_solo_lockBlock(Block blk)
    {
        blk.gameField.GetField()[blk.index].child = blk;
    }
}
