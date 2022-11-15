using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static int GridSize = 4;

    private readonly Transform[,] _tilePositions = new Transform[GridSize, GridSize];

    private readonly Tile[,] _tiles = new Tile[GridSize, GridSize];

    [SerializeField] private Tile tilePrefab;

    private bool _isAnimating;

    [SerializeField] private TileSettings tileSettings;
    private Stack<GameState> _gameStates = new Stack<GameState>();
    // Start is called before the first frame update
    void Start()
    {
        GetTilePositions();
        TrySpawnTile();
        TrySpawnTile();
        UpdateTilePositions(true);
    }

    private int _lastXImput;
    private int _lastYImput;

    // Update is called once per frame
    void Update()
    {
        var xInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        var yInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (_lastXImput == 0 && _lastYImput == 0)
        {
            if (!_isAnimating)
                TryMove(xInput, yInput);
        }

        _lastXImput = xInput;
        _lastYImput = yInput;


    }

    public void RestartGame()
    {
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

        


    private void GetTilePositions()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        int x = 0;
        int y = 0;
        foreach (Transform transform in this.transform)
        {
            _tilePositions[x, y] = transform;
            x++;
            if (x >= GridSize)
            {
                x = 0;
                y++;
            }
        }
    }

    private bool TrySpawnTile()
    {
        List<Vector2Int> availableSpots = new List<Vector2Int>();

        for (int x = 0; x < GridSize; x++)
            for (int y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null)
                    availableSpots.Add(new Vector2Int(x, y));
            }

        if (!availableSpots.Any())
            return false;

        int randomIndex = Random.Range(0, availableSpots.Count);
        Vector2Int spot = availableSpots[randomIndex];

        var tile = Instantiate(tilePrefab, transform.parent);
        tile.SetValue(GetRandomValue());
        _tiles[spot.x, spot.y] = tile;

        return true;
    }

    private int GetRandomValue()
    {
        var rand = Random.Range(0f, 1f);
        if (rand <= .8f)
            return 2;
        else
            return 4;
    }

    private void UpdateTilePositions(bool instant)
    {
        if (!instant)
        {
            _isAnimating = true;
            StartCoroutine(WaitFoTileAnimation());
        }
        for (int x = 0; x < GridSize; x++)
            for (int y = 0; y < GridSize; y++)
                if (_tiles[x, y] != null)
                    _tiles[x, y].SetPosition(_tilePositions[x, y].position, instant);
    }
    private IEnumerator WaitFoTileAnimation()
    {
        yield return new WaitForSeconds(tileSettings.animationTime);
        if (!TrySpawnTile())
        {
            Debug.LogError("UNABLE TO SPAWN TILE");
        }
        UpdateTilePositions(true);
        _isAnimating = false;
    }

    private bool _tilesUpdated;

    private void TryMove(int x, int y)
    {
        if (x == 0 && y == 0)
            return;

        if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
        {
            Debug.LogWarning($"Invalid move {x},{y}");
            return;
        }

        _tilesUpdated = false;
        int[,] preMoveTileValues = GetCurrentTileValues();

        if (x == 0)
        {
            if (y > 0)
                TryMoveUp();
            else
                TryMoveDown();
        }
        else
        {
            if (x < 0)
                TryMoveLeft();
            else
                TryMoveRight();
        }
        if (_tilesUpdated)
        {
            _gameStates.Push(new GameState() { tileValues = preMoveTileValues });
            UpdateTilePositions(false);
        }
        }

    private int[,] GetCurrentTileValues()
    {
        int[,] result = new int[GridSize, GridSize];
        for (int x = 0; x < GridSize; x++)
            for (int y = 0; y < GridSize; y++)
                if (_tiles[x, y] != null)
                    result[x, y] = _tiles[x, y].GetValue();

        return result;
    }

    public void LoadLastTileValues()
    {
        if(_isAnimating)
                return;

        if (!_gameStates.Any())
            return;

        GameState previosGameState = _gameStates.Pop();

        foreach (Tile t in _tiles)
            if (t != null)
                Destroy(t.gameObject);

        for(int x = 0; x < GridSize; x++ )
            for (int y = 0; y < GridSize; y++)
            {
                _tiles[x, y] = null;
                if (previosGameState.tileValues[x, y] == 0)
                    continue;

                Tile tile = Instantiate(tilePrefab, transform.parent);
                tile.SetValue(previosGameState.tileValues[x, y]);
                _tiles[x, y] = tile;
            }

        UpdateTilePositions(true);
    }    

    private bool TileExistsBetween(int x, int y, int x2, int y2)
    {
        if (x == x2)
            return TileExistsBetweenVertical(x, y, y2);
        else if (y == y2)
            return TileExistsBetweenHorizontal(x, x2, y);

        Debug.LogError($"BETWEEN CHECK - INVALID PARAMETERS ({x}, {y}) ({x2},{y2})");
        return true;
    }

    private bool TileExistsBetweenHorizontal(int x, int x2, int y)
    {
        int minX = Mathf.Min(x, x2);
        int maxX= Mathf.Max(x, x2);
        for (int xIndex = minX + 1; xIndex < maxX; xIndex++)
            if (_tiles[xIndex, y] != null)
                return true;
        return false;
    }
    private bool TileExistsBetweenVertical(int x, int y, int y2)
    {
        int minY = Mathf.Min(y, y2);
        int maxY = Mathf.Max(y, y2);
        for (int yIndex = minY + 1; yIndex < maxY; yIndex++)
            if (_tiles[x, yIndex] != null)
                return true;
        return false;        
    }

    private void TryMoveRight()
    {
        for (int y = 0; y < GridSize; y++)
            for (int x = GridSize - 1; x >= 0; x--)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                            continue;

                        if (_tiles[x2, y].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
    }

    private void TryMoveLeft()
    {
        for (int y = 0; y < GridSize; y++)
            for (int x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = 0; x2 < x; x2++)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                            continue;

                        if (_tiles[x2, y].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
    }

    private void TryMoveDown()
    {
        for (int x = 0; x < GridSize; x++)
            for (int y = GridSize - 1; y >= 0; y--)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = GridSize - 1; y2 > y; y2--)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                            continue;

                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }

            }
    }

    private void TryMoveUp()
    {
        for (int x = 0; x < GridSize; x++)
            for (int y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = 0; y2 < y; y2++)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                            continue;

                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tiles[x, y] = null;
                            _tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
    }
}
