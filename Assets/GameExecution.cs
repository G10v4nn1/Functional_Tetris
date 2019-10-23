using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class GameExecution : MonoBehaviour
{
    int[][] currentPiece;
    static bool recentlyChanged = false;
    float tick = 1f;

    static System.Random random = new System.Random();
    public static GameField field;
    public static GameField hold;
    public static GameField next;
    public static Text score;
    public Image gamePanel;
    public Image holdPanel;
    public Image nextPanel;
    public Text scoreCounter;
    static Tetromino currentTetromino;
    static Tetromino holdTetromino;
    static Tetromino nextTetromino;

    static List<int[][]> tetrominoes = new List<int[][]> { Assets.tJ, Assets.tL, Assets.tO, Assets.tS, Assets.tT, Assets.tZ, Assets.tI };

    Func<BlockGroup, bool> checkDown = blockGroup =>
    {
        if(blockGroup != null)
        {
            foreach (Block b in blockGroup.GetBlocks())
            {
                if (b.index - field.GetDimentions().width < 0 || field.GetField()[b.index - field.GetDimentions().width].child != null)
                { 
                    blockControl.blk_cnj_foreach(blockControl.blk_solo_lockBlock, currentTetromino.GetBlocks(), 0);
                    blockControl.blk_cnj_clear(currentTetromino.GetGhost().GetBlocks());
                    updateField(currentTetromino);
                    currentTetromino = null;
                    recentlyChanged = false;
                    return false;
                };
            }
            blockGroup.SetLocation(new Vector2Int(0, -1));
        }
        return true;
    };

    void Start() {
        field = createField(new GameField((10, 20), gamePanel));
        hold = createField(new GameField((5, 4), holdPanel));
        next = createField(new GameField((5, 4), nextPanel));
        nextTetromino = TetrominoControls.createTetromino(next);
        score = scoreCounter;
        score.text = 0.ToString();
    }

    void Update() {
        if (currentTetromino == null) //the tetromino is never generated on its own, it is cloned from the next tetromino
        {
            StopAllCoroutines();
            currentTetromino = TetrominoControls.cloneTetromino(nextTetromino, field);
            TetrominoControls.CreateGhost(currentTetromino);
            
            blockControl.blk_cnj_clear(nextTetromino.GetBlocks());
            nextTetromino = TetrominoControls.createTetromino(next);

            StartCoroutine(goDown());
            changeColor(currentTetromino.GetBlocks());
        }
        if (Input.GetKeyDown("d")) // Moves to the right
        {
            if (blockControl.blk_cnj_foreach(blockControl.blk_solo_checkRight, currentTetromino.GetBlocks(), 0))
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveRight, currentTetromino.GetBlocks(), 0);
                currentTetromino.SetLocation(new Vector2Int(1, 0));
                TetrominoControls.CreateGhost(currentTetromino);
            }
        }
        if (Input.GetKeyDown("a"))// Moves to the left
        {
            if (blockControl.blk_cnj_foreach(blockControl.blk_solo_checkLeft, currentTetromino.GetBlocks(), 0))
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveLeft, currentTetromino.GetBlocks(), 0);
                currentTetromino.SetLocation(new Vector2Int(-1, 0));
                TetrominoControls.CreateGhost(currentTetromino);
            }
        }
        if (Input.GetKeyDown("s"))// Moves Down
        {
            if (currentTetromino != null)
            {
                StopAllCoroutines();
                tick = 0.05f;
                StartCoroutine(goDown());
            }
        }
        if (Input.GetKeyUp("s"))// Moves Down
        {
            StopAllCoroutines();
            tick = 1f;
            StartCoroutine(goDown());
        }
        if (Input.GetKeyDown("w"))
        {
            rotation();
            TetrominoControls.CreateGhost(currentTetromino);
        }
        if (Input.GetKeyDown("f"))
        {
            TetrominoControls.fitPiece(currentTetromino, currentTetromino.GetField());
            UiControl.scr_add(score, 36);
            checkDown(currentTetromino);
        }
        if (Input.GetKeyDown("c") && recentlyChanged == false)
        {
            Tetromino cloneAux = null;

            if(holdTetromino != null)
            {
                cloneAux = holdTetromino;
                blockControl.blk_cnj_clear(holdTetromino.GetBlocks());
            }

            holdTetromino = TetrominoControls.cloneTetromino(currentTetromino, hold);
            blockControl.blk_cnj_clear(currentTetromino.GetBlocks());
            blockControl.blk_cnj_clear(currentTetromino.GetGhost().GetBlocks());
            currentTetromino = TetrominoControls.cloneTetromino(cloneAux, field);
            TetrominoControls.CreateGhost(currentTetromino);
            recentlyChanged = true;
        }
    }

    //This piece of code will make all the tetromino binaries 4 characters long
    
    // Make the tetromino go down every 1 second
    IEnumerator goDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(tick);
            if (checkDown(currentTetromino))
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveDown, currentTetromino.GetBlocks(), 0);
            };
        }
    }

    IEnumerator changeColor(List<Block> blkList)
    {
        if(currentTetromino != null)
        {
            blockControl.blk_cnj_foreach(TetrominoControls.ttm_view_colorBlue, blkList, 0);
            blockControl.blk_cnj_foreach(TetrominoControls.ttm_view_colorGreen, blkList, 0);
            blockControl.blk_cnj_foreach(TetrominoControls.ttm_view_colorRed, blkList, 0);
            return changeColor(blkList);
        }
        return null;
    }

    public GameField createField(GameField field)
    {
        (int width, int height) dimentions = field.GetDimentions();
        (GameObject space, Block child)[] newField = new (GameObject, Block)[dimentions.width * dimentions.height];
        Vector2 halfUnit = new Vector2((field.GetUnit().x / 2), (field.GetUnit().y / 2));
        (float X, float Y) location = (0, 0);

        for (int i = 0; i < newField.Length; i++)
        {
            newField[i].space = new GameObject();
            newField[i].space.transform.parent = field.GetPanel().transform;
            newField[i].space.transform.localPosition = new Vector3(location.X * field.GetUnit().x + halfUnit.x, location.Y * field.GetUnit().y + halfUnit.y, 0);
            newField[i].child = null;
            //this will determine the location
            location.X = location.X + 1;
            if (location.X % dimentions.width == 0)
            {
                location = (0, location.Y + 1);
            }
        }
        return field.setField(newField);
    }

    static void updateField(Tetromino ttm) // Destroys a line and move the remaining pieces down
    {
       bool checkRow(int row, int curr)
        {
            if(row >= 0)
            {
                Debug.Log(field.GetField()[curr + (row * field.GetDimentions().width)].child != null);
                if (field.GetField()[curr + (row * field.GetDimentions().width)].child == null) return false;
                if (curr == field.GetDimentions().width - 1) return true;
                return checkRow(row, curr + 1);
            }
            return false;
        }

        bool moveHorizontal(int row, int curr)
        {
            if(curr < ttm.GetField().GetDimentions().width)
            {
                blockControl.blk_solo_clear(field.GetField()[curr + (row * field.GetDimentions().width)].child);
                field.GetField()[curr + (row * field.GetDimentions().width)].child = null;
                return moveHorizontal(row, curr + 1);
            }
            return true;
        }

        bool eraseRow(int curr)
        {
            if (curr > ttm.GetLocation().y - 4)
            {
                if (checkRow(curr, 0))
                {
                    moveHorizontal(curr, 0);
                    IEnumerable<(GameObject space, Block child)> remainingSpaces = field.GetField().Where(x => x.child != null && x.child.index >= curr * field.GetDimentions().width);
                    IEnumerable<Block> remainingBlocks = remainingSpaces.Select(x => x.child);
                    blockControl.blk_cnj_foreach(blockControl.blk_solo_moveDownInField, remainingBlocks.ToList(), 0);
                    UiControl.scr_add(score, 100);
                    eraseRow(curr);
                }
                eraseRow(curr - 1);
            }
            return true;
        }
        eraseRow(ttm.GetLocation().y);
    }

    //This will rotate the tetromino TODO move left and move right
    void rotation()
    {
        blockControl.blk_cnj_clear(currentTetromino.GetBlocks());
        currentTetromino.IncreaseRotation();
        TetrominoControls.bn_map_forEachTrue(GraphicDefs.blk_draw, currentTetromino);

        foreach (Block b in currentTetromino.GetBlocks())
        {
            if (currentTetromino.GetLocation().x > 0 && b.index % field.GetDimentions().width == 0)
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveLeft, currentTetromino.GetBlocks(), 0);
                currentTetromino.SetLocation(new Vector2Int(-1, 0));
            }
            if (currentTetromino.GetLocation().x < 0 && b.index % field.GetDimentions().width != 0)
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveRight, currentTetromino.GetBlocks(), 0);
                currentTetromino.SetLocation(new Vector2Int(1, 0));
            }
            while(field.GetField()[b.index].child != null)
            {
                blockControl.blk_cnj_foreach(blockControl.blk_solo_moveUp, currentTetromino.GetBlocks(), 0);
                currentTetromino.SetLocation(new Vector2Int(0, 1));
            }
        }
    }
}

