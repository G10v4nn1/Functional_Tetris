using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicDefs : MonoBehaviour
{
    [SerializeField] private Mesh in_blockMesh;
    [SerializeField] private Material in_blockMaterial;
    [SerializeField] private Material in_ghostMaterial;
    private static Mesh blockMesh;
    private static Material blockMaterial;
    private static Material ghostMaterial;

    private void Start()
    {
        blockMesh = in_blockMesh;
        blockMaterial = in_blockMaterial;
        ghostMaterial = in_ghostMaterial;
    }

    static Block blk_draw(GameField gameField)
    {
        return new Block(blockMesh, blockMaterial, gameField);
    }

    static Block ght_draw(GameField gameField)
    {
        return new Block(blockMesh, ghostMaterial, gameField);
    }
    public static void blk_draw(BlockGroup blkG, int col, int row)
    {
        blkG.AddBlock(GraphicDefs.blk_draw(blkG.GetField()).Move(blkG.GetLocation().x + col + ((blkG.GetLocation().y - row) * blkG.GetField().GetDimentions().width)));
    }
    public static void blk_drawGhost(BlockGroup blkG, int col, int row)
    {
        blkG.AddBlock(GraphicDefs.ght_draw(blkG.GetField()).Move(blkG.GetLocation().x + col + ((blkG.GetLocation().y - row) * blkG.GetField().GetDimentions().width)));
    }
}
