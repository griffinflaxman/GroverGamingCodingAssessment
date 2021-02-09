using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChestController : MonoBehaviour {

	public Sprite Sprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown () 
	{
		var playButton = GameObject.FindGameObjectWithTag ("PlayButton").GetComponent<Button>();
        var lastGameWinText = GameObject.FindGameObjectWithTag("LastGameWin").GetComponent<Text>();
		if (playButton.interactable == false) {
			//replace the sprite to open on mouse down and display text above with prize amount
			var chestValue = GameController.prizes [GameController.prizeCounter++];
			SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = Sprite;
			//add the value of the chest to the prize and display it
			var prizeObject = this.gameObject.transform.Find ("Prize");
			prizeObject.GetComponent<TextMesh> ().text = StringifyText (chestValue);
			prizeObject.GetComponent<MeshRenderer> ().enabled = true;
            //add value to last game win total
            lastGameWinText.text = StringifyText(GetLastGameWinValue(lastGameWinText) + chestValue);
			if (chestValue == 0.00f) {
				var currentBalanceObj = GameObject.FindGameObjectWithTag ("CurrentBalance").GetComponent<Text>();
				var currentBalanceText = currentBalanceObj.text.Replace ("$", "");
				var currentBalance = Convert.ToSingle (currentBalanceText);
                currentBalance += GetLastGameWinValue(lastGameWinText);
				currentBalanceObj.text = StringifyText (currentBalance);
				playButton.interactable = GameObject.FindGameObjectWithTag ("IncDenom").GetComponent<Button>().interactable =
					GameObject.FindGameObjectWithTag ("DecDenom").GetComponent<Button>().interactable = true;
				GameController.state = GameStates.GameStart;
			}
		}
	}

    private float GetLastGameWinValue(Text text)
    {
        var lGWText = text.text;
        lGWText = lGWText.Replace ("$", "");
        return Convert.ToSingle (lGWText);
    }

	private string StringifyText(float value)
	{
		return string.Format ("${0}", value.ToString ("0.00"));
	}
}
