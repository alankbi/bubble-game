using UnityEngine;

public class HoverEffect : MonoBehaviour
{
	private Game game;
	private Sprite objectSprite;
	private float bubbleColliderScale;
	private float objectColliderScale;
	private Bubble bubbleObject;
	private GameObject childSprite;
	private GameObject bubbleBackground;

	void Start() {
		game = GameObject.Find("Main Camera").GetComponent<Game>();

		var index = -1;
		var bubbles = game.bubbles;
		for (int i = 0; i < bubbles.Count; i++) {
			if (gameObject == bubbles [i]) {
				index = i;
				break;
			}
		}

		bubbleObject = (index == -1 ? null : game.bubbleObjects [index]);

		if (bubbleObject != null) {
			objectSprite = (Sprite)Resources.Load ("BubbleSprites/" + bubbleObject.Sprite + (bubbleObject.Sprite.Contains("pic") ? "" : "t"), typeof(Sprite));
			childSprite = Instantiate (bubbleObject.bubble, bubbleObject.bubble.transform);
			childSprite.transform.localPosition = new Vector3 (0, 0, 0);
			childSprite.GetComponent<SpriteRenderer> ().sprite = objectSprite;
			Destroy (childSprite.GetComponent<CircleCollider2D> ());
			childSprite.transform.localScale = new Vector2(0.15f, 0.15f);
			childSprite.SetActive (false);
			childSprite.GetComponent<SpriteRenderer> ().sortingOrder = (int)(bubbleObject.bubble.transform.localPosition.z * -10000);

			bubbleBackground = Instantiate(bubbleObject.bubble, bubbleObject.bubble.transform);
			bubbleBackground.transform.localPosition = new Vector3 (0, 0, 0);
			bubbleBackground.transform.localScale = new Vector2 (1, 1);
			Destroy (bubbleBackground.GetComponent<CircleCollider2D> ());
			foreach (Transform transform in bubbleBackground.transform) {
				Destroy (transform.gameObject);
			}
			bubbleBackground.GetComponent<SpriteRenderer> ().sortingOrder = (int) (bubbleObject.bubble.transform.localPosition.z * -10000) - 1;
			bubbleObject.bubble.GetComponent<SpriteRenderer> ().sprite = null;
		}
	}

	void OnMouseOver()
	{
		if (bubbleObject != null) {
			childSprite.SetActive(true);
		}
	}

	void OnMouseExit()
	{
		if (bubbleObject != null) {
			childSprite.SetActive (false);
		}
	}
}