public class Block
{
    public GameObject gameObject = new GameObject();

    public int index = 0;

    public GameField gameField;

    public Block(Mesh mesh, Material material, GameField field)
    {
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
        gameField = field;
        gameObject.transform.localScale = new Vector3(field.GetUnit().x, field.GetUnit().y, field.GetUnit().x * 2);
    }

    public Block Move(int _direction)
    {
        index += _direction;
        gameObject.transform.parent = gameField.GetField()[index].space.transform;
        gameObject.transform.localPosition = Vector3.zero;
        return this;
    }
}

public class Assets
{
    public static int[][] tZ = { new int[]{ 0, 6, 3}, new int[] { 0, 2, 6, 4 } };
    public static int[][] tS = { new int[] { 0, 3, 6}, new int[] { 4, 6, 2} };
    public static int[][] tT = { new int[] { 7, 2}, new int[] {2, 3, 2}, new int[] { 2, 7}, new int[] { 2, 6, 2} };
    public static int[][] tO = { new int[] { 0, 6, 6} };
    public static int[][] tJ = { new int[] { 2, 2, 6}, new int[] { 4, 7}, new int[] { 6, 4, 4}, new int[] { 0, 7, 1 } };
    public static int[][] tL = { new int[] { 4, 4, 6}, new int[] { 2, 14 }, new int[] { 12, 4, 4 }, new int[] { 14, 8 } };
    public static int[][] tI = { new int[] { 4, 4, 4, 4 }, new int[] { 0, 15 } };
}


