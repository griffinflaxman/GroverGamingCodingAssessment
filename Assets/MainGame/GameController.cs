using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

enum GameStates 
{
	GameStart,
	GameDuring
}

public class GameController : MonoBehaviour {

	public Button PlayButton;
	public Button IncreaseDenom;
	public Button DecreaseDenom;
	public Text DenomText;
	public Text CurrentBalance;
	public Text LastGameWin;
	public Sprite ClosedChest;

	internal static List<float> prizes = new List<float>();
	internal static int prizeCounter = 0;
	internal static float TotalPrize = 0.00f;
	internal static GameStates state = GameStates.GameStart;

	private float currentBalance;
	private float[] denominationValues = new float[] { 0.25f, 0.50f, 1.00f, 5.00f };
	private int[] tier1Multiplers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	private int[] tier2Multiplers = new int[] { 12, 16, 24, 32, 48, 64 };
	private int[] tier3Multiplers = new int[] { 100, 200, 300, 400, 500 };
	private int denomCounter = 2;
	private System.Random random = new System.Random ();

	// Use this for initialization
	void Start () 
	{
		var playButton = PlayButton.GetComponent<Button>();
		playButton.onClick.AddListener(StartGame);
		var incDenom = IncreaseDenom.GetComponent<Button>();
		incDenom.onClick.AddListener(IncreaseDenomination);
		var decDenom = DecreaseDenom.GetComponent<Button>();
		decDenom.onClick.AddListener(DecreaseDenomination);
	}
	
	// Update is called once per frame
	void Update () 
	{
		var currentBalanceText = CurrentBalance.text;
		currentBalanceText = currentBalanceText.Replace ("$", "");
		currentBalance = Convert.ToSingle (currentBalanceText);
		if (currentBalance == 0.00 && PlayButton.interactable) {
			//send game into game over if the balance is 0 and no money was earned during the round (i.e. play button is interactable)
			Debug.Log("Game Over!");
            SceneManager.LoadScene("GameOverScene");
		}

		//also check if the denomination (bet) value is less than the current balance
		if (currentBalance < GetDenominationValue() && state == GameStates.GameStart)
		{
			PlayButton.interactable = IncreaseDenom.interactable = false;
		}
		else if (currentBalance >= GetDenominationValue() && state == GameStates.GameStart)
		{
			PlayButton.interactable = IncreaseDenom.interactable = true;
		}
	}

	void IncreaseDenomination() 
	{
		var denomText = DenomText.text;
		denomText = denomText.Replace ("$", "");
		float denomValue = Convert.ToSingle (denomText);
		if (denomCounter < denominationValues.Length - 1 && currentBalance > denomValue) {
			denomValue = denominationValues [++denomCounter];
			DenomText.text = string.Format ("${0}", denomValue.ToString ("0.00"));
			Debug.Log (string.Format ("Denomination Increased: {0}", StringifyText(denomValue)));
		}
	}

	void DecreaseDenomination() 
	{
		var denomValue = GetDenominationValue ();
		if (denomCounter > 0 && currentBalance > 0.00) {
			denomValue = denominationValues [--denomCounter];
			DenomText.text = StringifyText(denomValue);
			Debug.Log (string.Format ("Denomination Decreased: {0}", StringifyText(denomValue)));
		}
	}

	void StartGame() 
	{
		//set the game state
		state = GameStates.GameDuring;
		//populate the prizes
		PopulatePrizes();
		//subtract the current denomination
		var denomValue = GetDenominationValue();
		currentBalance -= denomValue;
		CurrentBalance.text = StringifyText(currentBalance);
		//disable the play and denomination buttons
		ToggleButtons(false);
		//reset chests
		ResetChests();
		//reset last game win total
		LastGameWin.text = "$0.00";
	}

	void ToggleButtons(bool toggle) 
	{
		PlayButton.interactable = IncreaseDenom.interactable = DecreaseDenom.interactable = toggle;
	}

	void ResetChests() 
	{
		var chests = GameObject.FindGameObjectsWithTag ("Chest");
		foreach (var chest in chests) {
			SpriteRenderer renderer = chest.GetComponent<SpriteRenderer>();
			renderer.sprite = ClosedChest;
			//enable child text
			chest.GetComponentInChildren<MeshRenderer>().enabled = false;
		}
	}

