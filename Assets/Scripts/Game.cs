using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Match3Skin match3;
    private Vector3 dragStart;
    private bool isDragging;

    private void Awake() => match3.StartNewGame();

    private void Update() {
        if (match3.IsPlaying) {
            if(!match3.IsBusy) {
                HandleInput();
            }
            match3.DoWork();
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            match3.StartNewGame();
        }
    }
    private void HandleInput() {
        if(!isDragging && Input.GetMouseButtonDown(0)) {
            dragStart = Input.mousePosition;
            isDragging = true;
        } else if(isDragging && Input.GetMouseButtonDown(0)) {
            isDragging = match3.EvaluateDrag(dragStart, Input.mousePosition);
        } else {
            isDragging = false;
        }
    }
}
