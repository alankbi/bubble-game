using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble {
	
	public string MatchingID { get; private set; }

	public float Period { get; private set; }

	public float Amplitude { get; private set; }

	public GameObject bubble { get; set; }

	public Bubble(string matchingID, Vector2 pos, Vector2 vel, Transform canvas, int divideBySize)
	{
		MatchingID = matchingID;

		var bubble = new GameObject ();
		bubble.transform.SetParent (canvas);

		var rigidbody = bubble.AddComponent<Rigidbody2D>();
		rigidbody.gravityScale = 0;
		rigidbody.sharedMaterial = Resources.Load<PhysicsMaterial2D> ("Bouncy");
		bubble.transform.localPosition = pos;
		rigidbody.velocity = vel;

		var canvasDimensions = canvas.GetComponent<RectTransform> ().rect;
		rigidbody.transform.localScale = new Vector2 (canvasDimensions.height / divideBySize, canvasDimensions.height / divideBySize);

		bubble.AddComponent<CircleCollider2D> ();
		bubble.layer = 0;
		Physics2D.IgnoreLayerCollision (0, 0);

		var spriteRenderer = bubble.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("Bubble" + matchingID);

		this.bubble = bubble;

		Period = Random.value * 8;
		Amplitude = Random.value * rigidbody.velocity.y;

	}
}
