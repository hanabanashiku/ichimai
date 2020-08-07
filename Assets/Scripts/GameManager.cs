using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages game initialization and flow.
/// </summary>
public class GameManager : MonoBehaviour {

    public enum States {
        /// <summary>
        /// The game is waiting for user or NPC input.
        /// </summary>
        Wait,
        /// <summary>
        /// The game is waiting for user or NPC to draw a card.
        /// </summary>
        Draw,
        /// <summary>
        /// The game is doing calculations.
        /// </summary>
        Busy,
        /// <summary>
        /// The game is waiting for the user or NPC to choose a color.
        /// </summary>
        ChooseColor,
        /// <summary>
        /// The game is waiting for the user or NPC to choose a player to swap hands with.
        /// </summary>
        ChoosePlayer};
    /// <summary>
    /// The type of game being played
    /// </summary>
    public enum GameTypes {
        /// <summary>
        /// A game with one player versus several NPCs
        /// </summary>
        SinglePlayer,
        /// <summary>
        /// A multiplayer game that is hosted by the user
        /// </summary>
        Host,
        /// <summary>
        /// A multiplayer game that is hosted by a different user
        /// </summary>
        Client
    }
    /// <summary>
    /// The type of game being played.
    /// </summary>
    public GameTypes GameType;
    /// <summary>
    /// The number of players in the game
    /// </summary>
    public int PlayerCount;
    /// <summary>
    /// The index of the current player of the game, usually 0.
    /// </summary>
    public int PlayerIndex;
    /// <summary>
    /// The index of the player that is currently taking their turn.
    /// </summary>
    public int Turn;
    /// <summary>
    /// How many cards should be drawn by the player?
    /// </summary>
    public int DrawCount = 0;
    /// <summary>
    /// Signifies the current direction. Generally changed by throwing a reverse card.
    /// </summary>
    public bool Clockwise;
    /// <summary>
    /// The game manager instance.
    /// </summary>
    public static GameManager gm;
    /// <summary>
    /// The game deck instance.
    /// </summary>
    public static Deck GameDeck;
    /// <summary>
    /// The discard pile instance.
    /// </summary>
    public static Discard DiscardPile;
    /// <summary>
    /// The gamelog instance.
    /// </summary>
    public static GameLog Log;
    /// <summary>
    /// A collections of all the players instantiated to the game board.
    /// </summary>
    public GameObject[] Players;
    public GameObject COM;
    private Card.Colors _colorInPlay;
    private string pname;
    /// <summary>
    /// The color of the card on the top of the discard pile. If it is wild, the color has yet to be chosen
    /// </summary>
    public Card.Colors ColorInPlay {
        get { return _colorInPlay; }
        set {
            _colorInPlay = value;
            string file = "materials/backgrounds/background-";
            switch (value) { // change the color with respect to the current card
                case Card.Colors.Blue: file += "blue"; break;
                case Card.Colors.Red: file += "red"; break;
                case Card.Colors.Green: file += "green"; break;
                case Card.Colors.Yellow: file += "yellow"; break;
                default: file += "default"; break;
            }
            gameObject.GetComponentInChildren<Image>().material = (Material)Resources.Load(file);
        }
    }
    /// <summary>
    /// The face of the card on the top of the discard pile.
    /// </summary>
    public Card.Numbers NumberInPlay;
    /// <summary>
    /// The current game state.
    /// </summary>
    public States CurrentState;

    /*** Alternative Rules ***/
    /// <summary>
    /// Enabling this option allows draw cards to be stacked between players
    /// When the player has a draw count, they can put down another draw card
    /// and the count will increase and pass onto the next player.
    /// They player may also use a skip/reverse to move onto someone else.
    /// </summary>
    public bool StackableDrawCards;

    /// <summary>
    /// If someone plays a 0, each player must pass their hand to the player
    /// in front of them.
    /// </summary>
    public bool PassHandOnZero;

    /// <summary>
    /// If someone plays a 7, that player has the ability to trade hands with
    /// one other player.
    /// </summary>
    public bool TradeHandOnSeven;

