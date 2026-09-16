using UnityEngine;

public class InfinateBackground : MonoBehaviour
{
    private PlayerManager _playerManager;

    [Header("Background")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private float backgroundScale = 1f;

    [Header("Grid")]
    [SerializeField] private int tilesFromCenter = 1;

    [Header("Player")]
    [SerializeField] private Vector2 playerPosition;

    private float tileWidth;
    private float tileHeight;

    private int currentGridX;
    private int currentGridY;

    private GameObject[,] tiles;

    private void Awake()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
    }

    private void Start()
    {
        tileWidth = backgroundSprite.bounds.size.x * backgroundScale;
        tileHeight = backgroundSprite.bounds.size.y * backgroundScale;

        int gridSize = tilesFromCenter * 2 + 1;

        tiles = new GameObject[gridSize, gridSize];

        CreateInitialGrid();

        UpdatePlayerGrid();
    }

    private void Update()
    {
        UpdatePlayerGrid();
    }

    private void CreateInitialGrid()
    {
        int gridSize = tilesFromCenter * 2 + 1;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject tile = new GameObject("Background Tile");

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = backgroundSprite;
                renderer.sortingOrder = -2;

                tile.transform.localScale = Vector3.one * backgroundScale;

                tiles[x, y] = tile;
            }
        }
    }

    private void UpdatePlayerGrid()
    {
        int newGridX = Mathf.FloorToInt(_playerManager.bodyRB.transform.position.x / tileWidth);
        int newGridY = Mathf.FloorToInt(_playerManager.bodyRB.transform.position.y / tileHeight);

        if (newGridX == currentGridX && newGridY == currentGridY)
            return;

        currentGridX = newGridX;
        currentGridY = newGridY;

        UpdateTiles();
    }

    private void UpdateTiles()
    {
        int gridSize = tilesFromCenter * 2 + 1;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int offsetX = x - tilesFromCenter;
                int offsetY = y - tilesFromCenter;

                int gridX = currentGridX + offsetX;
                int gridY = currentGridY + offsetY;

                Vector3 position = new Vector3(
                    gridX * tileWidth,
                    gridY * tileHeight,
                    0f
                );

                tiles[x, y].transform.position = position;
            }
        }
    }

    public void SetPlayerPosition(Vector2 position)
    {
        playerPosition = position;
    }
}
