using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	public Transform canvas;

	private List<GameObject> bubbles;
	private List<Bubble> bubbleObjects;

	private Rect canvasDimensions;
	private float time;

	// Use this for initialization
	void Start () {
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;
		time = 0;

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();

		for (int i = 0; i < 50; i++) {
			bubbleObjects.Add(new Bubble ("Bubble", 
				new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
					(float)(Random.value * canvasDimensions.height * -2 - canvasDimensions.height / 2)), 
				new Vector2 ((float)(Random.value * 5 - 2.5), (float)(Random.value * 4 + 1)), 
				canvas, 
				(int) (Random.value * 3 + 7)));
			bubbles.Add(bubbleObjects[i].bubble);
		}
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		for (int i = 0; i < bubbles.Count; i++) {
			var testBody = bubbles[i].GetComponent<Rigidbody2D> ();
			var vel = testBody.velocity;
			vel.x = bubbleObjects [i].Amplitude * Mathf.Sin (time * bubbleObjects [i].Period);
			testBody.velocity = vel;

			if (bubbles[i].transform.localPosition.y > canvasDimensions.height / 2) {
				bubbles[i].transform.localPosition = 
					new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), -5 * canvasDimensions.height / 2);
			}
		}
	}
}
