using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour {

    void Start() 
    {
        Button resetBtn = this.gameObject.GetComponent<Button>();
        resetBtn.onClick.AddListener(ResetGame);
    }

    void ResetGame() 
    {
        Debug.Log("Resetting Game...");
        SceneManager.LoadScene("Scene1");
    }
}
