using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents the player that the user has control over.
/// </summary>
public class PlayerController : Controller {

    void Awake() {
        HandRef = gameObject.transform.FindChild("Hand").gameObject;
        Hand = new List<Card>();
        PositionHand();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.gm.Turn == GameManager.gm.PlayerIndex) {
            if (GameManager.gm.CurrentState == GameManager.States.Wait && !MovesAvailable()) {
                // if there are no possible moves, draw a card instead.
                GameManager.gm.CurrentState = GameManager.States.Draw;
                GameManager.gm.DrawCount = 1;
            }
        }
    }

    /// <summary>
    /// Reposition the player's hand on the gameboard
    /// </summary>
    public override void PositionHand() {
        int width = 120;
        int n = Hand.Count;
        int x = -width * (n / 2) + 75;

        gameObject.transform.FindChild("Hand").localScale = new Vector3(0.7f, 0.85f, 1);
        for (int i = 0; i < n; i++) {
            Hand[i].Model.transform.localScale = new Vector3(5.0f, 4.0f, 3.3f);
            Hand[i].Model.transform.localPosition = new Vector3(x, 0, 0);
            x += width;
        }
    }

    /// <summary>
    /// Allows the player to choose a game color. For the player controller,
    /// this will launch a selection panel.
    /// </summary>
    /// <returns>Always returns wild, to be replaced when the user selects a color.</returns>
    public override Card.Colors ChooseColor() {
        GameManager.gm.CurrentState = GameManager.States.ChooseColor;
        Instantiate(Resources.Load("ColorSelector"));
        return Card.Colors.Wild;
    }

    /// <summary>
    /// Allows the player to choose a player to swap hands with.
    /// </summary>
    /// <returns>This will return the number of the player that the current player has chosen.</returns>
    /// <remarks>Always returns 0 for single player controller.</remarks>
    public override int ChoosePlayer() {
        GameManager.gm.CurrentState = GameManager.States.ChoosePlayer;
        Instantiate(Resources.Load("PlayerSelector"));
        return 0;
    }
}
