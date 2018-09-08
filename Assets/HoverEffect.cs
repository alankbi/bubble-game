using UnityEngine;

public class HoverEffect : MonoBehaviour
{
	private Game game;
	private Sprite objectSprite;
	private Sprite bubbleSprite;
	private SpriteRenderer spriteRenderer;
	private Vector3 bubbleScale;
	private Vector3 objectScale;
	private float bubbleColliderScale;
	private float objectColliderScale;
	private Bubble bubbleObject;
	private GameObject childSprite;
	private GameObject bubbleBackground;

	void Start() {
		game = GameObject.Find("Main Camera").GetComponent<Game>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		bubbleScale = gameObject.transform.localScale;
		objectScale = gameObject.transform.localScale / 5;
		bubbleSprite = (Sprite)Resources.Load ("BubbleSprites/Bubble", typeof(Sprite));
		//bubbleColliderScale = gameObject.GetComponent<CircleCollider2D>().radius;
		//objectColliderScale = bubbleColliderScale * 5;

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
			childSprite.transform.localPosition = new Vector3 (0, 0, 0);//-0.01f);
			childSprite.GetComponent<SpriteRenderer> ().sprite = objectSprite;
			Destroy (childSprite.GetComponent<CircleCollider2D> ());
			childSprite.transform.localScale = new Vector2(0.15f, 0.15f);
			childSprite.SetActive (false);

			bubbleBackground = Instantiate(bubbleObject.bubble, bubbleObject.bubble.transform);
			bubbleBackground.transform.localPosition = new Vector3 (0, 0, 0.5f);
			bubbleBackground.transform.localScale = new Vector2 (1, 1);
			Destroy (bubbleBackground.GetComponent<CircleCollider2D> ());
			foreach (Transform transform in bubbleBackground.transform) {
				Destroy (transform.gameObject);
			}
			childSprite.transform.SetAsFirstSibling ();
			bubbleObject.bubble.GetComponent<SpriteRenderer> ().sprite = null;
		}
	}

	void OnMouseOver()
	{
		if (bubbleObject != null) {
			//var sprite = bubbleObject.Sprite;
			//spriteRenderer.sprite = (Sprite)Resources.Load ("BubbleSprites/" + sprite + (sprite.Contains("Part") ? "t" : ""), typeof(Sprite));
			//gameObject.transform.localScale = objectScale;
			//gameObject.GetComponent<CircleCollider2D>().radius = objectColliderScale;

			childSprite.transform.localPosition = new Vector3 (0, 0, -0.01f);
			bubbleBackground.transform.localPosition = new Vector3 (0, 0, 5000f);
			childSprite.SetActive(true);

		}
	}

	void OnMouseExit()
	{
		if (bubbleObject != null) {
			childSprite.SetActive (false);

			//spriteRenderer.sprite = bubbleSprite;
			//gameObject.transform.localScale = bubbleScale;
			//gameObject.GetComponent<CircleCollider2D>().radius = bubbleColliderScale;
		}
	}
}