using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    public class PlayerController : MonoBehaviour
    {
        private Camera _camera;
        private GameSceneController _scene;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //ClickTest();
            if (_scene.IsPlayerTurn)
            {
                DoHoverOverBoard();
                DoHoverOverPlayerAvatar();
                DoHoverOverCard();
                DoPathHighlight();
            }

            DoHoverOverOpponentAvatar();
            DoClick();
        }


        public void Init(GameSceneController scene)
        {
            _scene = scene;
            _camera = _scene.Camera;
        }


        // Hover
        private AvatarController _hoveringOverAvatar;
        private AvatarController _hoveringOverOpponentAvatar;
        private TileController _hoveringOverTile;
        private SummonTileController _hoveringOverSummonTile;
        private CardController _hoverOverCard;

        // Selected
        private AvatarController _selectedAvatar;
        private AvatarController _selectedOpponentAvatar;
        private SummonTileController _selectedSummontile;

        private void DoHoverOverBoard()
        {
            _hoveringOverTile = null;
            _hoveringOverSummonTile = null;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Board")))
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;
                    //Debug.Log("Hit:" + collider.ColliderType);

                    if ((collider.ColliderType & ColliderType.Tile) > 0) // checks for all types of tiles
                    {
                        _hoveringOverTile = collider.gameObject.GetComponent<TileController>();

                        if ((collider.ColliderType & ColliderType.SummonTile) > 0)
                        {
                            _hoveringOverSummonTile = collider.gameObject.GetComponent<SummonTileController>();
                        }
                    }
                }

            }
        }

        // Only looks for Player's avatar
        private void DoHoverOverPlayerAvatar()
        {
            _hoveringOverAvatar = null;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Avatar")))
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;
                    
                    if (collider.ColliderType == ColliderType.Avatar)
                    {
                        _hoveringOverAvatar = collider.gameObject.GetComponent<AvatarController>();

                        // Make sure we're only dealing with the player's avatars
                        if (_hoveringOverAvatar != null && !_hoveringOverAvatar.IsPlayer)
                        {
                            _hoveringOverAvatar = null;
                        }
                    }
                }
            }
        }
        private void DoHoverOverOpponentAvatar()
        {
            _hoveringOverOpponentAvatar = null;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Avatar")))
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;

                    if (collider.ColliderType == ColliderType.Avatar)
                    {
                        _hoveringOverOpponentAvatar = collider.gameObject.GetComponent<AvatarController>();

                        // Make sure we're only dealing with the opponent's avatars
                        if (_hoveringOverOpponentAvatar != null && _hoveringOverOpponentAvatar.IsPlayer)
                        {
                            _hoveringOverOpponentAvatar = null;
                        }
                    }
                }
            }
        }

        private void DoHoverOverCard()
        {
            _hoverOverCard = null;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Card")))
            {

                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;
                    //Debug.Log("Hit:" + collider.ColliderType);

                    if (collider.ColliderType == ColliderType.Card)
                    {
                        _hoverOverCard = collider.gameObject.GetComponent<CardController>();


                        _scene.PlayerHand.Hover(_hoverOverCard);
                    }
                }
            }
        }

        private void DoPathHighlight()
        {
            _scene.Board.ClearHighlightPath();
            if (_selectedAvatar != null && _hoveringOverTile != null)
            {
                _scene.Board.HighlightPath(_selectedAvatar, _hoveringOverTile.Tile.Position);
            }
        }

        private void DoClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_hoveringOverAvatar != null)
                {
                    _selectedAvatar = _hoveringOverAvatar;
                    _selectedAvatar.SelectHilight();
                    _scene.SelectionWindow.SetSelectedAvatar(_selectedAvatar);

                    Debug.Log("Selected avatar");
                }
                else if (_selectedAvatar != null && _hoveringOverTile != null)
                {
                    // Move the avatar
                    _scene.Board.MoveAvatar(_selectedAvatar, _hoveringOverTile.Tile.Position);
                    _selectedAvatar.ClearSelectionHighlight();
                    _selectedAvatar = null;
                    Debug.Log("Moved avatar");
                }
                else if (_hoveringOverSummonTile != null)
                {
                    // If there are already cards placed, return them
                    if (_selectedSummontile != null && _hoveringOverSummonTile == _selectedSummontile && _selectedSummontile.HasCards())
                    {
                        _selectedSummontile.ReturnLastCardToPlayer();
                    }
                    // Don't let  the player select it for summoning until they can place cards
                    else if (_hoveringOverSummonTile.CanPlayerAddCards())
                    {
                        _selectedSummontile?.ClearSelectionHighlight();
                        _selectedSummontile = _hoveringOverSummonTile;
                        _selectedSummontile.ClearSelectionHighlight();
                        _selectedSummontile.SelectHilight();
                        Debug.Log("Selected summon tile");
                    }
                }
                else if (_selectedSummontile != null && _hoverOverCard != null)
                {
                    Debug.Log("Selected card: " + _hoverOverCard.Card.Name);

                    if (_selectedSummontile.CanPlayerAddCards())
                    {

                        if (_selectedSummontile.IsCardCompatible(_hoverOverCard))
                        {
                            _selectedSummontile.AddCard(_hoverOverCard);
                        }
                        else
                        {
                            _scene.SelectionWindow.AddErrorMessage("Sacrifice cards must have the same suit as the first card placed");

                            Debug.Log("Can't add card because it isn't the same suit as the one that's already placed: " + _hoverOverCard.Card.Name);
                        }
                    }
                    else
                    {
                        Debug.Log("Can't add card because there's an avatar on it: " + _hoverOverCard.Card.Name);

                    }
                }
                else if (_hoveringOverOpponentAvatar != null)
                {
                    _selectedOpponentAvatar = _hoveringOverOpponentAvatar;
                    _selectedOpponentAvatar.SelectHilight();
                    _scene.SelectionWindow.SetSelectedAvatar(_selectedOpponentAvatar);

                    Debug.Log("Selected opponent avatar");
                }
                else
                {
                    // Dese;ect if selected
                    _selectedAvatar?.ClearSelectionHighlight();
                    _selectedAvatar = null;

                    _selectedOpponentAvatar?.ClearSelectionHighlight();
                    _selectedOpponentAvatar = null;
                    _scene.SelectionWindow.ClearSelectedAvatar();

                    // if a summon tile has no cards, clear the selection
                    // Maybe better to do something more explicit
                    if (_selectedSummontile != null && !_selectedSummontile.HasCards())
                    {
                        _selectedSummontile?.ClearSelectionHighlight();
                        _selectedSummontile = null;
                    }

                    Debug.Log("Cleared selected avatar");
                }
            }
            else if (Input.GetMouseButtonDown(1)) // right click, clear selections
            {
                // Dese;ect if selected
                _selectedAvatar?.ClearSelectionHighlight();
                _selectedAvatar = null;

                _selectedOpponentAvatar?.ClearSelectionHighlight();
                _selectedOpponentAvatar = null;
                _scene.SelectionWindow.ClearSelectedAvatar();

                // if a summon tile has no cards, clear the selection
                // Maybe better to do something more explicit
                if (_selectedSummontile != null && !_selectedSummontile.HasCards())
                {
                    _selectedSummontile?.ClearSelectionHighlight();
                    _selectedSummontile = null;
                }

                Debug.Log("Cleared selected avatar by right clicking");
            }
        }

        private void ClickTest() {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Board")))
                {
                    Debug.Log("Point: " + hit.point + " grid: " + _scene.Board.World2Grid(hit.point));

                }
            }
        }
    }
}