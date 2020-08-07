using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controls the behavior of the color selector.
/// </summary>
public class ColorSelector : MonoBehaviour {

    void Start() {
        gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        RectTransform trans = (RectTransform) gameObject.transform;
        trans.offsetMin = new Vector2(100, 100);
        trans.offsetMax = new Vector2(-100, -100);
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -200);
        gameObject.transform.localScale = Vector3.one;
    }

	public void OnClick(Button btn) {
        if (GameManager.gm.Turn == GameManager.gm.PlayerIndex) {
            Card.Colors Color;
            switch (btn.name) {
                case "Red":
                    Color = Card.Colors.Red;
                    break;
                case "Blue":
                    Color = Card.Colors.Blue;
                    break;
                case "Green":
                    Color = Card.Colors.Green;
                    break;
                case "Yellow":
                    Color = Card.Colors.Yellow;
                    break;
                default: // something went wrong
                    Color = Card.Colors.Wild;
                    break;
            }
            GameManager.gm.ColorInPlay = Color;
            GameManager.Log.LogColor(Color);
            if (GameManager.gm.DrawCount > 0)
                GameManager.gm.CurrentState = GameManager.States.Draw;
            else GameManager.gm.CurrentState = GameManager.States.Wait;
            GameManager.gm.IncrementTurn();
            Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
