
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardCombos : MonoBehaviour
{

    private Board.TilesDestroyed _onTilesDestroySpawnExtraTiles;
    private Board.TilesSwaped _onTilesSwapedProcessExtraTilesFunctions;

    private Board _board;

    private List<Vector2Int> _matches;
    private HashSet<Vector2Int> _boom;
    private List<Tile> _ignore = new List<Tile>();
    private int _animationAmmount = 0;

    public int[] ExtraTiles;
    public GameObject DinoAttackPrefab;
    public GameObject RocketAttackPrefab;
    public GameObject BrazierAttackPrefab;
    public float Speed = 5.5f;
    public float FrogTimeActive = 3f;
    public TextMeshProUGUI TextFrogPrefab;
    public Vector2Int INACESSIBLE_VECTOR2INT { get { return new Vector2Int(9999, 9999); } }
    private void Start()
    {
        _board = FindObjectOfType<Board>();
        ConnectToBoard();
    }
    public void ConnectToBoard()
    {


           _onTilesDestroySpawnExtraTiles = new Board.TilesDestroyed(SpawnExtraTiles);
        _onTilesSwapedProcessExtraTilesFunctions = new Board.TilesSwaped(ExtraTilesExplode);

        _board.OnTileDestroyed.Add(_onTilesDestroySpawnExtraTiles);
        _board.OnTileSwapedReplacement = _onTilesSwapedProcessExtraTilesFunctions;
    }
    private bool ExtraTilesExplode(Vector2Int one, Vector2Int two)
    {
        _boom = new HashSet<Vector2Int>();
        bool SomebobyExploded = false;
        if (ActivateExtraTile(one,two))
        {
            SomebobyExploded = true;
        }
        if (ActivateExtraTile(two,one))
        {
            SomebobyExploded = true;
        }

        return SomebobyExploded;
    }
    private bool ActivateExtraTile(Vector2Int tile,Vector2Int SwappedWith)
    {
        if (_ignore.Contains(_board.GetTile(tile)))
        {
            return false;
        }
        switch (_board.GetTile(tile)?.Type)
        {
            case 1001:
                //line
                RocketAttack(tile,SwappedWith);
                return true;
            case 1002:
                //Dino attack
                DinoAttack(tile);
                return true;
            case 1003:
                if(SwappedWith != INACESSIBLE_VECTOR2INT)
                BazierAttack(tile,SwappedWith);
                return true;
            case 1004:
                GoldFrog(tile);
                return true;
            default:
                break;
        }

        return false;
    }

    private void GoldFrog(Vector2Int tile)
    {
        if(GameMaster.Data.tutorial)
            FindObjectOfType<TutorialLevel>().StartGoldenFrog();
        else
            FindObjectOfType<BoardInput>().StartGoldenFrog();
        _animationAmmount++;
        this.Invoke(() =>
        {
            FindObjectOfType<BoardInput>().EndGoldenFrog();
            _board.AskToDestroy(tile);
            OnAnimationEnd();
        }, FrogTimeActive);

        var Visual = ShowVisualFrog(FrogTimeActive);
        StartCoroutine(Visual);
    }
    private IEnumerator ShowVisualFrog(float timeActive)
    {
        float StartTime = Time.time;
        TextFrogPrefab.gameObject.SetActive(true);
        while (Time.time - StartTime < timeActive)
        {
            TextFrogPrefab.text = "" + (StartTime + timeActive  - Time.time).ToString("0.00").Replace(',',':');
            yield return new WaitForSeconds(0.025f);
        }
        TextFrogPrefab.gameObject.SetActive(false);
    }
    private void BazierAttack(Vector2Int tile, Vector2Int swappedWith)
    {
        _ignore.Add(_board.GetTile(tile));
         
        Tile tileType = _board.GetTile(swappedWith);
        _animationAmmount++;
        this.Invoke(() =>
        {
            _board.AskToDestroy(tile);
        }
                       , 1.1f);
        this.Invoke(() => OnAnimationEnd(), 1.1f);
        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 25; y++)
            {
                if (_board.GetTile(new Vector2Int(x, y)) != null && _board.GetTile(new Vector2Int(x, y)).Type == tileType.Type)
                {

                    _board.AskToDestroy(new Vector2Int(x,y));
                    var temp = Instantiate(BrazierAttackPrefab);
                    temp.transform.position = new Vector3(x, y);
                    Destroy(temp, 1.1f);
                }
            }

        }
        
    }

    private void RocketAttack(Vector2Int tile,Vector2Int SwappedWith)
    {

        _ignore.Add(_board.GetTile(tile));

        var Rocketnimation = Instantiate(RocketAttackPrefab);
        Rocketnimation.transform.position = (Vector2)tile;
        Vector2Int Dir;
        if (SwappedWith == INACESSIBLE_VECTOR2INT)
        {
            int Rand = UnityEngine.Random.Range(0, 2);
          
            Rocketnimation.transform.Rotate(Vector3.forward, 90 * (1 - Rand));
            Dir = new Vector2Int((1 - Rand),Rand);
        }
        else
        {

            Dir = (SwappedWith - tile);
            Rocketnimation.transform.Rotate(Vector3.forward, 90 * Dir.x);
        }

        Destroy(Rocketnimation, 1.5f);
        _animationAmmount++;
        this.Invoke(() => OnAnimationEnd(),1.5f);


        for (int i = 0; i < 20; i++)
        {
            Vector2Int pos = tile + i * Dir;
            if (_board.GetTile(pos) != null)
            {
                if (_board.GetTile(pos).Type > 1000)
                {
                        this.Invoke( () =>
                        {
                           ActivateExtraTile(pos, INACESSIBLE_VECTOR2INT);
                            _board.AskToDestroy(pos);
                        }
                        ,i/Speed);

                    continue;
                }

                this.Invoke(() =>_board.AskToDestroy(pos), i / Speed);

            }
        }
        for (int i = 0; i > -20; i--)
        {
            Vector2Int pos = tile + i * Dir;
            if (_board.GetTile(pos) != null && _board.GetTile(pos).Type > 1000)
            {
                this.Invoke(() =>
                {
                    ActivateExtraTile(pos, INACESSIBLE_VECTOR2INT);
                    _board.AskToDestroy(pos);
                }
                ,-i / Speed);
                continue;
            }
            if (_board.GetTile(pos) != null)
            this.Invoke(() => _board.AskToDestroy(pos), -i / Speed);

        }
    }

    private void DinoAttack(Vector2Int tile)
    {
        _ignore.Add(_board.GetTile(tile));
        _boom.Add(tile);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector2Int pos = tile + new Vector2Int(x, y);
                if (_board.GetTile(pos) != null)
                {
                    if (_board.GetTile(pos).Type > 1000)
                    {
                        this.Invoke(() =>
                        {
                            ActivateExtraTile(pos, INACESSIBLE_VECTOR2INT);
                            _board.AskToDestroy(pos);
                        }
                        , 0.3f);

                        continue;
                    }
                    _boom.Add(pos);
                }
            }
        }

        var DinoAnimation = Instantiate(DinoAttackPrefab);
        DinoAnimation.transform.position = (Vector2)tile;
        Destroy(DinoAnimation, 0.6f);
          
        SoundMaster.PlayOneShot("Explosion");

        _animationAmmount++;
        this.Invoke(
             () =>
             {
                 foreach (var item in _boom)
                 {
                     _board.AskToDestroy(item);

                 }
                 OnAnimationEnd();
             }
         ,0.3f);

    }

    private void OnAnimationEnd()
    {
        _animationAmmount--;
        if (_animationAmmount > 0)
        {
            return;
        
        }


        _ignore = new List<Tile>();
        _board.FindAndDestroyMatches();
    }


    private void SpawnExtraTiles(HashSet<Vector2Int> tiles)
    {
        _matches = new List<Vector2Int>(tiles);

        foreach (var key in tiles)
        {

            foreach (var item in Board.UDLR)
            {
                var RemoveFromMatches = LetterL(key, item);
                if (RemoveFromMatches != null)
                {
                    SpawnExtraTile(key, RemoveFromMatches,ExtraTiles[3]);
                    AskToDestroyIfFoundCombo(RemoveFromMatches, key);
                    
                }
            }
        }

        foreach (var key in tiles)
        {

            foreach (var item in Board.UDLR)
            {
                var RemoveFromMatches = LetterT(key, item);
                if (RemoveFromMatches != null)
                {
                    SpawnExtraTile(key, RemoveFromMatches, ExtraTiles[2]);
                    AskToDestroyIfFoundCombo(RemoveFromMatches, key);
                }

            }
        }
        

        foreach (var key in tiles)
        {
            var RemoveFromMatches = FiveInRow(key, Vector2Int.right);
            if (RemoveFromMatches != null)
            {
                SpawnExtraTile(key, RemoveFromMatches, ExtraTiles[1]);
                AskToDestroyIfFoundCombo(RemoveFromMatches, key);

            }
            RemoveFromMatches = FiveInRow(key, Vector2Int.down);
            if (RemoveFromMatches != null)
            {
                SpawnExtraTile(key, RemoveFromMatches, ExtraTiles[1]);
                AskToDestroyIfFoundCombo(RemoveFromMatches, key);

            }
        }
        foreach (var key in tiles)
        {
            var RemoveFromMatches = FourInRow(key, Vector2Int.right);
            if (RemoveFromMatches != null)
            {
                SpawnExtraTile(key, RemoveFromMatches, ExtraTiles[0]);
                AskToDestroyIfFoundCombo(RemoveFromMatches, key);

            }

            RemoveFromMatches = FourInRow(key, Vector2Int.down);
            if (RemoveFromMatches != null)
            {
                SpawnExtraTile(key, RemoveFromMatches, ExtraTiles[0]);
                AskToDestroyIfFoundCombo(RemoveFromMatches, key);
            }

        }
    }

    private void SpawnExtraTile(Vector2Int key, List<Vector2Int> removeFromMatches, int extratile)
    {
        var swapped = _board.GetLastSwapped();
        var TileToCompare = _board.GetTile(key);
        if (ContainsInSetAndEqual(swapped.Item1, TileToCompare, removeFromMatches))
        {
            _board.AskToSpawn(swapped.Item1, extratile);
            return;
        }
        if (ContainsInSetAndEqual(swapped.Item2, TileToCompare, removeFromMatches))
        {
            _board.AskToSpawn(swapped.Item2, extratile);
            return;
        }
        _board.AskToSpawn(key, extratile);
    }

    private List<Vector2Int> LetterL(Vector2Int start, Vector2Int direction)
    {
        List<Vector2Int> RemoveFromMatches = new List<Vector2Int>();
        Tile tile = _board.GetTile(start);
        for (int i = 1; i < 3; i++)
        {
            if (!ContainsInMatchAndEqual(start + direction * i, tile))
            {
                return null;
            }
            RemoveFromMatches.Add(start + direction * i);
        }

        Vector2Int TurnedDirection = new Vector2Int(-direction.y, direction.x);
        if (!ContainsInMatchAndEqual(start + direction * 2 + TurnedDirection, tile))
        {
            return null;
        }
        if (!ContainsInMatchAndEqual(start + direction * 2 + TurnedDirection * 2, tile))
        {
            return null;
        }

        RemoveFromMatches.Add(start + direction * 2 + TurnedDirection * 2);
        RemoveFromMatches.Add(start + direction * 2 + TurnedDirection);
        foreach (var item in RemoveFromMatches)
        {
            _matches.Remove(item);
        }
        return RemoveFromMatches;

    }
    private List<Vector2Int> LetterT(Vector2Int start, Vector2Int direction)
    {
        List<Vector2Int> RemoveFromMatches = new List<Vector2Int>();
        Tile tile = _board.GetTile(start);
        for (int i = 1; i < 3; i++)
        {
            if (!ContainsInMatchAndEqual(start + direction * i, tile))
            {
                return null;
            }
            RemoveFromMatches.Add(start + direction * i);
        }

        Vector2Int TurnedDirection = new Vector2Int(direction.y, direction.x);
        if (!ContainsInMatchAndEqual(start + direction * 2 + TurnedDirection, tile))
        {
            return null;
        }
        if (!ContainsInMatchAndEqual(start + direction * 2 - TurnedDirection, tile))
        {
            return null;
        }

        RemoveFromMatches.Add(start + direction * 2 - TurnedDirection);
        RemoveFromMatches.Add(start + direction * 2 + TurnedDirection);
        foreach (var item in RemoveFromMatches)
        {
            _matches.Remove(item);
        }
        return RemoveFromMatches;

    }
    private List<Vector2Int> FiveInRow(Vector2Int start, Vector2Int direction)
    {
        List<Vector2Int> RemoveFromMatches = new List<Vector2Int>();
        Tile tile = _board.GetTile(start);
        for (int i = 1; i < 5; i++)
        {
            if (!ContainsInMatchAndEqual(start + direction * i, tile))
            {
                return null;
            }
            RemoveFromMatches.Add(start + direction * i);
        }

        foreach (var item in RemoveFromMatches)
        {
            _matches.Remove(item);
        }
        return RemoveFromMatches;
    }
    private List<Vector2Int> FourInRow(Vector2Int start,Vector2Int direction)
    {
        List<Vector2Int> RemoveFromMatches = new List<Vector2Int>();
        Tile tile = _board.GetTile(start);

        for (int i = 1; i < 4; i++)
        {
            if (!ContainsInMatchAndEqual(start + direction * i, tile))
            {
                return null;
            }
            RemoveFromMatches.Add(start + direction * i);
        }

        foreach(var item in RemoveFromMatches)
        {
            _matches.Remove(item);
        }
        return RemoveFromMatches;
    }

    
    private bool ContainsInMatchAndEqual(Vector2Int pos,Tile tile)
    {
        return ContainsInSetAndEqual(pos, tile, _matches);
    }

    private bool ContainsInSetAndEqual(Vector2Int pos, Tile tile,List<Vector2Int> set)
    {
        if (tile == null)
        {
            return false;
        }
        if (set.Contains(pos))
        {
            if (!tile.CompareTiles(_board.GetTile(pos)))
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private void AskToDestroyIfFoundCombo(List<Vector2Int> tiles,Vector2 moveTo)
    {
        foreach (var item in tiles)
        {
            _board.AskToDestroy(item, null);
        }
    }
}

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}