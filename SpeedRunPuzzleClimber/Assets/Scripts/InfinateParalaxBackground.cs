using UnityEngine;

public class InfiniteParallaxBackground : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Material backgroundMaterial;

    [SerializeField] private float backgroundScale = 1f;
    [SerializeField] private int sortOrder = -2;

    [Header("Grid")]
    [SerializeField] private int tilesFromCenter = 1;

    [Header("Star Movement")]
    [SerializeField] private float starFollowAmount = 0.98f;
    [SerializeField] private float starFollowSmoothSpeed = 2f;

    private Camera _camera;

    private float tileWidth;
    private float tileHeight;

    private Vector2 previousCameraPosition;
    private Vector2 backgroundOffset;
    private Vector2 targetBackgroundOffset;

    private int currentGridX;
    private int currentGridY;

    private GameObject[,] tiles;
    private Vector3[,] tileBasePositions;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        tileWidth = backgroundSprite.bounds.size.x * backgroundScale;
        tileHeight = backgroundSprite.bounds.size.y * backgroundScale;

        int gridSize = tilesFromCenter * 2 + 1;

        tiles = new GameObject[gridSize, gridSize];
        tileBasePositions = new Vector3[gridSize, gridSize];

        previousCameraPosition = _camera.transform.position;

        CreateInitialGrid();

        UpdateCameraGrid(true);
    }

    private void LateUpdate()
    {
        UpdateBackgroundMovement();
        UpdateCameraGrid(false);
    }

    private void UpdateBackgroundMovement()
    {
        Vector2 cameraPosition = _camera.transform.position;

        Vector2 cameraMovement =
            cameraPosition - previousCameraPosition;

        targetBackgroundOffset +=
            cameraMovement * starFollowAmount;

        if (starFollowSmoothSpeed <= 0f)
        {
            backgroundOffset = targetBackgroundOffset;
        }
        else
        {
            backgroundOffset = Vector2.Lerp(
                backgroundOffset,
                targetBackgroundOffset,
                starFollowSmoothSpeed * Time.deltaTime
            );
        }

        previousCameraPosition = cameraPosition;

        ApplyTilePositions();
    }

    private void UpdateCameraGrid(bool forceUpdate)
    {
        Vector2 cameraPosition = _camera.transform.position;

        // Convert the camera position into the background's
        // coordinate space by removing the background movement.
        Vector2 backgroundCameraPosition =
            cameraPosition - backgroundOffset;

        int newGridX = Mathf.FloorToInt(
            backgroundCameraPosition.x / tileWidth
        );

        int newGridY = Mathf.FloorToInt(
            backgroundCameraPosition.y / tileHeight
        );

        if (!forceUpdate &&
            newGridX == currentGridX &&
            newGridY == currentGridY)
        {
            return;
        }

        currentGridX = newGridX;
        currentGridY = newGridY;

        UpdateTiles();
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

                if (backgroundMaterial != null) renderer.material = backgroundMaterial;

                renderer.sprite = backgroundSprite;
                renderer.sortingOrder = sortOrder;

                tile.transform.localScale =
                    Vector3.one * backgroundScale;

                tiles[x, y] = tile;
            }
        }
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

                Vector3 basePosition = new Vector3(
                    gridX * tileWidth,
                    gridY * tileHeight,
                    0f
                );

                tileBasePositions[x, y] = basePosition;
            }
        }

        ApplyTilePositions();
    }

    private void ApplyTilePositions()
    {
        int gridSize = tilesFromCenter * 2 + 1;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                tiles[x, y].transform.position =
                    tileBasePositions[x, y] +
                    (Vector3)backgroundOffset;
            }
        }
    }
}