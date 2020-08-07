using UnityEngine;
using System.Collections;

/// <summary>
/// Utility behavior class to be attached to card instances.
/// </summary>
public class CardBehavior : MonoBehaviour {

    /// <summary>
    /// Is this card in the player's hand?
    /// </summary>
    public bool isPlayerCard;
    /// <summary>
    /// Can this card be played ontop of the card currently in play?
    /// </summary>
    public bool isSelectable;
    /// <summary>
    /// Reference to the corresponding instance of Card, assigned at instantiation time.
    /// </summary>
    public Card currentCard;

    // Use this for initialization
    void Start() {
        Update();
    }

    // Update is called once per frame
    public void Update() { // is the card selectable by the player?
        if (gameObject.transform.parent.parent.name == "PlayerObject") isPlayerCard = true;
        else isPlayerCard = false;

        if (currentCard.Color == Card.Colors.Wild || currentCard.Color == GameManager.gm.ColorInPlay)
            isSelectable = true;
        else if (currentCard.Face == GameManager.gm.NumberInPlay)
            isSelectable = true;
        // Stacking rule for draw cards
        else if (GameManager.gm.StackableDrawCards && GameManager.gm.DrawCount > 0 &&
            Card.isDrawCard(currentCard))
            isSelectable = true;
        else isSelectable = false;
    }

    void OnMouseOver() { // Hover over playable cards
        if (isPlayerCard && isSelectable &&
            GameManager.gm.Turn == GameManager.gm.PlayerIndex &&
            (GameManager.gm.CurrentState == GameManager.States.Wait ||
                (GameManager.gm.StackableDrawCards &&
                GameManager.gm.CurrentState == GameManager.States.Draw &&
                GameManager.gm.DrawCount > 1 && // So you can't play different cards just because you don't have a card to play in your hand
                (Card.isDrawCard(currentCard) || Card.isTurnCard(currentCard))
                )
            )){ 
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 10f, 0);
        }
    }
    void OnMouseExit() { // return to normal place
        if(gameObject.transform.localPosition.y != 0)
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0, 0);
    }
    void OnMouseUpAsButton() { // The user wants to play a card
        if (isPlayerCard && isSelectable && GameManager.gm.Turn == GameManager.gm.PlayerIndex &&
            (GameManager.gm.CurrentState == GameManager.States.Wait ||
                GameManager.gm.StackableDrawCards &&
                GameManager.gm.CurrentState == GameManager.States.Draw &&
                (Card.isDrawCard(currentCard) || Card.isTurnCard(currentCard))
                )) {
            GameManager.gm.Players[GameManager.gm.PlayerIndex].GetComponent<Controller>().RemoveCard(currentCard);
            GameManager.gm.PlayCard(currentCard, GameManager.gm.PlayerIndex);
        }
    }
}
