using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    private int _score;

    public int ScoreValue {
        get => _score; 
        set {
            if (_score == value) return;
            _score = value;
            scoreText.SetText($"Score: {_score}"); 
        }
    }
    private void Awake() {
        Instance = this; }
    
}