	void PopulatePrizes() 
	{
		prizes.Clear ();
		prizeCounter = 0;
		var multiplier = GetMultiplier();
		var totalPrize = GetDenominationValue() * multiplier;
		Debug.Log (string.Format ("Multiplier: {0}x", multiplier));
		Debug.Log (string.Format ("Total prize won: ${0}", totalPrize));
		TotalPrize = totalPrize;
		if (totalPrize > 0.00f)
			AddPrizesToList (totalPrize);
		Shuffle (prizes);
		//add the pooper at the end
		prizes.Add (0f);
		Debug.Log ("Prizes Populated");
	}

	//Fisher-Yates shuffle algorithm
	private void Shuffle(List<float> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = random.Next(n + 1);  
			float value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	private string StringifyText(float value)
	{
		return string.Format ("${0}", value.ToString ("0.00"));
	}

	private float GetDenominationValue()
	{
		var denomText = DenomText.text;
		denomText = denomText.Replace ("$", "");
		return Convert.ToSingle (denomText);
	}

	private int GetMultiplier ()
	{
		var randomPercentage = random.Next (1, 1001);
		Debug.Log (string.Format("Percentage: {0}", randomPercentage));
		if (randomPercentage <= 500)
			return 0;
		else if (randomPercentage <= 800) {
			var index = random.Next (0, tier1Multiplers.Length);
			return tier1Multiplers [index];
		} 
		else if (randomPercentage <= 950) {
			var index = random.Next (0, tier2Multiplers.Length);
			return tier2Multiplers [index];
		} 
		else {
			var index = random.Next (0, tier3Multiplers.Length);
			return tier3Multiplers [index];
		}
	}

	private void AddPrizesToList(float totalPrize)
	{
		if (totalPrize >= 1000.00f)
		{
			var prize = (Convert.ToInt32(totalPrize) / 1000) * 1000.00f;
			prizes.Add(prize);
			totalPrize -= prize;
		}
		if (totalPrize >= 100.00f)
		{
			var prize = (Convert.ToInt32(totalPrize) / 100) * 100.00f;
			prizes.Add(prize);
			totalPrize -= prize;
		}
		if (totalPrize >= 10.00f)
		{
			var prize = (Convert.ToInt32(totalPrize) / 10) * 10.00f;
			prizes.Add(prize);
			totalPrize -= prize;
		}
		if (totalPrize >= 1.00f)
		{
			var prize = (Convert.ToInt32(totalPrize) / 1) * 1.00f;
			prizes.Add(prize);
			totalPrize -= prize;
		}

		while (totalPrize > 0.00f)
		{
			if (totalPrize < 0.75f && totalPrize > 0.25f)
			{
				var probability = random.Next(1, 3);
				if (probability == 2)
				{
					prizes.Add(totalPrize);
					totalPrize -= totalPrize;
				}
			}
			if (totalPrize / 0.25f >= 1.0f)
			{
				prizes.Add(0.25f);
				totalPrize -= 0.25f;
			}
			else
			{
				prizes.Add(totalPrize);
				totalPrize -= totalPrize;
			}
		}

		//to add a bit more variability
		if (prizes.Count < 5)
		{
			var probability = random.Next(1, 4);
			switch (probability)
			{
			//case 1 do nothing since no changes are made
				case 2:
					var prizeHalf = prizes[0] / 2.0f;
					prizes[0] = prizeHalf;
					prizes.Add(prizeHalf);
					break;
				case 3:
					var prizeQtr = prizes[0] / 4.0f;
					prizes[0] = prizeQtr;
					prizes.Add(prizeQtr);
					prizes.Add(prizeQtr);
					prizes.Add(prizeQtr);
					break;
			}
		}
		else if (prizes.Count < 8)
		{
			var probability = random.Next(1, 4);
			switch (probability)
			{
			//case 1 do nothing since no changes are made
				case 2:
					var prizeHalf = prizes[0] / 2.0f;
					prizes[0] = prizeHalf;
					prizes.Add(prizeHalf);
					break;
			}
		}
	}
}
