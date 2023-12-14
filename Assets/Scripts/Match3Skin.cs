using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Match3Skin : MonoBehaviour 
 {
    [SerializeField] Tile[] tilePrefabs;
    [SerializeField] Match3Game game;

    Grid2D<Tile> tiles;
    float2 tileOffset;
    public bool IsPlaying => true;
    public bool IsBusy => false;
    public void StartNewGame() {
        game.StartNewGame();
        tileOffset = -0.5f * (float2)(game.Size - 1);
        if(tiles.IsUndefined) {
            tiles = new(game.Size);
        }
        else {
            for(int y = 0; y < tiles.SizeY; y++) {
                for(int x = 0; x < tiles.SizeX; x++) {
                    tiles[x, y].Despawn();
                    tiles[x, y] = null;
                }
            }
        }
        for (int y = 0; y < tiles.SizeY; y++) {
            for(int x = 0; x < tiles.SizeX; x++) {
                tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }
    public void DoWork() {

    }

    public bool EvaluateDrag(Vector3 start, Vector3 end) {
        return false;
    }
    private Tile SpawnTile(TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x + tileOffset.x, y + tileOffset.y));
}
