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

	private const int BubbleCount = 60;
	private const int RealBubbleCount = 12;

	private GameObject gameOverText;
	private GameObject yesButton;
	private GameObject noButton;

	private GameObject test;
	private float time;

	private AudioSource audioSource;
	private Object[] popSounds;

	private readonly Color buttonColor = new Color (249, 123, 40, 255);

	// Use this for initialization
	void Start () {
		Debug.Log (buttonColor);
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;

		audioSource = GetComponent<AudioSource> ();
		popSounds = Resources.LoadAll ("Sounds/BubblePop", typeof(AudioClip));

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();

		for (int i = 0; i < BubbleCount; i++) {
			string sprite = (i / 3 < 4) ? "Object" + i / 3 : "Bubble";
			bubbleObjects.Add(new Bubble (sprite, 
				new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
					(float)(Random.value * canvasDimensions.height * -2 - canvasDimensions.height / 2)), 
				new Vector2 ((float)(Random.value * 5 - 2.5), (float)(Random.value * 4 + 1)), 
				canvas, 
				(int) (Random.value * 3 + 7)));
			bubbles.Add(bubbleObjects[i].bubble);
		}

		time = 0;

		touchCount = 0;
		currentIndex = -1;

		timeElapsed = 0;
		clickOccurred = false;
		isGameOver = false;

		poppedPosition = -canvasDimensions.width / 2 + canvasDimensions.width / BubbleCount;
	}
	
	// Update is called once per frame
	void Update () {
		CheckClicks ();

		HandleBubbleMotion ();

		if (clickOccurred) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 1) {
				timeElapsed = 0;
				clickOccurred = false;
				HandleClickOver ();
			}
		}

		isGameOver = (bubbleObjects.Count == BubbleCount - RealBubbleCount);

		if (isGameOver) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 1) {
				timeElapsed = 0;
				isGameOver = false;
				bubbleObjects.Add (null);

				ConstructGameOverScreen ();
			}
		}
	}

	void CheckClicks() {
		var hit = new RaycastHit2D();
		for (int i = 0; i < Input.touchCount; i++) {
			if (Input.GetTouch (i).phase.Equals (TouchPhase.Began)) {
				var ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);
				hit = Physics2D.Raycast (ray.origin, ray.direction);
				if (hit) {
					var bubble = hit.transform.gameObject;
					HandleInteraction (bubble);
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction);
			if (hit) {
				var bubble = hit.transform.gameObject;
				if (!clickOccurred) {
					HandleInteraction (bubble);
				}
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

			if (bubbles[i].transform.localPosition.y > canvasDimensions.height / 2) {
				bubbles[i].transform.localPosition = 
					new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), -5 * canvasDimensions.height / 2);
			}
		}
	}

	void HandleInteraction(GameObject bubble) {
		bool found = false;
		for (int i = 0; i < bubbles.Count; i++) {
			if (bubble == bubbles[i]) {
				currentIndex = i;
				found = true;
				break;
			}
		}

		if (!found)
			return;

		touchCount++;

		var rb = bubbles [currentIndex].GetComponent<Rigidbody2D> ();
		rb.velocity = new Vector2(0, 0);

		if (bubbleObjects [currentIndex].Sprite.Equals ("Bubble")) {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.red;
			audioSource.PlayOneShot ((AudioClip)popSounds [(int)(Random.value * popSounds.Length)]);
		} else {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.green;
		}

		clickOccurred = true;
	}

	void HandleClickOver() {
		if (!bubbleObjects[currentIndex].Sprite.Equals("Bubble")) {
			var temp1 = bubbles [currentIndex];
			bubbles.Remove (temp1);
			bubbleObjects.RemoveAt (currentIndex);
			temp1.transform.localPosition = new Vector3(poppedPosition, -canvasDimensions.height / 2 + 20, 1);
			temp1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			Destroy (temp1.GetComponent<Collider2D> ());
			poppedPosition += canvasDimensions.width / (RealBubbleCount);
		} else {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.white;
			var rb = bubbles [currentIndex].GetComponent<Rigidbody2D> ();
			rb.velocity = new Vector2 ((float)(Random.value * 5 - 2.5), (float)(Random.value * 4 + 1));
		}
		currentIndex = -1;
	}

	void ConstructGameOverScreen() {
		gameOverText = new GameObject ();
		gameOverText.transform.SetParent (canvas);
		gameOverText.transform.localPosition = new Vector2 (0, 80);
		gameOverText.transform.localScale = new Vector3 (1, 1, 1);

		var text = gameOverText.AddComponent<Text> ();
		text.text = "Play Again?";
		text.color = buttonColor;
		text.fontSize = 24;
		text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		TextSharpener.SharpenText (text);

		var rect = gameOverText.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (200, 100);

		yesButton = new GameObject ();
		yesButton.transform.SetParent (canvas);
		yesButton.transform.localPosition = new Vector2 (0, 0);
		yesButton.transform.localScale = new Vector3 (1, 1, 1);

		var button = yesButton.AddComponent<Button> ();
		button.onClick.AddListener(() => SceneManager.LoadScene (1));

		text = yesButton.AddComponent<Text> ();
		text.text = "Yes";
		text.color = buttonColor;
		text.fontSize = 21;
		text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		TextSharpener.SharpenText (text);

		rect = yesButton.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (200, 100);

		noButton = new GameObject ();
		noButton.transform.SetParent (canvas);
		noButton.transform.localPosition = new Vector2 (0, -80);
		noButton.transform.localScale = new Vector3 (1, 1, 1);

		button = noButton.AddComponent<Button> ();
		button.onClick.AddListener(() => SceneManager.LoadScene (0));

		text = noButton.AddComponent<Text> ();
		text.text = "No";
		text.color = buttonColor;
		text.fontSize = 21;
		text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		TextSharpener.SharpenText (text);

		rect = noButton.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector2 (200, 100);
	}
}
