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
	private const int RealBubbleCount = 12;

	public GameObject[] gameOverButtons;

	private GameObject test;
	private float time;

	private AudioSource audioSource;
	private SoundPlayer soundPlayer;

	private readonly Color TextColor = new Color (249f / 255, 123f / 255, 40f / 255, 1f);

	public Text instructions;
	public GameObject tuffy;

	// Use this for initialization
	void Start () {
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;

		audioSource = GetComponent<AudioSource> ();
		//soundPlayer = new SoundPlayer (audioSource);

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();

		for (int i = 0; i < BubbleCount; i++) {
			string sprite = (i < RealBubbleCount) ? "Object" + (i / 3) : "Bubble"; // Get rid of i / 3 later
			int divideBySize = (i < RealBubbleCount) ? 9 : (int) (Random.value * 5 + 6);
			var pos = new Vector3 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
				(float)(Random.value * canvasDimensions.height * -2 - canvasDimensions.height / 2), 
				(i < RealBubbleCount) ? -0.0001f : 0);

			bubbleObjects.Add(new Bubble (sprite, 
				pos, 
				new Vector2 ((float)(Random.value * 5 - 2.5), randomNormal(3, 2, 1, 7)), 
				canvas, 
				divideBySize));
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

		//soundPlayer.PlayBackgroundBubbling ();

		float tuffyXOffset = tuffy.GetComponent<RectTransform> ().rect.width * tuffy.transform.localScale.x * 7 / 10;
		tuffy.transform.localPosition = new Vector2(canvasDimensions.width / 2 + tuffyXOffset, 0);
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

				foreach (GameObject button in gameOverButtons) {
					button.SetActive (true);
				}

				StartCoroutine(AnimateTuffy ());
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
					HandleClickStart (bubble);
				}
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			hit = Physics2D.Raycast (ray.origin, ray.direction);
			if (hit) {
				var bubble = hit.transform.gameObject;
				if (!clickOccurred) {
					HandleClickStart (bubble);
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

			if (bubbles[i].transform.localPosition.y > canvasDimensions.height / 2 + 20) {
				bubbles[i].transform.localPosition = 
					new Vector3 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), 
						-5 * canvasDimensions.height / 2, 
						bubbles[i].transform.localPosition.z);
			}
		}
	}

	void HandleClickStart(GameObject bubble) {
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

		var rb = bubbles [currentIndex].GetComponent<Rigidbody2D> ();
		rb.velocity = new Vector2(0, 0);

		if (bubbleObjects [currentIndex].Sprite.Equals ("Bubble")) {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.red;
			//soundPlayer.PlayPopSound(1);
		} else {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.green;
			if (touchCount % 2 == 0 && bubbleObjects.Count - 1 != BubbleCount - RealBubbleCount) {
				//soundPlayer.PlayCorrectAnswerSounds ();
			} else {
				//soundPlayer.PlayPopSound(1);
			}
			touchCount++;
		}

		clickOccurred = true;

		StartCoroutine (FadeInstructions());
	}

	void HandleClickOver() {
		if (!bubbleObjects[currentIndex].Sprite.Equals("Bubble")) {
			var clickedObject = bubbles [currentIndex];
			bubbles.Remove (clickedObject);
			bubbleObjects.RemoveAt (currentIndex);

			clickedObject.transform.localScale /= 1.2f;
			var scale = clickedObject.transform.localScale;
			clickedObject.transform.localPosition = 
				new Vector3(poppedPosition + scale.x / 2 + 10, -canvasDimensions.height / 2 + scale.y / 2 + 10, -0.01f);
			clickedObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			Destroy (clickedObject.GetComponent<Collider2D> ());

			poppedPosition += canvasDimensions.width / (RealBubbleCount) - 20 / RealBubbleCount;
		} else {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.white;
			var rb = bubbles [currentIndex].GetComponent<Rigidbody2D> ();
			rb.velocity = new Vector2 ((float)(Random.value * 5 - 2.5), randomNormal(3, 2, 1, 7));
		}

		if (bubbleObjects.Count == BubbleCount - RealBubbleCount) { // Last object found
			//soundPlayer.PlayGameOverSounds ();
		}
		currentIndex = -1;
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
