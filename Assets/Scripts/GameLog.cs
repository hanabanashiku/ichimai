using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLog : MonoBehaviour {

    public GameObject LastLine;
    public ScrollRect scroll;
    public Scrollbar scrollbar;
    public Text textlog;

	// Use this for initialization
	void Start () {
        ToggleView();
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            scroll.scrollSensitivity = 15;
	}
	
    void Update() {
        if (Input.GetButtonUp("Text")) ToggleView();
    }

    private void ToggleView() {
        if (gameObject.transform.localScale == Vector3.zero)
            gameObject.transform.localScale = Vector3.one;
        else gameObject.transform.localScale = Vector3.zero;
    }

    private void Log(string msg) {
        textlog.text += msg + "\n";
        LastLine.GetComponent<Text>().text = msg;
        StartCoroutine(resetScroll());
    }

    public void LogTurn(Controller p) {
        Log("It is now <b>" + p.Name + "</b>'s turn.");
    }

    public void LogCard(Controller p, Card c) {
        Log(string.Format("<b>{0}</b> put down a {1}.", p.Name, renderCardText(c))); 
    }

    public void LogColor(Card.Colors color) {
        Log(string.Format("The color in play is now <color={0}>{1}</color>.", Card.HexFromColor(color), color));
    }

    public void LogDrawState(Controller p) {
        Log(string.Format("Waiting for <b>{0}</b> to draw a card.", p.Name));
    }

    public void LogDrawStack(int count) {
        Log(string.Format("The current draw stack is now {0}.", count));
    }

    public void LogDraw(Controller p, int n) {
        string msg;
        if (n == 1) msg = string.Format("<b>{0}</b> drew a card from the deck.", p.Name);
        else msg = string.Format("<b>{0}</b> drew {1} cards from the deck.", p.Name, n);
        Log(msg);
    }

    private string renderCardText(Card c) {
        if (c.Color == Card.Colors.Wild) {
            if (c.Face == Card.Numbers.Wild) return string.Format("<color={0}>wi</color><color={1}>ld</color> <color={2}>ca</color><color={3}>rd</color>.", Card.HexFromColor(Card.Colors.Red), Card.HexFromColor(Card.Colors.Blue), Card.HexFromColor(Card.Colors.Green), Card.HexFromColor(Card.Colors.Yellow));
            else return string.Format("<color={0}>wi</color><color={1}>ld</color> <color={2}>{3}</color>.", Card.HexFromColor(Card.Colors.Red), Card.HexFromColor(Card.Colors.Blue), Card.HexFromColor(Card.Colors.Green), c.Face.ToString().Replace("Plus", "+"));
        }
        else return string.Format("<color={0}> {1} {2}</color>.", Card.HexFromColor(c.Color), c.Color, c.Face.ToString().Replace("Plus", "+"));
    }

    private IEnumerator resetScroll() {
        yield return null;
        scrollbar.value = 0;
    }
}
