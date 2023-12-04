using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Skin : MonoBehaviour 
 {
    [SerializeField] Tile[] tilePrefabs;
    public bool IsPlaying => true;
    public bool IsBusy => false;
    public void StartNewGame() {

    }
    public void DoWork() {

    }

    public bool EvaluateDrag(Vector3 start, Vector3 end) {
        return false;
    }

}
