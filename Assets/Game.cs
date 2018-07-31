using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Game : MonoBehaviour {

	public Transform canvas;
	private Rect canvasDimensions;

	private float spriteWidth;

	private List<GameObject> bubbles;
	private List<Bubble> bubbleObjects;

	private int touchCount;
	private int currentIndex;

	private float timeElapsed;

	private bool clickOccurred;
	private bool isGameOver;

	private float poppedPosition;

	private const int BubbleCount = 360;
	private int RealBubbleCount;

	public GameObject[] gameOverButtons;

	private GameObject test;
	private float time;

	private AudioSource audioSource;
	private SoundPlayer soundPlayer;

	private readonly Color TextColor = new Color (249f / 255, 123f / 255, 40f / 255, 1f);

	public Text instructions;
	public GameObject tuffy;

	public GameObject[] items;
	private int itemIndex;
	private float itemYPos;

	private const int DefaultPartCount = 4;
	private const int RocketPartCount = 5;

	private int collectedPartsCount;

	private List<GameObject> moreBubbles;

	// Use this for initialization
	void Start () {
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;

		audioSource = GetComponent<AudioSource> ();
		soundPlayer = new SoundPlayer (audioSource);

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();
		moreBubbles = new List<GameObject> ();

		itemIndex = (int)(Random.value * items.Length);
		RealBubbleCount = (itemIndex == 1) ? RocketPartCount : DefaultPartCount;
		collectedPartsCount = 0;
		itemYPos = items [itemIndex].transform.localPosition.y;

		for (int i = 0; i < items.Length; i++) {
			if (i == itemIndex) {

			} else {
				items [i].SetActive (false);
			}
		}

		int tempCount = 0;
		for (int i = 0; i < items.Length; i++) {
			for (int j = 1; j < (i == 1 ? RocketPartCount : DefaultPartCount); j++) {
				string sprite = "Item" + (i + 1) + "/Part" + (j + 1);
				int divideBySize = 10 * (int)randomNormal (8, 3, 3, 20);
				bubbleObjects.Add(CreateBubble (sprite, divideBySize));
				bubbles.Add (bubbleObjects [tempCount].bubble);
				tempCount++;
			}
		}

		bubbleObjects.Add(CreateBubble ("Item1/Part1", 10 * (int)randomNormal (8, 3, 3, 20))); // Box part that's shared
		bubbles.Add (bubbleObjects [tempCount].bubble);
		tempCount++;

		for (int i = tempCount; i < BubbleCount; i++) {
			string sprite = "Bubble"; 
			int divideBySize = (int)randomNormal (8, 3, 3, 20); //(int) (Random.value * 5 + 6);
			bubbleObjects.Add(CreateBubble (sprite, divideBySize));
			bubbles.Add(bubbleObjects[i].bubble);
		}

		time = 0;

		touchCount = 0;
		currentIndex = -1;

		timeElapsed = 0;
		clickOccurred = false;
		isGameOver = false;

		poppedPosition = -canvasDimensions.width / 2 + canvasDimensions.width / BubbleCount;

		foreach (GameObject button in gameOverButtons) {
			TextSharpener.SharpenText (button.GetComponentInChildren<Text> ());
			button.SetActive (false);
		}

		soundPlayer.PlayBackgroundBubbling ();

		float tuffyXOffset = tuffy.GetComponent<RectTransform> ().rect.width * tuffy.transform.localScale.x * 7 / 10;
		tuffy.transform.localPosition = new Vector2(canvasDimensions.width / 2 + tuffyXOffset, 0);
	}
	
	// Update is called once per frame
	void Update () {
		CheckClicks ();

		isGameOver = (collectedPartsCount == RealBubbleCount);

		if (isGameOver) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 1) {
				timeElapsed = 0;
				isGameOver = false;
				collectedPartsCount += 10; // to stop GameOver triggering more than once

				foreach (GameObject button in gameOverButtons) {
					button.SetActive (true);
				}

				soundPlayer.PlayGameOverSounds ();
				StartCoroutine(AnimateTuffy ());
			}
		}
	}

	Bubble CreateBubble(string sprite, int divideBySize) {
		var pos = new Vector3 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
			(float)((Random.value - 0.5f) * canvasDimensions.height), 
			Random.value);

		int maxYSpeed = sprite.Contains ("Item") ? 7 : 10;

		return new Bubble (sprite, 
			pos, 
			new Vector2 (0, 0), 
			canvas, 
			divideBySize);
	}

	void CheckClicks() {
		var hit = new RaycastHit2D();
		for (int i = 0; i < Input.touchCount; i++) {
			if (Input.GetTouch (i).phase.Equals (TouchPhase.Began)) {
				var ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);
				hit = Physics2D.Raycast (ray.origin, ray.direction);
				if (hit) {
					var bubble = hit.transform.gameObject;
					HandleClick (bubble);
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction);
			if (hit) {
				var bubble = hit.transform.gameObject;
				HandleClick (bubble);
			}
		}
	}

	void HandleBubbleMotion() {
		time += Time.deltaTime;
		for (int i = 0; i < bubbles.Count; i++) {
			var testBody = bubbles[i].GetComponent<Rigidbody2D> ();
			var vel = testBody.velocity;
			if (i != currentIndex) {
				vel.x = bubbleObjects [i].Amplitude * Mathf.Sin (time * bubbleObjects [i].Period);
			}
			testBody.velocity = vel;

			if (bubbles[i].transform.localPosition.y > canvasDimensions.height / 2 + 20) {
				bubbles[i].transform.localPosition = 
					new Vector3 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
						-5 * canvasDimensions.height / 2, 
						bubbles[i].transform.localPosition.z);
			}
		}
		items [itemIndex].transform.localPosition = 
			new Vector3 (items [itemIndex].transform.localPosition.x, itemYPos + 15 * Mathf.Sin (time), -0.01f);
	}

	void HandleClick(GameObject bubble) {
		bool found = false;
		for (int i = 0; i < bubbles.Count; i++) {
			if (bubble == bubbles[i]) {
				currentIndex = i;
				found = true;
				break;
			}
		}

		var rb = bubble.GetComponent<Rigidbody2D> ();

		var sprite = bubbleObjects [currentIndex].Sprite;
		if (sprite.Contains ("Item" + (itemIndex + 1)) || sprite.Equals ("Item1/Part1")) { // Correct
			if (touchCount % 2 == 0 && collectedPartsCount + 1 != RealBubbleCount) {
				soundPlayer.PlayCorrectAnswerSounds ();
			} else {
				soundPlayer.PlayPopSound(1);
			}
			// Logic for right
			var clickedObject = bubbles [currentIndex];
			bubbles.Remove (clickedObject);
			bubbleObjects.RemoveAt (currentIndex);
			Destroy (clickedObject);

			var part = items [itemIndex].transform.Find ("Part" + sprite [sprite.Length - 1]).gameObject;
			part.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);

			touchCount++;
			collectedPartsCount++;
		} else {
			soundPlayer.PlayPopSound(1);
			bubbles.Remove (bubble);
			Destroy (bubble);
			bubbleObjects.RemoveAt(currentIndex);
			if (!sprite.Equals ("Bubble")) {
				for (int i = 0; i < 20; i++) {
					var newBubble = CreateBubble("Bubble", (int)randomNormal(8, 3, 3, 20));
					bubbleObjects.Add (newBubble);
					bubbles.Add (newBubble.bubble);
				}
			}
		} 

		clickOccurred = true;

		StartCoroutine (FadeInstructions());
	}

	float randomNormal(float mean, float stdDev, float min, float max) {
		float u1 = 1 - Random.value;
		float u2 = 1 - Random.value;
		float randStdNormal = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2 * Mathf.PI * u2); 
		float randNum = mean + stdDev * randStdNormal;
		if (randNum < min || randNum > max) {
			return randomNormal (mean, stdDev, min, max);
		} else {
			return randNum;
		}
	}

	IEnumerator FadeInstructions() {
		if (instructions.color.a == 1) {
			for (float i = 1; i >= 0; i -= Time.deltaTime) {
				instructions.color = new Color (TextColor.r, TextColor.g, TextColor.b, i);
				yield return null;
			}
		}
	}

	IEnumerator AnimateTuffy() {
		int width = (int) (tuffy.GetComponent<RectTransform> ().rect.width * tuffy.transform.localScale.x);
		for (int i = 0; i < width; i++) {
			var pos = tuffy.transform.localPosition;
			tuffy.transform.localPosition = new Vector2 (pos.x - 1, pos.y);
			yield return null;
		}
	}
}
