using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a game card.
/// </summary>
public class Card {
    public enum Colors { Red, Blue, Green, Yellow, Wild };
    public enum Numbers { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Skip, Reverse, Plus2, Plus4, Wild };

    public static GameObject Prefab = Resources.Load("Card", typeof(GameObject)) as GameObject;
    /// <summary>
    /// The color of the card
    /// </summary>
    public Colors Color { get; private set; }
    /// <summary>
    /// The number or action on the card's face
    /// </summary>
    public Numbers Face { get; private set; }
    /// <summary>
    /// The image on the front of the card
    /// </summary>
    public Material Front { get { return FetchImage(); } }

    /// <summary>
    /// The gameobject on the board that cooresponds to the card.
    /// </summary>
    public GameObject Model;

    public Card(Colors color, Numbers face) {
        if (color == Colors.Wild && !(face == Numbers.Wild || face == Numbers.Plus4))
            throw new Exception("Invalid card");
        if (face == Numbers.Plus4 && color != Colors.Wild)
            throw new Exception("Invalid card.");
        Color = color;
        Face = face;
    }

    /// <summary>
    /// Create the card's game object
    /// </summary>
    public void InstantiateObject() {
        if (Model != null) return; // don't create more than one instance
        GameObject clone;
        GameObject deck;
        deck = GameManager.GameDeck.gameObject;
        clone = GameObject.Instantiate(Prefab) as GameObject;
        
        clone.transform.SetParent(deck.transform);
        clone.transform.localPosition = new Vector3(0, 0, 0);

        GameObject frontface = clone.transform.FindChild("Front").gameObject;
        frontface.GetComponent<Renderer>().material = Front;
        frontface.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");

        Model = clone;
        Model.GetComponent<CardBehavior>().currentCard = this;
    }

    private Material FetchImage() {
        if (Color == Colors.Wild) {
            switch (Face) {
                case Numbers.Wild:
                    return Resources.Load("Materials\\Wild") as Material;
                case Numbers.Plus4:
                    return Resources.Load("Materials\\WildPlusFour") as Material;
                default:
                    throw new Exception();
            }
        }
        else {
            string file;
            switch (Color) {
                case Colors.Blue:
                    file = "Blue";
                    break;
                case Colors.Red:
                    file = "Red";
                    break;
                case Colors.Green:
                    file = "Green";
                    break;
                case Colors.Yellow:
                    file = "Yellow";
                    break;
                default:
                    throw new Exception();
            }
            switch (Face) {
                case Numbers.Zero:
                    file += "0";
                    break;
                case Numbers.One:
                    file += "1";
                    break;
                case Numbers.Two:
                    file += "2";
                    break;
                case Numbers.Three:
                    file += "3";
                    break;
                case Numbers.Four:
                    file += "4";
                    break;
                case Numbers.Five:
                    file += "5";
                    break;
                case Numbers.Six:
                    file += "6";
                    break;
                case Numbers.Seven:
                    file += "7";
                    break;
                case Numbers.Eight:
                    file += "8";
                    break;
                case Numbers.Nine:
                    file += "9";
                    break;
                case Numbers.Skip:
                    file += "Skip";
                    break;
                case Numbers.Reverse:
                    file += "Reverse";
                    break;
                case Numbers.Plus2:
                    file += "Plus2";
                    break;
                default:
                    throw new Exception();
            }
            return Resources.Load("Materials\\" + file) as Material;
        }
    }

    public static string HexFromColor(Colors color) {
        switch (color) {
            case Colors.Blue:
                return "#0094FF";
            case Colors.Green:
                return "#00FF21";
            case Colors.Red:
                return "#FF0000";
            case Colors.Yellow:
                return "#FFD800";
            default:
                return "#FFFFFF";
        }
    }

    /// <summary>
    /// Does the card trigger a draw count?
    /// </summary>
    /// <param name="c">The card to check</param>
    /// <returns>True if the card increases a draw count</returns>
    public static bool isDrawCard(Card c) {
        if (c.Face == Numbers.Plus2) return true;
        if (c.Face == Numbers.Plus4) return true;
        return false;
    }

    /// <summary>
    /// Does the card change the current turn?
    /// </summary>
    /// <param name="c">The card to check</param>
    /// <returns>True if the card will change the turn</returns>
    public static bool isTurnCard(Card c) {
        if (c.Face == Numbers.Reverse) return true;
        if (c.Face == Numbers.Skip) return true;
        return false;
    }
}
