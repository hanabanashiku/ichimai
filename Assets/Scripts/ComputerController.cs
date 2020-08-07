using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class ComputerController : Controller {

    /// <summary>
    /// Is the player taking its turn? Used to prevent multiple instances of TakeTurn().
    /// </summary>
    bool takingTurn = false;

	// Use this for initialization
	void Start () {
        HandRef = gameObject.transform.FindChild("Hand").gameObject;
        int PlayerCount = GameManager.gm.PlayerCount;
        float BoardWidth = 580 / 2;
        float BoardHeight =  300 / 2;
        Vector2 offset = new Vector2(-40, 0);
        float angle;
        if (PlayerCount == 2) angle = Mathf.PI / 2;
        else if (PlayerCount == 3) {
            angle = Mathf.PI / 4;
            if (Order == 2) angle *= 3;
            else angle *= 1;
        }
        else angle = (Order - 1) * Mathf.PI / (PlayerCount - 2);
        gameObject.transform.FindChild("Hand").localScale = new Vector3(0.7f, 0.85f, 1);
        gameObject.transform.localPosition = new Vector3(BoardWidth * Mathf.Cos(angle) + offset.x, BoardHeight * Mathf.Sin(angle) + offset.y, -60);
        PositionHand();
        gameObject.GetComponentInChildren<Text>().text = Name;
    }

    // Update is called once per frame
    void Update () {
	    if(GameManager.gm.Turn == Order && !takingTurn)
            StartCoroutine(TakeTurn());
	}

    IEnumerator TakeTurn() { // AI programming for a given turn
        float WaitTime = 2.5f;
        Debug.Log(Order + " taking turn!");
        takingTurn = true;
        yield return new WaitForSeconds(WaitTime); // make sure everything doesn't happen instantaneously 
        while(GameManager.gm.Turn == Order) {
            switch (GameManager.gm.CurrentState) {
                case GameManager.States.Wait:
                    if (!MovesAvailable()) { // No moves! Draw a card
                        Debug.Log("No moves! Drawing a card.");
                        AddCard(GameManager.GameDeck.Draw());
                        if (!MovesAvailable()) {// Still no moves! End turn
                            GameManager.gm.IncrementTurn();
                            Debug.Log("No moves still. Moving on...");
                        }
                    }
                    else { // grab the first selectable card and play it. Might improve on this in later versions.
                        Debug.Log("Looking for the perfect card...");
                        foreach (Card c in Hand)
                            if (c.Model.GetComponent<CardBehavior>().isSelectable) {
                                Debug.Log("Playing " + c.Color + " " + c.Face);
                                RemoveCard(c);
                                // Playing a card will re-parent the card and increment the turn counter accordingly.
                                GameManager.gm.PlayCard(c, Order);
                                break;
                            }
                    }
                    break;
                case GameManager.States.Draw:
                    Debug.Log("Drawing cards...");
                    while(GameManager.gm.DrawCount > 0) {
                        AddCard(GameManager.GameDeck.Draw());
                        GameManager.gm.DrawCount--;
                    }
                    GameManager.gm.CurrentState = GameManager.States.Wait;
                    GameManager.gm.IncrementTurn();
                    break;
                // The choose color function is called by the gamemanager.
            }
        }
        takingTurn = false;
        yield break;
    }

    /// <summary>
    /// Reposition the player's hand on the gameboard
    /// </summary>
    public override void PositionHand() {
        int offset = 4;
        for(int i = 0; i < Hand.Count; i++) {
            Hand[i].Model.transform.localPosition = new Vector3(i * offset, 0, 0);
            Hand[i].Model.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Allows the player to choose a game color
    /// </summary>
    /// <returns>The chosen color.</returns>
    public override Card.Colors ChooseColor() { // Chooses the color to the greatest advantage
        Debug.Log("Choosing color!");
        Dictionary<Card.Colors, int> count = new Dictionary<Card.Colors, int>();
        foreach (Card.Colors c in Enum.GetValues(typeof(Card.Colors)))
            if(c != Card.Colors.Wild) count[c] = 0;
        foreach (Card c in Hand)
            if(c.Color != Card.Colors.Wild) count[c.Color]++;
        if (GameManager.gm.DrawCount > 0) GameManager.gm.CurrentState = GameManager.States.Draw;
        else GameManager.gm.CurrentState = GameManager.States.Wait;
        Card.Colors choice = count.Aggregate((last, curr) => last.Value > curr.Value ? last : curr).Key;
        GameManager.Log.LogColor(choice);
        return choice;
    }

    /// <summary>
    /// Allows the player to choose a player to swap hands with.
    /// </summary>
    /// <returns>This will return the number of the player that the current player has chosen.</returns>
    public override int ChoosePlayer() {
        Debug.Log("Choosing player!");
        Dictionary<int, int> count = new Dictionary<int, int>();
        for(int i = 0; i < GameManager.gm.Players.Length; i++) {
            if (i == Order) continue;
            count[i] = GameManager.gm.Players[i].GetComponent<Controller>().Hand.Count;
        }
        int playerchoice = count.Aggregate((last, curr) => last.Value > curr.Value ? last : curr).Key;
        GameManager.gm.SwapHands(GameManager.gm.Players[Order], GameManager.gm.Players[playerchoice]);
        return playerchoice;
    }
}
