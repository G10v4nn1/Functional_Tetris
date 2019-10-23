using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoControls : MonoBehaviour
{
    static System.Random random = new System.Random();

    public static int[][] bn_map_pickRandom()
    {
        List<int[][]> piecePool = new List<int[][]> { Assets.tJ, Assets.tL, Assets.tO, Assets.tS, Assets.tT, Assets.tZ, Assets.tI };
        return piecePool[random.Next(0, piecePool.Count)];
    }

    public static Tetromino ttm_create_new(GameField gf)
    {
        return new Tetromino(bn_map_pickRandom(), gf);
    }

    public static Tetromino ttm_create_copy(Tetromino ttm, GameField gf)
    {
        return new Tetromino(ttm.GetPieceGroup(), gf);
    }

    public static void bn_map_forEachTrue(Action<BlockGroup, int, int> fn, BlockGroup blkG, List<string> bin = null)
    {
        if(bin == null)
        {
            bin = TetrominoControls.mapTetromino(blkG.GetPiece());
        }
        
        bool read_row(int curr)
        {
            if (curr < bin.Count)
            {
                return read_col(bin[curr], curr, 0);
            }
            return true;
        }

        bool read_col(string s, int r, int curr) //read all columns of a row
        {
            if (curr < s.Length)
            {
                if (s[curr] == '1')
                {
                    fn(blkG, curr, r);
                }
                return read_col(s, r, curr + 1);
            }
            return read_row(r + 1);
        }

        read_row(0);
    }

    public static List<string> mapTetromino(int[] tetromino)
    {
        IEnumerable rawBinaryString = tetromino.Select(x => System.Convert.ToString(x, 2));
        List<string> sPieceMap = new List<string>();

        foreach (string s in rawBinaryString)
        {
            string sRefinedBinary = "";
            while (sRefinedBinary.Length < 4 - s.Length)
            {
                sRefinedBinary += '0';
            }
            sRefinedBinary += s;
            sPieceMap.Add(sRefinedBinary);
        }
        return sPieceMap;
    }

    public static Tetromino createTetromino(GameField field)
    {
        Tetromino newTetromino = TetrominoControls.ttm_create_new(field);
        TetrominoControls.bn_map_forEachTrue(GraphicDefs.blk_draw, newTetromino);
        return newTetromino;
    }

    public static Tetromino cloneTetromino(Tetromino ttm, GameField field)
    {
        if (ttm == null) return null;
        Tetromino clone = TetrominoControls.ttm_create_copy(ttm, field);
        TetrominoControls.bn_map_forEachTrue(GraphicDefs.blk_draw, clone);
        return clone;
    }

    public static void fitPiece(BlockGroup blockGroup, GameField field)
    {
        bool goDown()
        {
            if(blockControl.blk_cnj_foreach(blockControl.blk_solo_checkDown, blockGroup.GetBlocks(), 0)){
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveDown, blockGroup.GetBlocks(), 0);
                blockGroup.SetLocation(new Vector2Int(blockGroup.GetLocation().x, -1));
                return goDown();
            }
            return true;
        }

        goDown();
    }

    public static void CreateGhost(Tetromino ttm)
    {
        if (ttm.ghost != null)
        {
            blockControl.blk_cnj_clear(ttm.ghost.GetBlocks());
        }
        Ghost ghost = new Ghost(ttm);
        TetrominoControls.bn_map_forEachTrue(GraphicDefs.blk_drawGhost, ghost, TetrominoControls.mapTetromino(ttm.GetPiece()));
        TetrominoControls.fitPiece(ghost, ghost.GetField());
        ttm.ghost = ghost;
    }

    public static void ttm_view_colorRed(Block blk)
    {
         Color.Lerp(blk.gameObject.GetComponent<Material>().color, Color.red, 1f);
    }
    public static void ttm_view_colorBlue(Block blk)
    {

    }
    public static void ttm_view_colorGreen(Block blk)
    {
        Color.Lerp(blk.gameObject.GetComponent<Material>().color, Color.green, 1f);
    }
}
