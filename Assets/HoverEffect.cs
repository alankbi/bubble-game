using UnityEngine;

public class HoverEffect : MonoBehaviour
{
	private Game game;
	private Sprite bubbleSprite;
	private SpriteRenderer spriteRenderer;
	private Vector3 bubbleScale;
	private Vector3 objectScale;
	private float bubbleColliderScale;
	private float objectColliderScale;
	private Bubble bubbleObject;

	void Start() {
		game = GameObject.Find("Main Camera").GetComponent<Game>();
		bubbleSprite = (Sprite)Resources.Load ("BubbleSprites/Bubble", typeof(Sprite));
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		bubbleScale = gameObject.transform.localScale;
		objectScale = gameObject.transform.localScale / 5;
		bubbleColliderScale = gameObject.GetComponent<CircleCollider2D>().radius;
		objectColliderScale = bubbleColliderScale * 5;

		var index = -1;
		var bubbles = game.bubbles;
		for (int i = 0; i < bubbles.Count; i++) {
			if (gameObject == bubbles [i]) {
				index = i;
				break;
			}
		}
		bubbleObject = (index == -1 ? null : game.bubbleObjects [index]);
	}

	void OnMouseOver()
	{
		if (bubbleObject != null) {
			var sprite = bubbleObject.Sprite;
			spriteRenderer.sprite = (Sprite)Resources.Load ("BubbleSprites/" + sprite + (sprite.Contains("Part") ? "t" : ""), typeof(Sprite));
			gameObject.transform.localScale = objectScale;
			gameObject.GetComponent<CircleCollider2D>().radius = objectColliderScale;
		}
	}

	void OnMouseExit()
	{
		spriteRenderer.sprite = bubbleSprite;
		gameObject.transform.localScale = bubbleScale;
		gameObject.GetComponent<CircleCollider2D>().radius = bubbleColliderScale;
	}
}