using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD55
{
    public class ComputerOpponentController : MonoBehaviour
    {
        private GameSceneController _scene;
        private int _maxMoves = 10; // no more than 10 moves at a time

        public void Init(GameSceneController scene)
        {
            _scene = scene;
        }

        public void StartTurn()
        {
            StartCoroutine(TurnCoroutine());   
        }

        /*IEnumerator TurnCoroutine()
        {
            yield return new WaitForSeconds(1f);

            int moves = 0;
            while (moves++ < _maxMoves)
            {
                while (IsAnyOpponentAvatarMoving())
                {
                    yield return new WaitForSeconds(0.25f);
                }

                if (NextMove())
                {
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitForSeconds(1f);
            _scene.EndOpponentTurn();
        }*/

        IEnumerator TurnCoroutine()
        {
            yield return new WaitForSeconds(1f);

            int moves = 0;


            // First try summoning
            while (moves++ < _maxMoves)
            {
                // Try to summon first, if not try to move a unit
                if (SummonIfPossible())
                {
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitForEndOfFrame();

            // Next seek an objective
            AvatarController[] allAvatars = _scene.Board.GetAvatars();

            foreach (AvatarController opponentAvatar in allAvatars)
            {
                yield return new WaitForEndOfFrame();

                if (!opponentAvatar.IsPlayer)
                {
                    // Randomly chose the target

                    int choice = Random.Range(0, 2);
                    if (choice == 0)
                    {
                        Debug.Log("Opponent decided to seek a summon tile");
                        GoToPreferredPlayerSummonTile(opponentAvatar);
                    }
                    else
                    {
                        Debug.Log("Opponent decided to fight the player");
                        GoToPlayerAvatar(opponentAvatar);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
            _scene.EndOpponentTurn();
        }

        private bool IsAnyOpponentAvatarMoving()
        {
            AvatarController[] allAvatars = _scene.Board.GetAvatars();

            foreach (AvatarController opponentAvatar in allAvatars)
            {
                if (!opponentAvatar.IsPlayer)
                {
                    if (opponentAvatar.IsPathing)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool NextMove()
        {
            // Try to summon first, if not try to move a unit
            if (SummonIfPossible())
            {
                return true;
            }

            AvatarController avatar = GetAvatar();
            if (avatar != null)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                {
                    Debug.Log("Opponent decided to seek a summon tile");
                    return GoToPreferredPlayerSummonTile(avatar);
                }
                else
                {
                    Debug.Log("Opponent decided to fight the player");
                    return GoToPlayerAvatar(avatar);
                }
            }
            return false;

            
            // possibly a third option is to defend
            // Fourth is to claim unclaimed summoning
        }


        private bool SummonIfPossible()
        {
            CardController[] cards = _scene.OpponentHand.GetCards();

            Dictionary<SuitSO, List<CardController>> cardsBySuit = new Dictionary<SuitSO, List<CardController>>();
            SimplePriorityQueue<SuitSO> suitPriority = new SimplePriorityQueue<SuitSO>();

            foreach (CardController card in cards)
            {
                if (!cardsBySuit.ContainsKey(card.Card.Suit))
                {
                    List<CardController> newcards = new List<CardController>();
                    newcards.Add(card);
                    cardsBySuit.Add(card.Card.Suit, newcards);
                    suitPriority.Enqueue(card.Card.Suit, 1);
                }
                else
                {
                    cardsBySuit[card.Card.Suit].Add(card);
                    suitPriority.Enqueue(card.Card.Suit, cardsBySuit[card.Card.Suit].Count);
                }
            }

            // sort by suit size

            while (suitPriority.Count > 0)
            {
                SuitSO suit = suitPriority.Dequeue();
                if (cardsBySuit[suit].Count >= suit.SuitRequiresNumCardsToSummon)
                {
                    // This is the one, summon it

                    // Pick a random spawn point
                    SummonTileController tile = GetSummonTile();
                    if (tile != null)
                    {
                        List<CardController> cardsForSuit = cardsBySuit[suit];
                        for (int i = 0; i < suit.SuitRequiresNumCardsToSummon; i++)
                        {
                            CardController cardForSuit = cardsForSuit[i];
                            tile.AddCard(cardForSuit);
                        }
                        // Success, we just summoned
                        return true;
                    }
                    else
                    {
                        Debug.Log("Opponent has no available tiles");
                    }

                }
            }

            return false;
        }

        private SummonTileController GetSummonTile()
        {
            SummonTileController[] alltiles = _scene.Board.GetSummonTiles();
            List<SummonTileController> opponentTiles = new List<SummonTileController>();

            foreach (SummonTileController tile in alltiles)
            {
                if (tile.TileController.Tile.IsOpponentOwned && tile.CanOpponentAddCards())
                {
                    opponentTiles.Add(tile);
                }
            }

            if (opponentTiles.Count > 0)
            {

                int index = Random.Range(0, opponentTiles.Count);

                SummonTileController tile = opponentTiles[index];

                //if (tile.CanOpponentAddCards())
               // {
                //}

                return tile;
            }
            return null;
        }

        // return a random avatar that still has moves
        private AvatarController GetAvatar()
        {
            AvatarController[] allAvatars = _scene.Board.GetAvatars();

            List<AvatarController> opponentAvatars = new List<AvatarController>();

            foreach (AvatarController opponentAvatar in allAvatars)
            {
                if (!opponentAvatar.IsPlayer && opponentAvatar.RemainingDistanceThisTurn > 0)
                {
                    opponentAvatars.Add(opponentAvatar);
                }
            }

            if (opponentAvatars.Count > 0) {

                int index = Random.Range(0, opponentAvatars.Count);

                return opponentAvatars[index];
            }
            return null;
        }


        private bool GoToPlayerAvatar(AvatarController avatar)
        {
            // Avatars must have moves and can move at least one block
            AvatarController[] allAvatars = _scene.Board.GetAvatars();

            Vector2Int avatarPos = _scene.Board.World2Grid(avatar.transform.position);

            SimplePriorityQueue<AvatarController> playerAvatars = new SimplePriorityQueue<AvatarController>();

            foreach (AvatarController playerAvatar in allAvatars)
            {
                if (playerAvatar.IsPlayer)
                {
                    Vector2Int playerAvatarPos = _scene.Board.World2Grid(playerAvatar.transform.position);
                    float h = GetHeuristic(avatarPos, playerAvatarPos);
                    playerAvatars.Enqueue(playerAvatar, h);
                }
            }

            while (playerAvatars.Count > 0)
            {
                //TODO To make the opponent dumber, we can skip the best ones by dequeuing the first few

                // Get the first choice and make sure that it's pathable
                AvatarController playerAvatar = playerAvatars.Dequeue();
                Vector2Int playerAvatarPos = _scene.Board.World2Grid(playerAvatar.transform.position);
                Vector3[] path = _scene.Board.GetPath(avatarPos, playerAvatarPos);
                if (path != null && path.Length > 0)
                {
                    // Cut the path down to what this avatar can walk
                    path = ReducePath(path, avatar.RemainingDistanceThisTurn);

                    // Start pathing
                    avatar.SetPath(path);
                    return true;
                }
            }

            return false;
        }


        private Vector3[] ReducePath(Vector3[] path, int maxDistance)
        {
            if (path.Length <= maxDistance)
            {
                return path;
            }

            Vector3[] newpath = new Vector3[maxDistance];
            for (int i = 0; i < maxDistance; i++)
            {
                newpath[i] = path[i];
            }
            return newpath;
        }

        private bool GoToPreferredPlayerSummonTile(AvatarController avatar)
        {
            // Avatars must have moves and can move at least one block
            SummonTileController[] alltiles = _scene.Board.GetSummonTiles();

            Vector2Int avatarPos = _scene.Board.World2Grid(avatar.transform.position);

            SimplePriorityQueue< SummonTileController> playerTiles = new SimplePriorityQueue< SummonTileController>();

            foreach (SummonTileController tile in alltiles)
            {
                if (tile.TileController.Tile.IsPlayerOwned)
                {
                    float h = GetHeuristic(avatarPos, tile.TileController.Tile.Position);
                    playerTiles.Enqueue(tile, h);
                }
            }

            while (playerTiles.Count > 0)
            {
                //TODO To make the opponent dumber, we can skip the best ones by dequeuing the first few

                // Get the first choice and make sure that it's pathable
                SummonTileController tile = playerTiles.Dequeue();
                Vector3[] path = _scene.Board.GetPath(avatarPos, tile.TileController.Tile.Position);
                if (path != null && path.Length > 0)
                {
                    // Cut the path down to what this avatar can walk
                    path = ReducePath(path, avatar.RemainingDistanceThisTurn);

                    // Start pathing
                    avatar.SetPath(path);
                    return true;
                }
            }

            Debug.Log("Opponent could not find an available summon tile");
            return false;
        }

        private float GetHeuristic(Vector2Int from, Vector2Int to)
        {
            return new Vector2(to.x - from.x, to.y - from.y).magnitude;
        }
    }
}