using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    public class BoardController : MonoBehaviour
    {
        // Inspector
        public AvatarController[] AvatarPrefabs;
        public GameObject TilePrefab;
        public GameObject SummonTilePrefab;
        public GameObject TileWall1Prefab;
        public GameObject TileWall2Prefab;
        public GameObject TileWall3Prefab;

        public GameObject PathHighlightOkPrefab;
        public GameObject PathHighlightNotOkPrefab;

        public Transform PathHighlightContainer;
        public Transform TileContainer;
        public Transform AvatarContainer;

        // Pools

        private List<GameObject> _highlightedOkPathPool = new List<GameObject>();
        private List<GameObject> _highlightedNotOkPathPool = new List<GameObject>();

        // Public
        private Tile[] _tiles;

        // Private
        private Vector2 _tileSize = new Vector2 (2f, 1.7320508f);
        private int _tilesW = 20;
        private int _tilesH = 12;

        private GameSceneController _scene;


        private Dictionary<CardSO, GameObject> _avatarPrefabsByCard = new Dictionary<CardSO, GameObject>();


        public void Init(GameSceneController scene)
        {
            _scene = scene;

            InitializeAvatarPrefabs();
        }

        private void Awake()
        {
            InitializeTiles();
        }
        // Start is called before the first frame update
        void Start()
        {
            PlaceTiles();
            //SpawnAvatar(0, 0);
            //MoveAvatar(testAvatar, new Vector2Int(3, 5));

            // Testing positions
            //for (int y = 0; y < _tilesH; y++)
            //{
            //    for (int x = 0; x < _tilesW; x++)
            //    {
            //        SpawnAvatar(x, y);
            //        MoveAvatar(testAvatar, new Vector2Int(1, 1));
            //    }
            //}
        }

        private void InitializeTiles()
        {
            int w = _tilesW;
            int h = _tilesH;

            _tiles = new Tile[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    _tiles[y * w + x] = new Tile()
                    {
                        PathScore = 1f,
                        Position = new Vector2Int(x, y),
                    };
                }
            }


            /*LoadStringMap(
            "11111111111111111111" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "1------------------1" +
            "11111111111111111111");*/

            int level = GameManager.Instance.Level;

            if (level == 0)
            {

                LoadStringMap(
                        "11111111111111111211" +
                        "1---------------2321" +
                        "1------------------1" +
                        "1----------o-----111" +
                        "1----s--------121111" +
                        "1-------------232111" +
                        "1-------------121111" +
                        "1----s-----------111" +
                        "1-------p----------1" +
                        "1------------------1" +
                        "1----2221----------1" +
                        "11111111111111111111");

                
            }
            else if (level == 1)
            {

                LoadStringMap(
                        "11111111111111111211" +
                        "1---------------2321" +
                        "1--o---------------1" +
                        "1-----1----o-----111" +
                        "1-----1-------121111" +
                        "1--s-----1----232111" +
                        "1--------1----121111" +
                        "1--1-------------111" +
                        "1--1----p----------1" +
                        "1------------------1" +
                        "1----2221----------1" +
                        "11111111111111111111");

            }
            else if (level == 2)
            {
                LoadStringMap(
                        "--o----o----o-----o-" +
                        "---------1---------1" +
                        "---11--s-2---11--s-2" +
                        "---------2---------2" +
                        "-------------------2" +
                        "---s--11-2---s--11-2" +
                        "---------2---------2" +
                        "---------1--111----1" +
                        "------------3221---1" +
                        "---------1----11---1" +
                        "-------p-1-------p-1" +
                        "-p------11-p------11");

            }
            else if (level == 3)
            {
               LoadStringMap(
                "11111111111111111111" +
                "1------------------1" +
                "1----o-------p-----1" +
                "1------------------1" +
                "1--------1---------1" +
                "1-------121--------1" +
                "1-o----12321----p--1" +
                "1-------121--------1" +
                "1--------1---------1" +
                "1------------------1" +
                "1--------z---------1" +
                "11111111111111111111");

            }
            else if (level == 4)
            {
                LoadStringMap(
                        "--o------3--------o-" +
                        "---------2----------" +
                        "---111---2---11--s--" +
                        "--11211--2----------" +
                        "---111---s----------" +
                        "-p----11-----1--11--" +
                        "-------------1------" +
                        "---------s--111--o--" +
                        "---------1--3221----" +
                        "--------11----11----" +
                        "---------2-------p--" +
                        "-p------111---------");

            }
        }

        /// <summary>
        /// The map must have 100 characters for the 10x10 area.
        /// - = blank

        /// </summary>
        /// <param name="map"></param>
        private void LoadStringMap(string map)
        {
            int cNum = 0;

            foreach (char c in map)
            {
                //_tiles[position.y * _tilesW + position.x];
                Vector2Int pos = new Vector2Int(cNum % _tilesW, _tilesH - Mathf.FloorToInt(cNum / _tilesW) - 1);
                //Vector2Int pos = new Vector2Int(cNum % _tilesW, cNum / _tilesH);

                Tile tile = new Tile()
                {
                    PathScore = 1f,
                    Position = new Vector2Int(pos.x, pos.y),
                };

                if (c == 'p')
                {
                    tile.IsPlayerOwned = true;
                    tile.IsSummonTile = true;
                }
                else if (c == 'o')
                {
                    tile.IsOpponentOwned = true;
                    tile.IsSummonTile = true;
                }
                else if (c == 's')
                {
                    tile.IsSummonTile = true;
                }
                else if (c == '1')
                {
                    tile.PathScore = 0;
                    tile.Height = 1; // indicates an elevated basalt column
                }
                else if (c == '2')
                {
                    tile.PathScore = 0;
                    tile.Height = 2; // indicates an elevated basalt column
                }
                else if (c == '3')
                {
                    tile.PathScore = 0;
                    tile.Height = 3; // indicates an elevated basalt column
                }
                else
                {

                }

                //_tiles[cNum] = tile;
                _tiles[pos.y * _tilesW + pos.x] = tile;
                cNum++;
            }
        }

        private void PlaceTile(Tile tile)
        {
            int x = tile.Position.x;
            int y = tile.Position.y;

            if (tile.IsSummonTile) // Determine which prefab is needed
            {
                GameObject prefab = SummonTilePrefab;
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(TileContainer, false);
                go.transform.position = Grid2World(x, y);
                TileController controller = go.GetComponent<TileController>();
                controller.Tile = tile;
                controller.Init(_scene);

                SummonTileController summonTileController = go.GetComponent<SummonTileController>();
                summonTileController.OwnershipChanged += SummonTileOwnershipChanged;
            }
            else if (tile.Height > 0)
            {
                GameObject prefab = tile.Height == 1? TileWall1Prefab : tile.Height == 2? TileWall2Prefab : TileWall3Prefab;
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(TileContainer, false);
                go.transform.position = Grid2World(x, y);
                TileController controller = go.GetComponent<TileController>();
                controller.Tile = tile;
                controller.Init(_scene);
            }
            else
            {
                GameObject prefab = TilePrefab;
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(TileContainer, false);
                go.transform.position = Grid2World(x, y);
                TileController controller = go.GetComponent<TileController>();
                controller.Tile = tile;
                controller.Init(_scene);
            }


        }


        public void EndTurn()
        {
            AvatarController[] avatars = GetAvatars();
            foreach (AvatarController avatar in avatars)
            {
                avatar.EndTurn();
            }
        }


        // Currently, there's only 1 prefab for all cards
        private void InitializeAvatarPrefabs()
        {
            _scene.Deck.InitializeLists();

            _avatarPrefabsByCard.Clear();
            //foreach (var avatarPrefab in AvatarPrefabs)
            //{
             //   _avatarPrefabsByCard.Add(avatarPrefab.Card, avatarPrefab.gameObject);
            //}

            foreach (KeyValuePair<CardSO, CardController> kv in _scene.Deck.CardPrefabsByType)
            {
                _avatarPrefabsByCard.Add(kv.Key, AvatarPrefabs[0].gameObject);
            }
        }

        public bool TileEmpty(Vector2Int position)
        {
            AvatarController[] avatars = GetAvatars();
            foreach (AvatarController avatar in avatars)
            {
                Vector2Int avatarpos = World2Grid(avatar.transform.position);
                if (position == avatarpos)
                {
                    return false;
                }
            }
            return true;
        }
        public Tile GetTile(Vector2Int position)
        {
            return _tiles[position.y * _tilesW + position.x];
        }

        public void SpawnAvatar(Vector2Int position, CardSO card, bool isPlayer)
        {
            GameObject avatarPrefab = _avatarPrefabsByCard[card];

            GameObject avatar = Instantiate(avatarPrefab);
            avatar.transform.SetParent(AvatarContainer, false);
            Vector3 desiredPosition = Grid2World(position.x, position.y);
            avatar.transform.position = desiredPosition - new Vector3(0, 2f, 0); // sink it, so the animation has it rise

            AvatarController avatarController = avatar.GetComponent<AvatarController>();
            avatarController.Init(_scene);
            avatarController.IsPlayer = isPlayer;
            avatarController.Card = card;

            StartCoroutine(SpawnAvatarCoroutine(avatar, desiredPosition));
        }


        IEnumerator SpawnAvatarCoroutine(GameObject avatar, Vector3 desiredPosition)
        {
            float moveSpeed = .5f; // seconds per unit distance
            float progress = 0;

            Vector3 initialPosition = avatar.transform.localPosition;
           // Quaternion initialRotation = avatar.transform.localRotation;

            //Vector3 desiredPosition = Vector3.zero;
           // Quaternion desiredRotation = Quaternion.identity;

            float distance = (desiredPosition - initialPosition).magnitude;

            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed / distance;
                progress += frameProgress;

                avatar.transform.localPosition = Vector3.Slerp(initialPosition, desiredPosition, progress);
                //avatar.transform.localRotation = Quaternion.Slerp(initialRotation, desiredRotation, progress);
            }
        }

        public AvatarController GetAvatar(Vector3 worldPosition)
        {
            Ray ray = new Ray(worldPosition + transform.up * 5f, -transform.up); ;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Avatar")))
            {
                AvatarController avatar = hit.collider.gameObject.GetComponent<AvatarController>();
                return avatar;
            }
            return null;
        }

        public AvatarController GetPlayerAvatar(Vector3 worldPosition)
        {
            Ray ray = new Ray(worldPosition + transform.up * 5f, -transform.up); ;
            
            RaycastHit[] hits = Physics.RaycastAll(ray, 10f, LayerMask.GetMask("Avatar"));
            foreach (RaycastHit hit in hits)
            {

                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;

                    if ((collider.ColliderType & ColliderType.Avatar) > 0) // checks for all types of tiles
                    {
                        AvatarController avatar = collider.gameObject.GetComponent<AvatarController>();
                        if (avatar != null)
                        {
                            if (avatar.IsPlayer)
                            {
                                return avatar;
                            }
                        }
                    }
                }


                
            }
            return null;
        }
        public AvatarController GetOpponentAvatar(Vector3 worldPosition)
        {
            Ray ray = new Ray(worldPosition + transform.up * 5f, -transform.up); ;

            RaycastHit[] hits = Physics.RaycastAll(ray, 10f, LayerMask.GetMask("Avatar"));
            foreach (RaycastHit hit in hits)
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;

                    if ((collider.ColliderType & ColliderType.Avatar) > 0) // checks for all types of tiles
                    {
                        AvatarController avatar = collider.gameObject.GetComponent<AvatarController>();
                        if (avatar != null)
                        {
                            if (!avatar.IsPlayer)
                            {
                                return avatar;
                            }
                        }
                    }
                }
            }
            return null;
        }


        public void KillAvatar(AvatarController avatar)
        {
            StartCoroutine(KillAvatarCoroutine(avatar));
        }
        IEnumerator KillAvatarCoroutine(AvatarController avatar)
        {
            yield return new WaitForEndOfFrame();
            Destroy(avatar.gameObject);

            Debug.Log("Avatar is dead. Do something.");
        }

        public void ClearHighlightPath()
        {
            for (int i = PathHighlightContainer.childCount - 1; i >= 0; i--)
            {
                PathHighlightContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
        public bool HighlightPath(AvatarController avatar, Vector2Int position)
        {
            Vector2Int currentPosition = World2Grid(avatar.transform.position);
            Vector3[] path = GetPath(currentPosition, position);

            if (path == null)
            {
                // Do nothing
                return false;
            }

            if (path.Length > avatar.RemainingDistanceThisTurn)
            {
                // Highlight red
                HighlightPath(path, PathHighlightNotOkPrefab, _highlightedNotOkPathPool);
                return false;
            }

            // Highlight ok
            HighlightPath(path, PathHighlightOkPrefab, _highlightedOkPathPool);
            return true;
        }

        private void HighlightPath(Vector3[] path, GameObject prefab, List<GameObject> highlightedPathPool)
        {
            ClearHighlightPath();
            //_highlightedPathPool
            int count = highlightedPathPool.Count;
            for (int i = 0; i < path.Length; i++)
            {
                Vector3 pos = path[i];

                if (i >= count)
                {
                    GameObject go = Instantiate(prefab);
                    go.transform.SetParent(PathHighlightContainer, false);
                    go.transform.localPosition = pos;
                    highlightedPathPool.Add(go);
                }
                else
                {
                    GameObject go = highlightedPathPool[i];
                    go.SetActive(true);
                    go.transform.localPosition = pos;
                }


                // Kinda hacky. Don't display the current spot
                /*if (i == 0)
                {
                    GameObject go = _highlightedPathPool[i];
                    go.SetActive(false);
                }*/
            }
        }

        public bool MoveAvatar(AvatarController avatar, Vector2Int position)
        {
            Vector2Int currentPosition = World2Grid(avatar.transform.position);
            Debug.Log("Moving from : " + currentPosition + " to " + position);

            Vector3[] path = GetPath(currentPosition, position);

            if (path == null)
            {
                Debug.Log("Avatar cannot move there.");
                return false;
            }

            if (path.Length > avatar.RemainingDistanceThisTurn)
            {
                Debug.Log("Avatar cannot move that far.");
                return false;
            }

            _scene.SelectionWindow.AddMessage(avatar.Card.Name + " moved to " + (position.x+1) + " ," + (position.y+1));

            avatar.SetPath(path);

            //DebugPath(path);

            return true;
        }


        public AvatarController[] GetAvatars()
        {
            return AvatarContainer.GetComponentsInChildren<AvatarController>();
        }


        

        private void PlaceTiles()
        {
            int w = _tilesW;
            int h = _tilesH;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (_tiles[y * w + x] != null)
                    {
                        Tile tile = _tiles[y * w + x];
                        PlaceTile(tile);
                    }
                }
            }
        }




        public SummonTileController[] GetSummonTiles()
        {
            return TileContainer.GetComponentsInChildren<SummonTileController>();
        }

        private void SummonTileOwnershipChanged(SummonTileController controller)
        {
            SummonTileController[] tiles = GetSummonTiles();

            bool playerOwnsAll = true;
            bool opponentOwnsAll = true;

            // Check if they've all changed
            foreach (SummonTileController tile in tiles)
            {
                if (tile.TileController.Tile.IsPlayerOwned)
                {
                    opponentOwnsAll = false;
                }
                else
                {
                    playerOwnsAll = false;
                }
            }

            if (playerOwnsAll)
            {
                _scene.WinCondition();
            }
            else if (opponentOwnsAll)
            {
                _scene.LoseCondition();
            }
        }

        private Vector3 Grid2World(int x, int y)
        {
            float zOffset = 0f;
            if (x % 2 == 1)
            {
                zOffset = _tileSize.y / 2f;
            }

            return new Vector3(x* _tileSize.x * .75f, 0, y * _tileSize.y + zOffset);
        }

        // This isn't precise. It assumes the position is at the center of the tile
        public Vector2Int World2Grid(Vector3 worldPosition)
        {
            /*float xOffset = _tileSize.x / 2f;
            float yOffset = _tileSize.y / 2f;

            int xPos = Mathf.FloorToInt(worldPosition.x / _tileSize.x / .75f + xOffset);
            int yPos = Mathf.FloorToInt(worldPosition.z / _tileSize.y + yOffset);

            return new Vector2Int(xPos, yPos);*/


            Ray ray = new Ray(worldPosition + transform.up * 5f, -transform.up); ;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Board")))
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;

                    if ((collider.ColliderType & ColliderType.Tile) > 0) // checks for all types of tiles
                    {
                        return collider.gameObject.GetComponent<TileController>().Tile.Position;
                    }
                }

            }


            // Fallback to the old calculation. It's prone to error
            Debug.LogError("Couldn't find a tile using raycast. Falling back to calculation.");
            int xPos = Mathf.FloorToInt(worldPosition.x / _tileSize.x / .75f);
            int yPos = Mathf.FloorToInt(worldPosition.z / _tileSize.y);

            return new Vector2Int(xPos, yPos);
        }

        #region Pathfinding

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>The path.</returns>
        public Vector3[] GetPath(Vector2Int fromPosition, Vector2Int destination)
        {
            List<Vector3> path = new List<Vector3>();

            int fromCell = fromPosition.y * _tilesW + fromPosition.x;
            int toCell = destination.y * _tilesW + destination.x;

            SimplePriorityQueue<int> frontier = new SimplePriorityQueue<int>();
            Dictionary<int, float> testedCells = new Dictionary<int, float>();
            Dictionary<int, int> parentCells = new Dictionary<int, int>();

            int currentCell = -1;
            //IVector3 currentCellPosition;

            int maxTiles = _tiles.Length;

            // address out of bounds issues
            if (fromCell < 0 || fromCell >= maxTiles || toCell < 0 || toCell >= maxTiles)
            {
                return null;
            }


            // test whether or not both position are walkable
            if (_tiles[fromCell] != null && _tiles[fromCell].PathScore > 0
                && _tiles[toCell] != null && _tiles[toCell].PathScore > 0)
            {

                frontier.Enqueue(fromCell, 0);
                testedCells.Add(fromCell, 0);

                while (frontier.Count > 0)
                {

                    currentCell = frontier.Dequeue();

                    Vector2Int currentCellPosition = GridPosFromIndex(currentCell);

                    if (currentCell == toCell)
                    {
                        // found the destination
                        break;
                    }

                    //Debug.Log(currentCell + " -> " + currentCellPosition);

                    Vector2Int[] neighbouringCellOffsets = (currentCellPosition.x % 2 == 0)? neighbouringCellOffsetsEvenX : neighbouringCellOffsetsOddX;

                    // Get neighbours
                    for (int i = 0; i < neighbouringCellOffsets.Length; i++)
                    {
                        int neighbouringCell = currentCell + (neighbouringCellOffsets[i].y * _tilesW + neighbouringCellOffsets[i].x);


                        Vector2Int neighbouringCellPosition = new Vector2Int(
                            (currentCell % _tilesW) + neighbouringCellOffsets[i].x,
                            Mathf.FloorToInt(currentCell / _tilesW) + neighbouringCellOffsets[i].y);

                        // test if the neighboring cell exists, is walkable 
                        if (neighbouringCell > 0 && neighbouringCell < maxTiles && _tiles[neighbouringCell] != null && _tiles[neighbouringCell].PathScore > 0)
                        {
                            //IVector3 neighbouringCellPosition = neighbouringCell.bounds.position;
                            float newCost = testedCells[currentCell] + _tiles[neighbouringCell].PathScore;

                            if (!TileEmpty(_tiles[neighbouringCell].Position))
                            {
                                // Make it more expensive if the tile isn't empty
                                newCost += 10f;
                            }

                            // test if the neighboring cell hasn't been tested or has a better score than previously tested
                            if (!testedCells.ContainsKey(neighbouringCell) || newCost < testedCells[neighbouringCell])
                            {
                                // update the cell cost
                                testedCells[neighbouringCell] = newCost;
                                parentCells[neighbouringCell] = currentCell;

                                float guessedCost = newCost + (float)GetHeuristic(neighbouringCellPosition, destination);
                                if (frontier.Contains(neighbouringCell))
                                {
                                    frontier.UpdatePriority(neighbouringCell, guessedCost);
                                }
                                else
                                {
                                    frontier.Enqueue(neighbouringCell, guessedCost);
                                }
                            }
                        }
                    }
                }

                // build the path from parent cells
                while (currentCell > -1)
                {
                    Tile currentTile = _tiles[currentCell];
                    Vector3 tilePos = Grid2World(currentTile.Position.x, currentTile.Position.y);
                    path.Add(tilePos);
                    currentCell = parentCells.ContainsKey(currentCell) ? parentCells[currentCell] : -1;
                }

                // reverse the path, so the first step is first
                path.Reverse();
            }
            return path.ToArray();
        }

        public Vector2Int GridPosFromIndex(int index)
        {
            int y = index / _tilesW;
            int x = index % _tilesW;

            return new Vector2Int(x, y);
        }

        private float GetHeuristic(Vector2Int from, Vector2Int to)
        {
            return new Vector2(to.x - from.x, to.y - from.y).magnitude;
        }

        // Use when x is even
        private Vector2Int[] neighbouringCellOffsetsOddX = {

            new Vector2Int(0, -1), // below
            new Vector2Int(0, 1),  // above

            new Vector2Int(-1, 0), // lower left
            new Vector2Int(-1, 1), // upper left

            new Vector2Int(1, 0), // lower right
            new Vector2Int(1, 1), // upper right
        };

        // Use when x is odd
        private Vector2Int[] neighbouringCellOffsetsEvenX = {
            new Vector2Int(0, -1), // below
            new Vector2Int(0, 1),  // above

            new Vector2Int(-1, -1), // lower left
            new Vector2Int(-1, 0), // upper left

            new Vector2Int(1, -1), // lower right
            new Vector2Int(1, 0), // uppwer right
        };

        private void DebugPath(Vector3[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                //Debug.Log("Path(" + i + ") "  + path[i]);
            }
        }

        #endregion






        
    }
}