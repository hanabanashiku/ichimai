using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the behavior of the game deck.
/// </summary>
public class Deck : MonoBehaviour {

    /// <summary>
    /// The stack of cards in the game deck.
    /// </summary>
    public Stack<Card> Cards;
    private static System.Random r = new System.Random();

    void Awake() {
        Cards = new Stack<Card>(108);
        foreach (Card.Colors c in Enum.GetValues(typeof(Card.Colors))) {
            if (c == Card.Colors.Wild) {
                for (int i = 0; i < 4; i++) {
                    Cards.Push(new Card(c, Card.Numbers.Wild));
                    Cards.Push(new Card(c, Card.Numbers.Plus4));
                }
            }
            else {
                foreach (Card.Numbers f in Enum.GetValues(typeof(Card.Numbers))) {
                    if (f == Card.Numbers.Zero)
                        Cards.Push(new Card(c, f));
                    else if (f != Card.Numbers.Wild && f != Card.Numbers.Plus4)
                        for (int i = 0; i < 2; i++)
                            Cards.Push(new Card(c, f));
                }
            }
        }
        Shuffle();
    }

    /// <summary>
    /// Randomly order the game deck.
    /// </summary>
    private void Shuffle() {
        Cards = new Stack<Card>(Cards.OrderBy(x => r.Next()));
    }

    /// <summary>
    /// Draw the first card from the game deck.
    /// </summary>
    /// <returns></returns>
    public Card Draw() {
        return Cards.Pop();
    }

    void OnMouseUpAsButton() {
        // The player has a card to draw
        if (GameManager.gm.Turn == GameManager.gm.PlayerIndex && GameManager.gm.CurrentState == GameManager.States.Draw) {
            bool TryWait = (GameManager.gm.DrawCount == 1) ? true : false; // Drawing because no cards, so check to see if the card can be used
            while (GameManager.gm.DrawCount > 0) {
                GameManager.gm.Players[GameManager.gm.PlayerIndex].GetComponent<Controller>().AddCard(Draw());
                GameManager.gm.DrawCount--;
            }
            
            GameManager.gm.CurrentState = GameManager.States.Wait;
            GameManager.gm.Players[GameManager.gm.PlayerIndex].GetComponent<Controller>().PositionHand();
            // We are not waiting. Increment turn.
            if (!TryWait) GameManager.gm.IncrementTurn();
            // the new card didn't help. Increment turn.
            else if (!GameManager.gm.Players[GameManager.gm.PlayerIndex].GetComponent<Controller>().MovesAvailable())
                GameManager.gm.IncrementTurn();
        }
    }
}
