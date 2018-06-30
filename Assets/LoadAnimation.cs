using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAnimation : MonoBehaviour {

	private Object[] frames;
	private const int framesPerSecond = 10;

	public GameObject animation;
	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		frames = Resources.LoadAll ("tuffy_frames1", typeof(Sprite));
		sr = animation.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		var index = (int) (Time.time * framesPerSecond) % frames.Length;
		sr.sprite = (Sprite) frames[index];
		Debug.Log (index);
	}
}
