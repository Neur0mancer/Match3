using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace MatchThreeEngine {
    public sealed class Board : MonoBehaviour {
        public static Board Instance { get; private set; }

        private const int TILE_VALUE = 2;

        [SerializeField] private TileTypeAsset[] tileTypes;
        [SerializeField] private Row[] rows;
        [SerializeField] private float tweenDuration;
        [SerializeField] private bool noStaringMatches;
        [SerializeField] private Transform swappingOverlay;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Space(5)]

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip matchSound;

        public event Action<TileTypeAsset, int> OnMatch;
        private readonly List<Tile> _selection = new List<Tile>();

        private bool _isSwapping;
        private bool _isMatching;
        private bool _isShuffling;
        private int _score = 0;

        public int Score {
            get => _score;
            set {
                if (_score == value) return;
                _score = value;
                scoreText.SetText($"Score: {_score}");
            }
        }

        private TileData[,] Matrix {
            get {
                var width = rows.Max(row => row.tiles.Length);
                var height = rows.Length;

                var data = new TileData[width, height];

                for (var y = 0; y < height; y++) {
                    for (var x = 0; x < width; x++) {
                        data[x, y] = GetTile(x, y).Data;
                    }
                }
                return data;
            }
        }


        private void Awake() => Instance = this;

        private void Start() {
            for (var y = 0; y < rows.Length; y++) {
                for (var x = 0; x < rows.Max(row => row.tiles.Length); x++) {
                    var tile = GetTile(x, y);

                    tile.x = x;
                    tile.y = y;
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];

                    tile.button.onClick.AddListener(() => Select(tile));
                }
            }
            if (noStaringMatches) StartCoroutine(NoStartingMatches());

            OnMatch += (type, count) => Debug.Log($"Matched {count} x {type.name}");
            scoreText.SetText($"Score: {_score}");
        }
        private void Update() {
            if(Input.GetKeyDown(KeyCode.Space)) {
                var bestMove = TileDataMatrixUtility.FindBestMove(Matrix);

                if(bestMove != null) {
                    Select(GetTile(bestMove.X1, bestMove.Y1));
                    Select(GetTile(bestMove.X2, bestMove .Y2));
                }
            }
        }

        private IEnumerator NoStartingMatches() {
            var wait = new WaitForEndOfFrame();

            while (TileDataMatrixUtility.FindBestMatch(Matrix) != null) {
                Shuffle();
                yield return wait;
            }
        }
        private Tile GetTile(int x, int y) => rows[y].tiles[x];

        private Tile[] GetTiles(IList<TileData> tileData) {
            var length = tileData.Count;
            var tiles = new Tile[length];

            for(var i = 0; i < length; i++) {
                tiles[i] = GetTile(tileData[i].X, tileData[i].Y);
            }
            return tiles;
        }
        public async void Select(Tile tile) {
            if (_isSwapping || _isMatching || _isShuffling) return;

            if (!_selection.Contains(tile)) {
                if(_selection.Count > 0) {
                    if(Math.Abs(tile.x - _selection[0].x) == 1 && Math.Abs(tile.y - _selection[0].y) == 0 ||
                            Math.Abs(tile.y - _selection[0].y) == 1 && Math.Abs(tile.x - _selection[0].x) == 0)
                        _selection.Add(tile);
                } else {
                    _selection.Add(tile);
                }
            }
            if (_selection.Count < 2) return;

            await SwapAsync(_selection[0], _selection[1]);

            if (!await TryMatchAsync()) await SwapAsync(_selection[0], _selection[1]);

            var matrix = Matrix;

            while (TileDataMatrixUtility.FindBestMove(matrix) == null || TileDataMatrixUtility.FindBestMatch(matrix) != null) {
                Shuffle();
                matrix = Matrix;
            }

            _selection.Clear();
        }
        private async Task SwapAsync(Tile tile1, Tile tile2) {
            _isSwapping = true;

            var icon1 = tile1.icon;
            var icon2 = tile2.icon;

            var icon1Transform = icon1.transform;
            var icon2Transform = icon2.transform;

            icon1Transform.SetParent(swappingOverlay);
            icon2Transform.SetParent(swappingOverlay);

            icon1Transform.SetAsLastSibling();
            icon2Transform.SetAsLastSibling();

            var sequence = DOTween.Sequence();
            sequence.Join(icon1Transform.DOMove(icon2Transform.position, tweenDuration).SetEase(Ease.OutBack))
                             .Join(icon2Transform.DOMove(icon1Transform.position, tweenDuration).SetEase(Ease.OutBack));
            await sequence.Play().AsyncWaitForCompletion();

            icon1Transform.SetParent(tile2.transform);
            icon2Transform.SetParent(tile1.transform);

            tile1.icon = icon2;
            tile2.icon = icon1;

            var tile1Item = tile1.Type;
            tile1.Type = tile2.Type;
            tile2.Type = tile1Item;           

            _isSwapping = false;

        }
        private async Task<bool> TryMatchAsync() {
            var matched = false;

            _isMatching = true;

            var match = TileDataMatrixUtility.FindBestMatch(Matrix);

            while (match != null) {
                matched = true;
                var tiles = GetTiles(match.Tiles);

                var deflateSequence = DOTween.Sequence();
                foreach (var tile in tiles) {
                    deflateSequence.Join(tile.icon.transform.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack));
                }
                 audioSource.PlayOneShot(matchSound);
                 await deflateSequence.Play().AsyncWaitForCompletion();

                 var inflateSequence = DOTween.Sequence();                    
                
                foreach (var tile in tiles) {
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
                    inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack));
                }
                await inflateSequence.Play().AsyncWaitForCompletion();
                
                OnMatch?.Invoke(Array.Find(tileTypes, tileType => tileType.id == match.TypeId), match.Tiles.Length);
                Score += TILE_VALUE * tiles.Length;
               match = TileDataMatrixUtility.FindBestMatch(Matrix);
            }
            _isMatching = false;
            return matched;
        }       
        
        private void Shuffle() {
            _isShuffling = true;

            foreach (var row in rows) {
                foreach(var tile in row.tiles) {
                    tile.Type = tileTypes[Random.Range(0, tileTypes.Length)];
                }
            }
            _isShuffling = false;
        }
    }
}
