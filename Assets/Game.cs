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

	private const int BubbleCount = 400;
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

	private Rect menuButtonBounds;

	private readonly int[] DefaultPartCounts = {7, 8, 8, 6, 1};
	private bool isTuffyVariant;

	private int collectedPartsCount;

	private List<GameObject> moreBubbles;

	public Material defaultMaterial;

	// Use this for initialization
	void Start () {
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;

		audioSource = GetComponent<AudioSource> ();
		soundPlayer = new SoundPlayer (audioSource);

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();
		moreBubbles = new List<GameObject> ();

		itemIndex = (int)(Random.value * items.Length);
		RealBubbleCount = DefaultPartCounts [itemIndex];
		isTuffyVariant = RealBubbleCount == 1;
		if (isTuffyVariant) {
			instructions.text = "Find the correct image of Tuffy Tiger in the bubbles!";
		}
		collectedPartsCount = 0;
		itemYPos = items [itemIndex].transform.localPosition.y;

		menuButtonBounds = instructions.GetComponent<RectTransform> ().rect;
		menuButtonBounds.size *= 2;
		menuButtonBounds.center = new Vector2(-canvasDimensions.width / 2, canvasDimensions.height / 2 - 10);

		for (int i = 0; i < items.Length; i++) {
			if (i == itemIndex) {

			} else {
				items [i].SetActive (false);
			}
		}

		int tempCount = 0;
		for (int i = 0; i < items.Length; i++) {
			var temp = DefaultPartCounts [i] == 1 ? 2 : DefaultPartCounts [i];
			for (int j = 1; j < temp; j++) {
				string sprite = "Item" + (i + 1) + "/Part" + (j + 1);
				int divideBySize = 5 * (int)randomNormal (7, 4, 3, 12);
				bubbleObjects.Add(CreateBubble (sprite, divideBySize));
				bubbles.Add (bubbleObjects [tempCount].bubble);
				tempCount++;
			}
		}

		// Box part that's shared
		bubbleObjects.Add(CreateBubble ("Item1/Part1", 5 * (int)randomNormal (7, 4, 3, 12))); 
		bubbles.Add (bubbleObjects [tempCount].bubble);
		tempCount++;

		// Random tuffy objects
		for (int i = tempCount; i < tempCount + 45; i++) {
			string sprite = "tuffy"; 
			int divideBySize = 5 * (int)randomNormal (7, 4, 3, 12); 
			bubbleObjects.Add(CreateBubble (sprite, divideBySize));
			bubbles.Add(bubbleObjects[i].bubble);
		}
		tempCount += 45;

		for (int i = tempCount; i < BubbleCount; i++) {
			string sprite = "Bubble"; 
			int divideBySize = (int)randomNormal (7, 4, 3, 12); 
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
		PlayBackgroundMusic ();
		collectedPartsCount = RealBubbleCount - 1;
		float tuffyXOffset = tuffy.GetComponent<RectTransform> ().rect.width * tuffy.transform.localScale.x * 9 / 10;
		tuffy.transform.localPosition = new Vector2(canvasDimensions.width / 2 + tuffyXOffset, -9 * canvasDimensions.height / 10);
	}
	
	// Update is called once per frame
	void Update () {
		CheckClicks ();
		HandleBubbleMotion ();

		isGameOver = (collectedPartsCount == RealBubbleCount);

		if (isGameOver) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 2) {
				timeElapsed = 0;
				isGameOver = false;
				collectedPartsCount += 99; // to stop GameOver triggering more than once
				//StartCoroutine(AnimateTuffy ());

				foreach (GameObject button in gameOverButtons) {
					button.SetActive (true);
				}

				soundPlayer.PlayGameOverSounds ();
			}
		}
	}

	Bubble CreateBubble(string sprite, int divideBySize) {
		Vector3 pos;
		do {
			pos = new Vector3 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
				(float)((Random.value - 0.5f) * canvasDimensions.height), 
				Random.value);
		} while (Vector3.Distance(pos, items[itemIndex].transform.localPosition) < 70 || menuButtonBounds.Contains(pos));

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
			var rb = bubbles [i].GetComponent<Rigidbody2D> ();
			rb.velocity = new Vector2(rb.velocity.x * 0.95f * Time.deltaTime, rb.velocity.y * 0.90f);

			var pos = bubbles [i].transform.localPosition;
			if (pos.x < -canvasDimensions.width / 2) {
				bubbles [i].transform.localPosition = (new Vector2 (-canvasDimensions.width / 2, pos.y));
			} else if (pos.x > canvasDimensions.width / 2) {
				bubbles [i].transform.localPosition = (new Vector2 (canvasDimensions.width / 2, pos.y));
			} else if (pos.y < -canvasDimensions.height / 2) {
				bubbles [i].transform.localPosition = (new Vector2 (pos.x, -canvasDimensions.height / 2));
			} else if (pos.y > canvasDimensions.height / 2) {
				bubbles [i].transform.localPosition = (new Vector2 (pos.x, canvasDimensions.height / 2));
			}
		}
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
		foreach (var b in bubbles) {
			b.GetComponent<Rigidbody2D>().AddExplosionForce (30, bubble.transform.position, 3);
		}


		var sprite = bubbleObjects [currentIndex].Sprite;
		if (sprite.Contains ("Item" + (itemIndex + 1)) || sprite.Equals ("Item1/Part1") && !isTuffyVariant) { // Correct
			if (touchCount % 2 == 0 && collectedPartsCount + 1 != RealBubbleCount) {
				soundPlayer.PlayCorrectAnswerSounds ();
			} else if (collectedPartsCount + 1 == RealBubbleCount) {
				StartCoroutine (AnimateTuffy ());
			} else {
				soundPlayer.PlayPopSound(1);
			}
			// Logic for right
			var clickedObject = bubbles [currentIndex];
			bubbles.Remove (clickedObject);
			bubbleObjects.RemoveAt (currentIndex);
			Destroy (clickedObject);

			var part = items [itemIndex].transform.Find ("Part" + sprite [sprite.Length - 1]).gameObject;
			//part.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
			part.GetComponent<SpriteRenderer>().material = defaultMaterial;

			touchCount++;
			collectedPartsCount++;
		} else {
			soundPlayer.PlayPopSound(1);
			bubbles.Remove (bubble);
			Destroy (bubble);
			bubbleObjects.RemoveAt(currentIndex);
			if (!sprite.Equals ("Bubble")) {
				StartCoroutine(AddBubbles ());
			}
		} 

		clickOccurred = true;

		StartCoroutine (FadeInstructions());
	}

	IEnumerator AddBubbles() {
		for (int i = 0; i < 20; i++) {
			var newBubble = CreateBubble("Bubble", (int)randomNormal(8, 3, 3, 20));
			bubbleObjects.Add (newBubble);
			bubbles.Add (newBubble.bubble);
			soundPlayer.PlayPopSound (1f);
			yield return null;
		}
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
		for (int i = 0; i < width / 2; i++) {
			var pos = tuffy.transform.localPosition;
			tuffy.transform.localPosition = new Vector3 (pos.x - 2, pos.y + (i % 2 == 0 ? 1 : 1), -0.01f);
			yield return null;
		}
	}

	void PlayBackgroundMusic() {
		var clip = soundPlayer.PlayBackgroundMusic ();
		Invoke ("PlayBackgroundMusic", clip.length + 1);
	}
}
