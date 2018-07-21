using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble {
	
	public string Sprite { get; private set; }

	public float Period { get; private set; }

	public float Amplitude { get; private set; }

	public GameObject bubble { get; set; }

	public Bubble(string sprite, Vector3 pos, Vector2 vel, Transform canvas, int divideBySize) {
		Sprite = sprite;

		var bubble = new GameObject ();
		bubble.transform.SetParent (canvas);

		var rigidbody = bubble.AddComponent<Rigidbody2D>();
		rigidbody.gravityScale = 0;
		rigidbody.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("Bouncy");
		bubble.transform.localPosition = pos;
		rigidbody.velocity = vel;

		var canvasDimensions = canvas.GetComponent<RectTransform> ().rect;
		rigidbody.transform.localScale = 
			new Vector2 (canvasDimensions.height / divideBySize, canvasDimensions.height / divideBySize);

		var collider = bubble.AddComponent<CircleCollider2D> ();
		if (sprite.Contains ("Item")) {
			collider.radius *= 10;
		}
		bubble.layer = 0;
		Physics2D.IgnoreLayerCollision (0, 0);

		var spriteRenderer = bubble.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("BubbleSprites/" + sprite);

		this.bubble = bubble;

		Period = Random.value * 4;
		Amplitude = Random.value * Mathf.Min(rigidbody.velocity.y, 5);

	}
}
