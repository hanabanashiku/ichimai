using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a player controller. 
/// </summary>
public abstract class Controller : MonoBehaviour {

    /// <summary>
    /// Contains all of the card instances currently in the player's hand.
    /// </summary>
    public List<Card> Hand;
    public GameObject HandRef;
    /// <summary>
    /// The index number for the player corresponding to GameManager.Players[].
    /// </summary>
    public int Order; // Which index number is the controller in Players[]
    /// <summary>
    /// The name of the player
    /// </summary>
    public string Name;
    /// <summary>
    /// Add a card to the player's hand
    /// </summary>
    /// <param name="c">The card to add</param>
    public void AddCard(Card c) {
        Hand.Add(c);
        c.Model.transform.SetParent(HandRef.transform);
        c.Model.transform.localPosition = new Vector3(0, 0, 0);
        c.Model.transform.localRotation = Quaternion.Euler(0, 0, 0);
        PositionHand();
    }

    /// <summary>
    /// Remove a card from the player's hand.
    /// </summary>
    /// <param name="c">The card instance to remove.</param>
    public void RemoveCard(Card c) {
        if (!Hand.Contains(c))
            return;
        Hand.Remove(c);
        PositionHand();
    }

    /// <summary>
    /// Is it possible for the player to play a card without drawing?
    /// </summary>
    public bool MovesAvailable() {
        foreach (Card c in Hand) {
            c.Model.GetComponent<CardBehavior>().Update();
            if (c.Model.GetComponent<CardBehavior>().isSelectable)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Reposition the player's hand on the gameboard
    /// </summary>
    public abstract void PositionHand();
    /// <summary>
    /// Allows the player to choose a game color
    /// </summary>
    /// <returns>The chosen color for NPCs.</returns>
    /// <remarks>This will always return Wild for player characters.</remarks>
    public abstract Card.Colors ChooseColor();
    /// <summary>
    /// Allows the player to choose a player to swap hands with.
    /// </summary>
    /// <returns>This will return the number of the player that the current player has chosen.</returns>
    public abstract int ChoosePlayer();
}
