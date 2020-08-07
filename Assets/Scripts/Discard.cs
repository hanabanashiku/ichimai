using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the behavior of the discard pile
/// </summary>
public class Discard : MonoBehaviour {

    /// <summary>
    /// Stack of discarded cards
    /// </summary>
    private Stack<Card> Cards;

    public Discard() {
        Cards = new Stack<Card>();
    }

    void Start() {
    }

    /// <summary>
    /// Add a card to the top of the pile
    /// </summary>
    /// <param name="c">The card to add</param>
    public void DiscardCard(Card c) {
        if(Cards.Count > 0) {
            Cards.Peek().Model.transform.localPosition = Vector3.zero;
        }

        Cards.Push(c);
        c.Model.transform.SetParent(gameObject.transform);
        c.Model.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        c.Model.transform.localScale = new Vector3(3.87f, 3.87f, 3.87f);
        c.Model.transform.localPosition = new Vector3(0, 0, -1);
        GameManager.gm.ColorInPlay = c.Color;
        GameManager.gm.NumberInPlay = c.Face; 
    }

    /// <summary>
    /// The card at the top of the pile.
    /// </summary>
    public Card CurrentCard {
        get { return Cards.Peek(); }
    }
}
