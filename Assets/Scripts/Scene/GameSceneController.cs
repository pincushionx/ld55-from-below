using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    public class GameSceneController : MonoBehaviour
    {
        public Camera Camera;

        public PlayerController Player; // This is the player's interface (controls, UI control). If this game were to be multiplayer, this would only be relevant for the current client.
        public ComputerOpponentController Opponent;
        public BoardController Board; // The main game board. This includes the tiles and everything on them
        public HandController PlayerHand;
        public HandController OpponentHand;
        public DeckController Deck;
        public GameSceneOverlayController Overlay;
        public SelectionWindowController SelectionWindow;

        public bool IsPlayerTurn = true;

        private void Awake()
        {
            Player.Init(this);
            Opponent.Init(this);
            Board.Init(this);
            PlayerHand.Init(this);
            OpponentHand.Init(this);
            Deck.Init(this);
            Overlay.Init(this);
            SelectionWindow.Init(this);
        }
        // Start is called before the first frame update
        void Start()
        {
            if (GameManager.Instance.Level == 0)
            {
                BeginTutorialPlayerTurn();
            }
            else
            {
                BeginPlayerTurn();
            }
        }

        private void BeginTutorialPlayerTurn()
        {
            BeginPlayerTurn();

            Deck.DealCardToPlayerBySuitName("Stars");
            Deck.DealCardToPlayerBySuitName("Stars");
            Deck.DealCardToPlayerBySuitName("Stars");
            Deck.DealCardToPlayerBySuitName("Stars");
            Deck.DealCardToPlayerBySuitName("Stars");
        }

        public void EndPlayerTurn()
        {
            Overlay.ShowEndTurnButton(false);

            Debug.Log("End player turn");
            SelectionWindow.AddMessage("");
            SelectionWindow.AddMessage("You ended yout turn. It's your opponent's turn.");

            EndTurn();
            BeginOpponentTurn();
        }
        public void BeginPlayerTurn()
        {


            Debug.Log("BeginPlayerTurn");
            IsPlayerTurn = true;

            Overlay.ShowEndTurnButton(true);

            Deck.DealCardToPlayer();
            Deck.DealCardToPlayer();
            Deck.DealCardToPlayer();
        }

        public void EndOpponentTurn()
        {
            Debug.Log("End opponent turn");
            SelectionWindow.AddMessage("");
            SelectionWindow.AddMessage("Opponent ended their turn. It's your turn.");

            EndTurn();
            BeginPlayerTurn();
        }
        public void BeginOpponentTurn()
        {

            Debug.Log("BeginOpponentTurn");
            IsPlayerTurn = false;
            Deck.DealCardToOpponent();
            Deck.DealCardToOpponent();
            Deck.DealCardToOpponent();
            Opponent.StartTurn();
        }

        public void EndTurn()
        {
            Board.EndTurn();
            Debug.Log("New turn");
        }

        public void WinCondition()
        {
            Debug.Log("Win");

            SelectionWindow.AddMessage("");
            SelectionWindow.AddMessage("You Won!. Thanks for playing!");

            if (GameManager.Instance.Level == GameManager.Instance.LevelData[GameManager.Instance.LevelData.Count - 1].Id)
            {
                Overlay.ShowWinGameMessageBox();
            }
            else
            {
                Overlay.ShowWinLevelMessageBox();
            }
        }

        public void LoseCondition()
        {
            Debug.Log("Lose");

            SelectionWindow.AddMessage("");
            SelectionWindow.AddMessage("You Lost. Try again!");

            Overlay.ShowLoseLevelMessageBox();
        }
    }
}