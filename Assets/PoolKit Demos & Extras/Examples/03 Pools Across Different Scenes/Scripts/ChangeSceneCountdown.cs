using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneCountdown : MonoBehaviour {

	public float waitForSeconds = 5f;
	public string sceneToLoad = "New Scene";

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds( waitForSeconds );
		SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single);
	}
	
}