public class GameField {

    (GameObject space, Block child)[] field;

    (int width, int height) dimentions;

    Image parentPanel;

    public GameField((int width, int height) newDimentions, Image panel)
    {
        dimentions = newDimentions;
        parentPanel = panel;
    }

    public (int width, int height) GetDimentions()
    {
        return dimentions;
    }

    public (GameObject space, Block child)[] GetField()
    {
        return field;
    }

    public GameField setField((GameObject space, Block child)[] newField)
    {
        field = newField;
        return this;
    }

    public Image GetPanel()
    {
        return parentPanel;
    }

    public Vector2 GetUnit()
    {
        RectTransform rect = parentPanel.GetComponent<RectTransform>();
        return new Vector2(rect.sizeDelta.x / dimentions.width, rect.sizeDelta.y / dimentions.height);
    }
} 

public interface BlockGroup
{
    List<Block> GetBlocks();
    Vector2Int GetLocation();
    GameField GetField();
    void SetLocation(Vector2Int newLocation);
    BlockGroup SetBlock(List<Block> block);
    void CleanBlocks();
    int[] GetPiece();
    void AddBlock(Block blk);
}

public class Tetromino : BlockGroup
{
    List<Block> blocks = new List<Block>();
    Vector2Int location;
    public int rotation;
    int[][] pieceGroup;
    GameField gameField;
    public Ghost ghost;

    public Tetromino(int[][] pieceGroup, GameField field)
    {
        this.pieceGroup = pieceGroup;
        this.gameField = field;
        this.location = new Vector2Int((gameField.GetDimentions().width / 2) - 2, gameField.GetDimentions().height - 1);
    }

    public int[] GetPiece()
    {
        return pieceGroup[rotation];
    }

    public int[][] GetPieceGroup()
    {
        return pieceGroup;
    }

    public int GetRotation()
    {
        return rotation;
    }

    public List<Block> GetBlocks()
    {
        return blocks;
    }

    public Vector2Int GetLocation()
    {
        return location;
    }

    public void SetLocation(Vector2Int newLocation)
    {
        this.location += newLocation;
    }

    public BlockGroup SetBlock(List<Block> block)
    {
        this.blocks = block;
        return this;
    }

    public void AddBlock(Block blk)
    {
        blocks.Add(blk);
    }

    public void CleanBlocks()
    {
        blocks.Clear();
    }

    public int[] IncreaseRotation()
    {
        int newRotation = GetRotation() + 1;
        this.rotation = (newRotation == pieceGroup.Length) ? 0 : newRotation;
        return this.GetPiece();
    }

    public Ghost GetGhost()
    {
        return ghost;
    }

    public void NewGhost()
    {
        ghost = new Ghost(this);
    }

    public GameField GetField()
    {
        return gameField;
    }
}

public class Ghost : BlockGroup
{
    Vector2Int location;
    List<Block> blocks = new List<Block>();
    GameField gameField;

    public Ghost(Tetromino tetromino)
    {
        location = tetromino.GetLocation();
        gameField = tetromino.GetField();
    }

    public List<Block> GetBlocks()
    {
        return blocks;
    }

    public BlockGroup SetBlock(List<Block> block)
    {
        this.blocks = block;
        return this;
    }

    public void AddBlock(Block blk)
    {
        blocks.Add(blk);
    }

    public Vector2Int GetLocation()
    {
        return location;
    }

    public void SetLocation(Vector2Int newLocation)
    {
        this.location += newLocation;
    }

    public void CleanBlocks()
    {
        this.blocks.Clear();
    }
    
    public int[] GetPiece()
    {
        return null;
    }

    public GameField GetField()
    {
        return gameField;
    }
}