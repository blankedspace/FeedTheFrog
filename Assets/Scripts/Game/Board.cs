using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Board : MonoBehaviour
{
    const int X_MAX_SIZE = 25;
    const int Y_MAX_SIZE = 25;
    public static List<Vector2Int> UDLR = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private Level _level;
    private Dictionary<Vector2Int,Tile> _tiles;
    private TileFactory _tileFactory;
    private int _movingTiles = 0;
    private Vector2Int _prevSwapOne, _prevSwapTwo;
    private Dictionary<Vector2Int, int> _tilesToSpawn = new Dictionary<Vector2Int, int>();



    public delegate void TilesDestroyed(HashSet<Vector2Int> tiles);
    public List<TilesDestroyed> OnTileDestroyed = new List<TilesDestroyed>();

    public delegate bool TilesSwaped(Vector2Int one, Vector2Int two);
    public TilesSwaped OnTileSwapedReplacement;

    public delegate void ChainOver();
    public List<ChainOver> OnChainOver = new List<ChainOver>();

    public bool Idle = false;

    public Tuple<Vector2Int, Vector2Int> GetLastSwapped()
    {

        return new Tuple<Vector2Int, Vector2Int>(_prevSwapOne, _prevSwapTwo );
    }

    public void OnLevelEndAndBlockInput()
    {
        
        GetComponent<BoardInput>().BlockInput();
    }
    public void ContinueLevel()
    {
        GetComponent<BoardInput>().UnblockInput();
    }

    private void Start()
    { 
        _level = FindObjectOfType<Level>();
        _tileFactory = GetComponent<TileFactory>();
        LeanTween.init(800);    
    }

    //Ask for to Board from outside;
    public void AskToSpawn(Vector2Int pos, int tile)
    {
        if(!_tilesToSpawn.ContainsKey(pos))
            _tilesToSpawn.Add(pos, tile);
    }

    public void AskToDestroy(Vector2Int pos,Action onDestroyComplete)
    {
        Tile tile = GetTile(pos);
        if (tile == null)
        {
            return;
        }
        for (int i = 0; i < OnTileDestroyed.Count; i++)
        {
            OnTileDestroyed[i].Invoke(new HashSet<Vector2Int>() { pos });
        }
        DestroyTile(pos, onDestroyComplete);
    }
    public void AskToDestroy(Vector2Int pos)
    {
       
        AskToDestroy(pos, null);
       

    }
    public void SwapTiles(Vector2Int one, Vector2Int two)
    {
        //if you change this change SwapTiles in "void AnyMoves" too
        
        Tile first = GetTile(_tiles, one);
        Tile second = GetTile(_tiles, two);
     
        if (first == null || second == null )
        {
            return;
        }
        if (!first.CanBeSwapped() || !second.CanBeSwapped())
        {
            return;
        }

        Idle = false;
        _tiles[one] = second;
        _tiles[two] = first;
        _prevSwapOne = one;
        _prevSwapTwo = two;
        first.OnMoveAnimEnd = OnTilesSwapComplete;
        first.MoveTo(two);
        second.MoveTo(one);

    }
    public Tile GetTile(Vector2Int Key)
    {
        if (_tiles.ContainsKey(Key))
        {
            return _tiles[Key];
        }
        return null;
    }
    private Tile GetTile(Dictionary<Vector2Int, Tile> tiles,Vector2Int Key)
    {
        if (tiles.ContainsKey(Key))
        {
            return tiles[Key];
        }
        return null;
    }
    public void InitializeBoard()
    {
        _tiles = new Dictionary<Vector2Int, Tile>();
        FillBoard();
    }
    public void InitializeBoardWithPreexistingTiles(Dictionary<Vector2Int, string> TileTypesInPositions)
    {
        _tiles = new Dictionary<Vector2Int, Tile>();
        foreach (var item in TileTypesInPositions)
        {
            SpawnTile(item.Key, item.Value, Vector2.up * 5f);
        }
        FillBoard();
    }
    private void FillBoard()
    {
        //move down existing tiles
        for (int x = 0; x < X_MAX_SIZE; x++)
        {
            for (int y = 1; y < Y_MAX_SIZE; y++)
            {
                Tile tile = GetTile(_tiles, new Vector2Int(x, y));
                if (tile != null)
                {
                    int i = 1;
                    while (GetTile(_tiles, new Vector2Int(x, y - i)) == null && y - i >= 0)
                    {
                        i++;
                    }
                    i--;
                    while (!_level.Elements.ContainsKey(new Vector2Int(x, y - i)))
                    {
                        i--;
                    }
                    if (i > 0)
                    {
                        _tiles[new Vector2Int(x, y - i)] = tile;
                        _movingTiles += 1;
                        tile.OnMoveAnimEnd = OnTilesMovementComplete;
                        tile.MoveTo(new Vector2Int(x, y - i));
                        _tiles[new Vector2Int(x, y)] = null;
                    }
                }
            }
        }
        SpawnNewTiles();
       
    }
    private void SpawnNewTiles()
    {
        foreach (var item in _level.Elements)
        {
            if (GetTile(_tiles,item.Key) == null)
            {
                
                SpawnTile(item.Key);
            }
        }
    }
    private void ReSpawnTiles()
    {
        foreach (var item in _level.Elements)
        {

            if (GetTile(item.Key)?.Type == 2000)
            {

                Tile tile = GetTile(item.Key);
                _tiles[item.Key] = null;
                tile.Destroy();
                SpawnTile(item.Key, "2000", Vector2.up * 5f);
                continue;
            }
            if (GetTile(item.Key) != null)
            {
                DestroyTile(item.Key, null);
            }
            SpawnTile(item.Key);
            
        }
    }
    private void SpawnTile(Vector2Int Key)
    {
        SpawnTile(Key, _level.KeysForTiles[UnityEngine.Random.Range(0, _level.KeysForTiles.Length)],Vector2.up*5f);
    }
    private void SpawnTile(Vector2Int Key,string tile,Vector2 offset)
    {
     

        Tile tempTile = _tileFactory.Create(tile);
        tempTile.transform.SetParent(transform);
        tempTile.transform.position = Key + offset;
        _movingTiles += 1;
        tempTile.OnMoveAnimEnd = OnTilesMovementComplete;
        tempTile.MoveTo(Key);
        _tiles[Key] = tempTile;
    }
    private void OnTilesMovementComplete()
    {

        foreach (var item in _tiles)
        {
            if (item.Value != null)
            {
                if (item.Value.IsMoving)
                {
                    return;
                }
            }
        }
        bool WereDestoyed = FindAndDestroyMatches();
        //spawn and destroy chain is over board waits for input
        if (!WereDestoyed)
        {
            foreach (var item in OnChainOver)
            {
                item.Invoke();
            }
            //if after spawn no possible moves respawn whole board
            if (!AnyMoves())
            {
                ReSpawnTiles();
                return;
            }
        }


    }
    private void OnTilesSwapComplete()
    {
        
        if(OnTileSwapedReplacement != null)
        {
            if (OnTileSwapedReplacement.Invoke(_prevSwapOne, _prevSwapTwo))
            {
                _level.MinusMoves();
                return;
            }
        }
        bool WereDestoyed = FindAndDestroyMatches();
        if (!WereDestoyed)
        {
            SwapTilesBack(_prevSwapOne, _prevSwapTwo);
            return;
        }


        _level.MinusMoves();
        
    }
    private bool AnyMoves()
    {
        Dictionary<Vector2Int, Tile> testTiles = new Dictionary<Vector2Int, Tile>(_tiles);
        foreach (var item in _tiles)
        {
            foreach (var vector in UDLR)
            {
                if (item.Value.Type > 1000 && item.Value.Type < 2000)
                {
                    return true;
                }

                SwapTiles(item.Key, item.Key + vector);
                if (FindMatches(testTiles).Count > 0)
                {
                    return true;
                }
                SwapTiles(item.Key, item.Key + vector);
            }

        }

        void SwapTiles(Vector2Int one, Vector2Int two)
        {
            if (_level.Elements.ContainsKey(one) && _level.Elements.ContainsKey(two))
            {
                Tile tempTile = testTiles[one];
                testTiles[one] = testTiles[two];
                testTiles[two] = tempTile;
            }
        }

        return false;
    }
    public bool FindAndDestroyMatches()
    {
        HashSet<Vector2Int> matches = FindMatches(_tiles);
        if (matches.Count <= 0)
        {
            
            if (IsBoardEmpty())
            {
                FillBoard();
            }
            else
            {
                Idle = true;
            }
            return false;
        }
        DestroyTiles(matches);
        return true;
    }
    private void DestroyTiles(HashSet<Vector2Int> tilesToDestroy)
    {
        //Если кому-то интересно, что тайл разрушен    
        for (int i = 0; i < OnTileDestroyed.Count; i++)
        {
            OnTileDestroyed[i].Invoke(tilesToDestroy);
        }

        foreach (var item in tilesToDestroy)
        {
            DestroyTile(item, OnDestroyMatchesComplete);
        }

    }
    private void DestroyTile(Vector2Int key,Action onDestroyComplete)
    {
        Tile tile = GetTile(key);
        if (tile == null || !tile.CanBeDestroyed())
        {
            return;
        }
        _tiles[key] = null;
        tile.OnDestroyAnimEnd = onDestroyComplete;
        tile.Destroy();
    
    }
    private void OnDestroyMatchesComplete()
    {
        //Spawn all asked tiles;
        foreach (var item in _tilesToSpawn)
        {
            SpawnTile(item.Key,Convert.ToString(item.Value),Vector2.zero);
        }
        _tilesToSpawn.Clear();
        if (IsBoardEmpty())
        {
            FillBoard();
        }

    }
    private HashSet<Vector2Int> FindMatches(Dictionary<Vector2Int,Tile> tiles)
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        for (int x = 0; x < X_MAX_SIZE; x++)
        {
            for (int y = 0; y < Y_MAX_SIZE; y++)
            {
                Tile currentTile = GetTile(tiles, new Vector2Int(x, y));
                if (currentTile != null)
                {
                    if (currentTile.CompareTiles(GetTile(tiles, new Vector2Int(x + 1, y))) && currentTile.CompareTiles(GetTile(tiles, new Vector2Int(x - 1, y))))
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x - 1, y));
                    }

                    if (currentTile.CompareTiles(GetTile(tiles, new Vector2Int(x, y + 1))) && currentTile.CompareTiles(GetTile(tiles, new Vector2Int(x, y - 1))))
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y - 1));
                    }
                }

            }
        }
        return matches;
    }
    private bool IsBoardEmpty()
    {
        foreach (var tile in _tiles.Values)
        {
            if (tile == null)
            {
                return true;
            }
        }
        return false;
    }
    private void SwapTilesBack(Vector2Int one, Vector2Int two)
    {
        Tile first = GetTile(_tiles, one);
        Tile second = GetTile(_tiles, two);
        if (first == null || second == null)
        {
            return;
        }
        _tiles[one] = second;
        _tiles[two] = first;

        first.MoveTo(two);
        second.MoveTo(one);
    }
}
