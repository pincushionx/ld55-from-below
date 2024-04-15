
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


namespace Pincushion.LD55
{
    [RequireComponent(typeof(TileController))]
    public class SummonTileController : MonoBehaviour
    {
        public Transform[] CardContainers; // must be exactly 7 (one for center, 6 surrounding)

        public TileController TileController;

        public Action<SummonTileController> OwnershipChanged;

        public Material PlayerOwnedMaterial;
        public Material PlayerOwnedSelectionMaterial;
        public Material OpponentOwnedMaterial;
        public MeshRenderer TileMeshRenderer;

        private void Awake()
        {
            TileController = GetComponent<TileController>();
        }
        private void Start()
        {
            UpdateMaterial();
        }



        public void AvatarLanded(AvatarController avatar)
        {
            ChangeOwnershipIfNeeded(avatar);
        }

        private void UpdateMaterial()
        {
            if (TileController.Tile.IsPlayerOwned)
            {
                TileMeshRenderer.material = PlayerOwnedMaterial;
            }
            else if (TileController.Tile.IsOpponentOwned)
            {
                TileMeshRenderer.material = OpponentOwnedMaterial;
            }
        }


        public void SelectHilight()
        {
            TileMeshRenderer.material = PlayerOwnedSelectionMaterial;
        }

        public void ClearSelectionHighlight()
        {
            UpdateMaterial();
        }

        private void ChangeOwnershipIfNeeded(AvatarController avatar)
        {
            if (TileController.Tile.IsPlayerOwned != avatar.IsPlayer
            || TileController.Tile.IsOpponentOwned == avatar.IsPlayer)
            {
                TileController.Tile.IsPlayerOwned = avatar.IsPlayer;
                TileController.Tile.IsOpponentOwned = !avatar.IsPlayer;

                UpdateMaterial();

                TileController.Scene.SelectionWindow.AddMessage((avatar.IsPlayer? "You" : "Your opponent") + " captured a summoning tile");

                OwnershipChanged?.Invoke(this);
            }
        }


        public bool HasCards()
        {
            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                if (container.childCount > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public int GetCardCount()
        {
            int count = 0;
            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                if (container.childCount > 0)
                {
                    count++;
                }
            }
            return count;
        }

        public bool IsCardCompatible(CardController card)
        {
            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                if (container.childCount == 0)
                {
                    return true;
                }
                else
                {
                    SuitSO currentSuit = container.GetChild(0).GetComponent<CardController>().Card.Suit;

                    if (currentSuit != card.Suit)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /*public bool CanAddCards()
        {
            if (!TileController.Scene.Board.TileEmpty(TileController.Tile.Position))
            {
                return false;
            }
            return true;
        }*/
        public bool CanOpponentAddCards()
        {
            if (!TileController.Tile.IsOpponentOwned || !TileController.Scene.Board.TileEmpty(TileController.Tile.Position))
            {
                return false;
            }
            return true;
        }
        public bool CanPlayerAddCards()
        {
            if (!TileController.Tile.IsPlayerOwned || !TileController.Scene.Board.TileEmpty(TileController.Tile.Position))
            {
                return false;
            }
            return true;
        }


        public void AddCard(CardController card)
        {

            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                if (container.childCount == 0)
                {
                    card.transform.SetParent(container.transform, true); // To animate this, set the param to true, then coroutine to 0

                    SummonIfConditionsMet();

                    StartCoroutine(CardMovementCoroutine(card));
                    return;
                }
            }
        }

        public void ReturnLastCardToPlayer()
        {
            for (int i = CardContainers.Length - 1; i >= 0; i--)
            {
                Transform container = CardContainers[i];

                if (container.childCount > 0)
                {
                    CardController card = container.GetChild(0).gameObject.GetComponent<CardController>();
                    if(card != null)
                    {
                        TileController.Scene.PlayerHand.AddCardToHand(card);
                        return;
                    }
                }
            }
        }


        // There's a race condition here. Added a time limiter to avoid it.
        private float SummonIfConditionsMetLastRun = 0;
        public void SummonIfConditionsMet()
        {
            if (!TileController.Scene.Board.TileEmpty(TileController.Tile.Position))
            {
                Debug.Log("Can't summong because the tile isn't empty");
            }

            if (HasCards())
            {
                CardSO card = CardContainers[0].GetChild(0).GetComponent<CardController>().Card;

                if (GetCardCount() >= card.Suit.SuitRequiresNumCardsToSummon)
                {
                    ClearSelectionHighlight();

                    TileController.Scene.SelectionWindow.AddMessage(card.Name + " summoned to " + (TileController.Tile.Position.x + 1) + " ," + (TileController.Tile.Position.y + 1));

                    // spawn avatar (that animation happens in BoardController)
                    TileController.Scene.Board.SpawnAvatar(TileController.Tile.Position, card, TileController.Tile.IsPlayerOwned);

                    // clear cards (that animation happens here)
                    StartCoroutine(SummonCoroutine());
                }
                else
                {
                    Debug.Log(GetCardCount() + " of " + card.Suit.SuitRequiresNumCardsToSummon + " on summon tile");
                }
            }
            
        }

        IEnumerator CardMovementCoroutine(CardController card)
        {
            float moveSpeed = .05f; // seconds per unit distance
            float progress = 0;

            Vector3 initialPosition = card.transform.localPosition;
            Quaternion initialRotation = card.transform.localRotation;

            Vector3 desiredPosition = Vector3.zero;
            Quaternion desiredRotation = Quaternion.identity;

            float distance = (desiredPosition - initialPosition).magnitude;

            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed / distance;
                progress += frameProgress;

                card.transform.localPosition = Vector3.Slerp(initialPosition, desiredPosition, progress);
                card.transform.localRotation = Quaternion.Slerp(initialRotation, desiredRotation, progress);
            }

            yield return null;
        }

        
        // This only destroys the cards. The summon is done by the board
        IEnumerator SummonCoroutine()
        {
        
            // The timing is icky here. Waiting a second since orchestrating using coroutines was causing race conditions
            yield return new WaitForSeconds(1);



            float moveSpeed = .1f; // seconds per unit distance


            // For now, drop them. Maybe later they get a dissolve effect

            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                for (int j = container.childCount - 1; j >= 0; j--)
                {
                    GameObject card = container.GetChild(j).gameObject;

                    Vector3 initialPosition = card.transform.localPosition;
                    Quaternion initialRotation = card.transform.localRotation;

                    Vector3 desiredPosition = card.transform.InverseTransformVector(new Vector3(0, -10f, 0));
                    Quaternion desiredRotation = Quaternion.identity;

                    float distance = (desiredPosition - initialPosition).magnitude;
                    float progress = 0;

                    while (progress < 1)
                    {
                        yield return null;

                        if (card.IsDestroyed())
                        {
                            yield break;
                        }

                        float frameProgress = Time.deltaTime / moveSpeed / distance;

                        card.transform.localPosition = Vector3.Slerp(initialPosition, desiredPosition, progress);
                        card.transform.localRotation = Quaternion.Slerp(initialRotation, desiredRotation, progress);

                        progress += frameProgress;
                    }
                }

            }

            yield return new WaitForEndOfFrame();

            // finally, destroy them
            for (int i = 0; i < CardContainers.Length; i++)
            {
                Transform container = CardContainers[i];

                for (int j = container.childCount - 1; j >= 0; j--)
                {
                    Destroy(container.GetChild(j).gameObject);
                }
            }

        }


    }
}