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

	GameObject[] walls;

	private Sprite wallSprite;

	private float spriteWidth;

	private List<GameObject> bubbles;
	private List<Bubble> bubbleObjects;

	private int touchCount;
	private int previousIndex;
	private int currentIndex;

	private float timeElapsed;

	private bool startTimer;
	private bool startGameOverTimer;

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

	// Use this for initialization
	void Start () {
		wallSprite = Resources.Load<Sprite> ("Square");
		spriteWidth = wallSprite.bounds.size.x;
		canvasDimensions = canvas.GetComponent<RectTransform> ().rect;

		audioSource = GetComponent<AudioSource> ();
		popSounds = Resources.LoadAll ("Sounds", typeof(AudioClip));

		/*walls = new GameObject[4];
		for (int i = 0; i < walls.Length; i++) {
			walls[i] = MakeWall(i);
		}*/

		bubbles = new List<GameObject>();
		bubbleObjects = new List<Bubble> ();
		for (int i = 0; i < BubbleCount; i++) {
			string j = (i / 3 < 4) ? "" + i / 3 : "";
			bubbleObjects.Add(new Bubble (j, new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), (float)(Random.value * canvasDimensions.height * -2 - canvasDimensions.height / 2)), new Vector2 ((float)(Random.value * 5 - 2.5), (float)(Random.value * 4 + 1)), canvas, (int) (Random.value * 3 + 7)));
			bubbles.Add(bubbleObjects[i].bubble);
		}

		time = 0;

		touchCount = 0;
		previousIndex = 0;
		currentIndex = -1;

		timeElapsed = 0;
		startTimer = false;
		startGameOverTimer = false;

		poppedPosition = -canvasDimensions.width / 2 + canvasDimensions.width / BubbleCount;
	}
	
	// Update is called once per frame
	void Update () {
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
				if (!startTimer) {
					HandleInteraction (bubble);
				}
			}
		}

		time += Time.deltaTime;
		for (int i = 0; i < bubbles.Count; i++) {
			var testBody = bubbles[i].GetComponent<Rigidbody2D> ();
			var vel = testBody.velocity;
			if (i != currentIndex) {
				vel.x = bubbleObjects [i].Amplitude * Mathf.Sin (time * bubbleObjects [i].Period);
			}
			testBody.velocity = vel;

			if (bubbles[i].transform.localPosition.y > canvasDimensions.height / 2) {
				bubbles[i].transform.localPosition = new Vector2 ((float)(Random.value * canvasDimensions.width - canvasDimensions.width / 2), -5 * canvasDimensions.height / 2);
			}
		}
		//Debug.Log (testBody.velocity.x);

		/*foreach (var bubble in bubbles) {
			var rigidbody = bubble.GetComponent<Rigidbody2D> ();
			var vel = rigidbody.velocity;

			if (vel.x > 0) {
				vel.x = Mathf.Min (vel.x, 4);
			} else {
				vel.x = Mathf.Max (vel.x, -4);
			}

			if (vel.y > 0) {
				vel.y = Mathf.Min (vel.y, 4);
			} else {
				vel.y = Mathf.Max (vel.y, -4);
			}
			rigidbody.velocity = vel;
		}*/

		if (startTimer) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 1) {
				timeElapsed = 0;
				startTimer = false;
				HandleTwoClicks ();
			}
		}

		if (bubbleObjects.Count == BubbleCount - RealBubbleCount) {
			startGameOverTimer = true;
		}

		if (startGameOverTimer) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed >= 1) {
				timeElapsed = 0;
				startGameOverTimer = false;
				bubbleObjects.Add (null);

				gameOverText = new GameObject ();
				gameOverText.transform.SetParent (canvas);
				gameOverText.transform.localPosition = new Vector2 (0, 80);
				gameOverText.transform.localScale = new Vector3 (1, 1, 1);

				var text = gameOverText.AddComponent<Text> ();
				text.text = "Play Again?";
				text.color = Color.black;
				text.fontSize = 24;
				text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
				text.alignment = TextAnchor.MiddleCenter;

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
				text.color = Color.black;
				text.fontSize = 21;
				text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
				text.alignment = TextAnchor.MiddleCenter;

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
				text.color = Color.black;
				text.fontSize = 21;
				text.font = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
				text.alignment = TextAnchor.MiddleCenter;

				rect = noButton.GetComponent<RectTransform> ();
				rect.sizeDelta = new Vector2 (200, 100);
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

		if (bubbleObjects [currentIndex].MatchingID.Equals ("")) {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.red;
			audioSource.PlayOneShot ((AudioClip)popSounds [(int)(Random.value * popSounds.Length)]);
		} else {
			bubbles [currentIndex].GetComponent<SpriteRenderer> ().color = Color.green;
		}

		startTimer = true;
	}

	void HandleTwoClicks() {
		if (!bubbleObjects[currentIndex].MatchingID.Equals("")) {
			var temp1 = bubbles [currentIndex];
			bubbles.Remove (temp1);
			bubbleObjects.RemoveAt (currentIndex);
			//Destroy (temp1);
			//Destroy (temp2);
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

	// 0: left, 1: bottom, 2: right, 3: top
	GameObject MakeWall(int position) {
		GameObject wall = new GameObject ();
		var rigidbody = wall.AddComponent<Rigidbody2D>();
		rigidbody.bodyType = RigidbodyType2D.Static;
		wall.AddComponent<BoxCollider2D> ();
		var spriteRenderer = wall.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = wallSprite;
		wall.transform.SetParent (canvas);

		if (position % 2 == 0) {
			wall.transform.localScale = new Vector2 (20, canvasDimensions.height / spriteWidth);
			wall.transform.localPosition = new Vector2((position - 1) * canvasDimensions.width / 2, 0);
		} else {
			wall.transform.localScale = new Vector2 (canvasDimensions.width / spriteWidth, 20);
			wall.transform.localPosition = new Vector2(0, (position - 2) * canvasDimensions.height / 2);
		}
		return wall;
	}
}
