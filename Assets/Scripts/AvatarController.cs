using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pincushion.LD55
{
    [RequireComponent(typeof(IColliderController))]
    public class AvatarController : MonoBehaviour
    {
        // Inspector
        public IColliderController Collider;
        [HideInInspector] public bool IsPlayer = true;

        public AoeController AoeController;


        public CardSO Card; // The card this avatar is based on

        // Status (initialized from cards)
        [HideInInspector] public int RemainingSleepTurns = 0;
        [HideInInspector] public int RemainingHealth = 10;
        [HideInInspector] public int RemainingDistanceThisTurn = 1;


        // From the card. They may change and only be based on the card.
        public int DistancePerTurn { get { return Card.DistancePerTurn; } }
        public int Damage { get { return Card.Damage; } }
        public int AoeDamage { get { return Card.AoeDamage; } }
        public int MaxHealth { get { return Card.MaxHealth; } }
        public int Defense { get { return Card.Defense; } }

        public bool HasAoe { get { return AoeDamage > 0; } }


        // Private
        private float _movementSpeed = 3f;
        private float pathPointThresholdDistance = 0.001f;
        private float _aoeRadius = 4f; // attempting to get everybody around them


        public bool IsPathing { get { return pathPosition >= 0; } }

        private Vector3[] _path;
        private int pathPosition = -1;


        private GameSceneController _scene;


        public Material PlayerOwnedMaterial;
        public Material OpponentOwnedMaterial;
        public MeshRenderer MeshRenderer;

        private FloatingTextController _floatingText;

        private void UpdateMaterial()
        {
            if (IsPlayer)
            {
                MeshRenderer.material = PlayerOwnedMaterial;
            }
            else
            {
                MeshRenderer.material = OpponentOwnedMaterial;
            }
        }

        public void Init(GameSceneController scene)
        {
            _scene = scene;

            _floatingText = GetComponent<FloatingTextController>();
            _floatingText.Init(_scene);
        }

        private void Start()
        {
            RemainingSleepTurns = Card.SleepTurns;
            RemainingHealth = Card.MaxHealth;
            RemainingDistanceThisTurn = Card.DistancePerTurn;


            UpdateMaterial();
        }

        public void ReceiveDamage(int damage)
        {
            int totalDamage = damage - Defense;


            if (totalDamage < 0)
            {
                totalDamage = 0;
            }

            if (totalDamage > RemainingHealth)
            {
                Debug.Log("Avatar is dead");
                _scene.SelectionWindow.AddMessage(Card.Name + " died from a " + damage + " hit");

                _floatingText.SpawnFloatingText(RemainingHealth.ToString());

                RemainingHealth = 0;
                _scene.Board.KillAvatar(this);
            }
            else
            {
                _scene.SelectionWindow.AddMessage(Card.Name + " was hit for " + damage + " damage");
                _floatingText.SpawnFloatingText(totalDamage.ToString());

                RemainingHealth -= totalDamage;
            }

        }


        public void EndTurn()
        {
            if (IsPlayer == _scene.IsPlayerTurn || !IsPlayer == !_scene.IsPlayerTurn)
            {
                DoAoeIfAvailable();
            }


            RemainingDistanceThisTurn = Card.DistancePerTurn;
        }

        void Update()
        {
            UpdatePath();
        }

        public void SetPath(Vector3[] path)
        {
            _path = path;
            pathPosition = 0;

            RemainingDistanceThisTurn -= path.Length - 1;
            if (RemainingDistanceThisTurn < 1) { RemainingDistanceThisTurn = 1; }
        }

        private bool HitPlayerIfNear(Vector3 nextNode)
        {
            AvatarController avatarAtEndOfPath = IsPlayer? _scene.Board.GetOpponentAvatar(nextNode) : _scene.Board.GetPlayerAvatar(nextNode);
            if (avatarAtEndOfPath != null)
            {
 
                Debug.Log("Avatar attacks");
                avatarAtEndOfPath.ReceiveDamage(Damage);

                RemainingDistanceThisTurn = 1; // Attacking ends the character's utility. 1 is zero

                return true;
            }
            return false;
        }

        // Should happen at the end of every turn
        private bool DoAoeIfAvailable()
        {
            if (!HasAoe) return false;

            // Circle raycast to get all nearby enemies
            Collider[] nearbyAvatars = Physics.OverlapSphere(transform.position, _aoeRadius, LayerMask.GetMask("Avatar"));
            List<AvatarController> avatars = new List<AvatarController>();

            foreach (Collider nearbyAvatar in nearbyAvatars)
            {
                IColliderController collider = nearbyAvatar.gameObject.GetComponent<IColliderController>();
                collider = collider.Root;

                AvatarController avatar = collider.gameObject.GetComponent<AvatarController>();

                if (avatar != null && avatar != this)
                {
                    if (IsPlayer && !avatar.IsPlayer)
                    {
                        // Look for opponents
                        avatars.Add(avatar);
                    }
                    else if(avatar.IsPlayer)
                    {
                        // Look for player
                        avatars.Add(avatar);
                    }
                }
            }

            // damage all nearby enemies
            foreach (AvatarController avatar in avatars)
            {
                avatar.ReceiveDamage(AoeDamage);
            }



            // Play vfx to show the aoe
            AoeController.DoAoE();



            return avatars.Count > 0;
        }



        /// <summary>
        /// updates pathing operations
        /// </summary>
        private void UpdatePath()
        {
            // If a path is set, move the avatar
            if (pathPosition > -1)
            {
                Vector3 testPosition = _path[pathPosition];
                if (Vector3.Distance(transform.position, testPosition) < pathPointThresholdDistance)
                {
                    pathPosition++; // set next path
                }

                if (pathPosition >= _path.Length)
                {
                    StopPathing();
                }


                if (pathPosition > 0)
                {
                    if( HitPlayerIfNear(_path[pathPosition]))
                    {
                        // There's an enemy there, hit him
                        StopPathing();
                    }
                    else
                    {


                        float delta = Time.deltaTime;
                        Vector3 previous = transform.position;
                        Vector3 destination = _path[pathPosition];
                        Vector3 currentPosition = Vector3.MoveTowards(previous, destination, delta * _movementSpeed);

                        transform.position = currentPosition;

                        //Vector3 lookAtPos = new Vector3();
                        //lookAtPos.x = destination.x;
                        //lookAtPos.y = transform.position.y;
                        //lookAtPos.z = destination.z;
                        //gameObject.transform.LookAt(lookAtPos);
                    }
                }
            }
        }
        private void StopPathing()
        {
            pathPosition = -1;
            _path = null;

            SummonTileController tile = GetTile(); 
            if (tile != null)
            {
                tile.AvatarLanded(this);
            }
        }

        private SummonTileController GetTile()
        {
            Ray ray = new Ray(transform.position + transform.up * 5f, -transform.up); ;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Board")))
            {
                IColliderController collider = hit.collider.gameObject.GetComponent<IColliderController>();

                if (collider != null)
                {
                    collider = collider.Root;
                    
                    if ((collider.ColliderType & ColliderType.Tile) > 0) // checks for all types of tiles
                    {
                        if ((collider.ColliderType & ColliderType.SummonTile) > 0)
                        {
                            return collider.gameObject.GetComponent<SummonTileController>();
                        }
                    }
                }

            }
            return null;
        }

        public void SelectHilight()
        {
            
        }

        public void ClearSelectionHighlight()
        {
            
        }
    }
}