    /// <summary>
    /// This allows all players to see all the cards in eachother's hands.
    /// </summary>
    public bool ShowAllHands;

    void Awake() {
        GameObject obj = GameObject.Find("Relayer");
        if (obj == null) return; // probably working with the editor
        if (GameType == GameTypes.SinglePlayer) {
            var relay = obj.GetComponent<SinglePlayerSceneDataRelayer>();
            if (relay == null) return;
            PlayerCount = relay.PlayerCount;
            pname = relay.PlayerName;
        }
        else if (GameType == GameTypes.Host) { }
        else if (GameType == GameTypes.Client) { }
        Destroy(obj);
    }

    // Use this for initialization
    void Start() {
        gm = this;
        GameDeck = GameObject.Find("Deck").GetComponent<Deck>();
        DiscardPile = GameObject.Find("Discard").GetComponent<Discard>();
        Log = GameObject.Find("GameLog").GetComponent<GameLog>();
        Players = new GameObject[PlayerCount];
        Players[0] = GameObject.Find("PlayerObject");
        Players[0].GetComponent<Controller>().Order = 0;
        Players[0].GetComponent<Controller>().Name = pname;
        PlayerIndex = 0;
        for (int i = 1; i < PlayerCount; i++) {
            Players[i] = Instantiate(COM);
            Players[i].transform.SetParent(gameObject.transform);
            Players[i].GetComponent<Controller>().Order = i;
            Players[i].GetComponent<Controller>().Name = "COM " + i;
            if (ShowAllHands) {
                Players[i].transform.localRotation = Quaternion.Euler(180, 0, 0);
                Players[i].transform.FindChild("Name").localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
        foreach (Card c in GameDeck.Cards) {
            c.InstantiateObject();
        }
        for (int i = 0; i < 7; i++) // distribute cards
            foreach (GameObject player in Players) {
                Controller pc = player.GetComponent<Controller>();
                if (pc.Hand == null) pc.Hand = new List<Card>();
                Card c = GameDeck.Draw();
                player.GetComponent<Controller>().AddCard(c);
            }

        Turn = (int)System.Math.Floor(Random.Range(0, 3.9f));
        Log.LogTurn(Players[Turn].GetComponent<Controller>());
        Clockwise = false;
        DiscardPile.DiscardCard(GameDeck.Draw());
        if (ColorInPlay == Card.Colors.Wild) ColorInPlay = Players[Turn].GetComponent<Controller>().ChooseColor();
        CurrentState = States.Wait;
    }

    /// <summary>
    /// Dispatch a card to the discard pile and update accordingly.
    /// </summary>
    /// <param name="c">The card instance to play</param>
    /// <param name="Player">The current player</param>
    public void PlayCard(Card c, int Player) {
        CurrentState = States.Busy;
        Log.LogCard(Players[Player].GetComponent<Controller>(), c);
        DiscardPile.DiscardCard(c); // add to discard pile and set current color and number
        if (ColorInPlay == Card.Colors.Wild) {
            CurrentState = States.ChooseColor;
            ColorInPlay = Players[Player].GetComponent<Controller>().ChooseColor(); // this will return wild for player controller while waiting for selection
        }
        switch (c.Face) {
            case Card.Numbers.Plus2: // +2 played: The next player draws two
                Debug.Log("Plus two!");
                DrawCount += 2;
                if (CurrentState != States.ChooseColor) CurrentState = States.Draw;
                break;
            case Card.Numbers.Plus4: // +4 played: The next player draws four
                Debug.Log("Plus four!");
                DrawCount += 4;
                if (CurrentState != States.ChooseColor) CurrentState = States.Draw;
                break;
            case Card.Numbers.Skip: // Skip played: Increment the turn twice.
                Debug.Log("Skip!");
                IncrementTurn(true);
                // For stackable draw cards
                if (CurrentState != States.Draw)
                    CurrentState = States.Wait;
                break;
            case Card.Numbers.Reverse: // Reverse played: switch the order
                Debug.Log("Reverse!");
                Clockwise = !Clockwise;
                if (PlayerCount == 2) IncrementTurn(true); // For two players, this functions as a skip
                // For stackable draw cards
                if (CurrentState != States.Draw)
                    CurrentState = States.Wait;
                break;
            default:
                if (CurrentState != States.ChooseColor) CurrentState = States.Wait;
                break;
        }
        if (CurrentState == States.Wait && Players[Player].GetComponent<Controller>().Hand.Count == 1)
            StartCoroutine(DisplayIchimai());
        if (Players[Turn].GetComponent<Controller>().Hand.Count == 0) { // a player won.
            CurrentState = States.Busy;
            StartCoroutine(LoadEndScene());
        }
        if (PassHandOnZero && NumberInPlay == Card.Numbers.Zero) PassHandsUp();
        else if (TradeHandOnSeven && NumberInPlay == Card.Numbers.Seven) {
            Players[Turn].GetComponent<Controller>().ChoosePlayer();
        }
        if (CurrentState != States.ChooseColor && CurrentState != States.ChoosePlayer)
            IncrementTurn(); // the player controller will become responsbile for calling this if the state is choosecolor
    }

    /// <summary>
    /// Move on to the next player in line.
    /// </summary>
    public void IncrementTurn(bool silent = false) {
        if (Clockwise) {
            Turn--;
            if (Turn < 0) Turn = PlayerCount - 1;
        }
        else {
            Turn++;
            if (Turn >= PlayerCount) Turn = 0;
        }

        Debug.Log("It's player " + Turn + "'s turn! Current card is " + ColorInPlay + " " + NumberInPlay);
        if (!silent) Log.LogTurn(Players[Turn].GetComponent<Controller>());
        if (CurrentState == States.Draw) {
            if (StackableDrawCards) Log.LogDrawStack(DrawCount);
            Log.LogDrawState(Players[Turn].GetComponent<Controller>());
        }
    }

    IEnumerator DisplayIchimai() {
        GameObject ichimai = Instantiate(Resources.Load("Ichimai")) as GameObject;
        ichimai.transform.SetParent(gameObject.transform);
        yield return new WaitForSeconds(1.5f);
        Destroy(ichimai);
    }

    IEnumerator LoadEndScene() {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("WinScreen");
    }

    /// <summary>
    /// Swap hands between two players, for use with hand-swapping rule
    /// </summary>
    /// <param name="p1">Player to swap with</param>
    /// <param name="p2">Player to swap with</param>
    public void SwapHands(GameObject p1, GameObject p2) {
        Transform hand1 = p1.transform.FindChild("Hand");
        Transform hand2 = p2.transform.FindChild("Hand");

        hand1.SetParent(p2.transform);
        p2.GetComponent<Controller>().HandRef = hand1.gameObject;
        hand2.SetParent(p1.transform);
        p1.GetComponent<Controller>().HandRef = hand2.gameObject;
        hand1.localPosition = Vector3.zero;
        hand2.localPosition = Vector3.zero;

        p1.GetComponent<Controller>().PositionHand();
        p2.GetComponent<Controller>().PositionHand();

        List<Card> t = p1.GetComponent<Controller>().Hand;
        p1.GetComponent<Controller>().Hand = p2.GetComponent<Controller>().Hand;
        p2.GetComponent<Controller>().Hand = t;
    }

    /// <summary>
    /// Every player will give their hand to the next player
    /// </summary>
    public void PassHandsUp() {
        int j;
        int k = 0;
        // if we push the 0th hand to the correct position, the offset
        // will move the others into the correct spot as well
        for (int i = 0; i < Players.Length - 1; i++) {
            j = k;
            if (Clockwise) {
                if (j == Players.Length - 1) break; // we're done
                if (k == Players.Length - 1) k = 0;
                else k++;
                SwapHands(Players[j], Players[k]);
            }
            else {
                if (j == 1) break; // we're done
                if (k == 0) k = Players.Length - 1;
                else k--;
                SwapHands(Players[j], Players[k]);
        }}}
}
