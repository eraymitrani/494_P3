﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWind : MonoBehaviour {

	public GameObject wind;

	int[] windSpawnCoords = {-10, -9, 9, 10};

	void Start () {
		StartCoroutine (SpawnLoop ());
	}
	
	IEnumerator SpawnLoop() {
		// change to "while !gameOver -- not sure how team is managing this
		while (true) {
			
			float xcoord = windSpawnCoords [Random.Range (0, windSpawnCoords.Length)];
			float ycoord = Random.Range(-3, 3);

			GameObject w = GameObject.Instantiate (wind);
			w.transform.position = new Vector2 (xcoord, ycoord);

			// Wait for next spawn
			yield return new WaitForSeconds (5);
		}
	}
}
