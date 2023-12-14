using UnityEngine;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;
using Unity.Mathematics;

public class Match3Game : MonoBehaviour
{
    [SerializeField] int2 size = 8;

    Grid2D<TileState> grid;

    public TileState this[int x, int y] => grid[x, y];
    public TileState this[int2 c] => grid[c];
    public int2 Size => size;

    public void StartNewGame() {
        if (grid.IsUndefined) {
            grid = new(size);
        }
        FillGrid();
    }
    private void FillGrid() {
        for(int y = 0; y < size.y; y++) {
            for(int x = 0; x < size.x; x++) {
                grid[x, y] = (TileState)Random.Range(1, 8);
            }
        }
    }
}
