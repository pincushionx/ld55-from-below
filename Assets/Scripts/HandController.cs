
using UnityEngine;

namespace Pincushion.LD55
{
    public class HandController : MonoBehaviour
    {
        private GameSceneController _scene;
        private float _distanceFromCamera = 5f;
        private float _cardWidth = 0.5f; // Includes spacing

        private float _tightWidth = 0.1f; // Includes spacing
        private float _partWidth = 0.4f; // Includes spacing

        private float _width = 4;

        public Transform CardContainer;
        public Transform SelectCardContainer;


        public void Init(GameSceneController scene)
        {
            _scene = scene;
        }


        public void AddCardToHand(CardController card)
        {
            card.transform.SetParent(CardContainer, false);

            RepositionCards();
        }


        private CardController _lastHoveredCard = null;
        public void Hover(CardController hoverCard)
        {
            if (hoverCard == _lastHoveredCard)
            {
                return;
            }

            CardController[] cards = GetCards();

            _tightWidth = _width / cards.Length;
            if (_tightWidth > _cardWidth)
            {
                _tightWidth = _cardWidth;
            }

            CardContainer.localPosition = new Vector3(cards.Length * _tightWidth / -2, 0, 0);

            float x = 0;
            float zIncrement = 0.001f;
            float z = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                CardController card = cards[i];

                if (card == hoverCard)
                {
                   // x -= _partWidth;

                    //card.transform.localPosition = new Vector3(x, 0, z);
                    card.MoveToLocalPos(new Vector3(x, 0.05f, z+0.05f));

                    x += _partWidth;
                    z += zIncrement;
                }
                else
                {
                    //card.transform.localPosition = new Vector3(x, 0, z);
                    card.MoveToLocalPos(new Vector3(x, 0, z));

                    x += _tightWidth;
                    z += zIncrement;
                }
                
            }

            _lastHoveredCard = hoverCard;
        }
        
        public void RepositionCards()
        {
            CardController[] cards = GetCards();

            if (cards.Length > 0)
            {
//                Hover(cards[0]);
            }
            else
            {
                
                Debug.LogError("Couldn't reposition cards");
                return;
            }

            CardController hoverCard = cards[0];

            //CardController[] cards = GetCards();

            _tightWidth = _width / cards.Length;
            if (_tightWidth > _cardWidth)
            {
                _tightWidth = _cardWidth;
            }

            CardContainer.localPosition = new Vector3(cards.Length * _tightWidth / -2, 0, 0);

            float x = 0;
            float zIncrement = 0.001f;
            float z = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                CardController card = cards[i];

                if (card == hoverCard)
                {
                    // x -= _partWidth;

                    //card.transform.localPosition = new Vector3(x, 0, z);
                    card.MoveToLocalPos(new Vector3(x, 0.05f, z + 0.05f));

                    x += _partWidth;
                    z += zIncrement;
                }
                else
                {
                    //card.transform.localPosition = new Vector3(x, 0, z);
                    card.MoveToLocalPos(new Vector3(x, 0, z));

                    x += _tightWidth;
                    z += zIncrement;
                }

            }

            _lastHoveredCard = hoverCard;
        }

        public CardController[] GetCards()
        {
            return CardContainer.GetComponentsInChildren<CardController>();
        }








        /*public void RepositionSelectedCards()
{
    CardController[] cards = GetSelectedCards();

    SelectCardContainer.localPosition = new Vector3(cards.Length * _cardWidth / -2, 0, 0);

    float x = 0;
    for (int i = 0; i < cards.Length; i++)
    {
        CardController card = cards[i];

        card.transform.localPosition = new Vector3(x, 0, 0);

        x += _cardWidth;
    }
}



public void SelectCard(CardController selectCard)
{
    CardController[] handCards = GetCards();

    for (int i = 0; i < handCards.Length; i++)
    {
        CardController handCard = handCards[i];
        if (handCard == selectCard)
        {

        }
    }
}

private CardController[] GetSelectedCards()
{
    return CardContainer.GetComponentsInChildren<CardController>();
}
        
        
        private void Start()
        {
           // Reposition();
        }

        private void Reposition()
        {
            //transform.position = _scene.Camera.transform.position + _scene.Camera.transform.forward * _distanceFromCamera;

            //_scene.Camera.nearClipPlane
            //_scene.Camera.rect

            var screenBottomCenter = new Vector3(Screen.width / 2, 0, _scene.Camera.nearClipPlane);
            var inWorld = _scene.Camera.ScreenToWorldPoint(screenBottomCenter);












            Vector3[] frustumCorners = new Vector3[4];
            _scene.Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), _scene.Camera.nearClipPlane * _distanceFromCamera, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
            Vector3 worldSpaceCorner = _scene.Camera.transform.TransformVector(frustumCorners[3]);

            //  for (int i = 0; i < 4; i++)
           //   {
            //      var worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
           //       Debug.DrawRay(camera.transform.position, worldSpaceCorner, Color.blue);
           //   }



            Vector3 lowerForward = new Vector3(0, frustumCorners[3].y, frustumCorners[3].z);






            Vector3 handpos = inWorld + transform.TransformVector(lowerForward);

            

            transform.position = handpos;
            transform.LookAt(_scene.Camera.transform.position);


            RepositionCards();
        }*/
    }
}