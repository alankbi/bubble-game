using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {

	private static AudioSource audioSource;
	private static Object[] popSounds;
	private static Object[] correctAnswerSounds;
	private static Object[] gameOverSounds;
	private static Object startSound;
	private static Object bubblingSound;
	private static Object[] backgroundMusic;

	private static int index;
	private static int backgroundMusicIndex;

	void Start(){//public SoundPlayer(AudioSource audioSource) {
		audioSource = GetComponent<AudioSource>();//audioSource; 
		popSounds = Resources.LoadAll ("Sounds/BubblePop", typeof(AudioClip));
		correctAnswerSounds = Resources.LoadAll ("Sounds/CorrectAnswer", typeof(AudioClip));
		gameOverSounds = Resources.LoadAll ("Sounds/GameOver", typeof(AudioClip));
		startSound = Resources.Load ("Sounds/start", typeof(AudioClip));
		bubblingSound = Resources.Load ("Sounds/BackgroundBubbling/bubbling_long", typeof(AudioClip));
		backgroundMusic = Resources.LoadAll ("Sounds/BackgroundMusic", typeof(AudioClip));

		Shuffle (correctAnswerSounds);
		Shuffle (backgroundMusic);

		index = 0;
		backgroundMusicIndex = 0;
	}

	public static void PlayPopSound(float volume) {
		audioSource.PlayOneShot ((AudioClip)popSounds [(int)(Random.value * popSounds.Length)], volume);
	}

	public static void PlayCorrectAnswerSounds() {
		if (index >= correctAnswerSounds.Length) {
			index = 0;
			Shuffle(correctAnswerSounds);
		}
		audioSource.PlayOneShot ((AudioClip)correctAnswerSounds [index++]);
	}

	public static void PlayGameOverSounds() {
		audioSource.PlayOneShot ((AudioClip)gameOverSounds [(int)(Random.value * gameOverSounds.Length)]);
	}

	public static void PlayStartSound() {
		Debug.Log (audioSource);
		audioSource.PlayOneShot ((AudioClip)startSound);
	}

	public static void PlayBackgroundBubbling() {
		audioSource.clip = (AudioClip)bubblingSound;
		audioSource.Play ();
		PlayBackgroundMusic ();
	}

	private static void Shuffle(Object[] array) {
		int size = array.Length;
		while (size > 1) {
			int k = Random.Range(0, size--);
			Object temp = array[size];
			array[size] = array[k];
			array[k] = temp;
		}
	}

	public static void PlayBackgroundMusic() {
		if (backgroundMusicIndex >= backgroundMusic.Length) {
			backgroundMusicIndex = 0;
			Shuffle (backgroundMusic);
		}
		var clip = (AudioClip)backgroundMusic [backgroundMusicIndex++];
		audioSource.PlayOneShot (clip, 0.4f);
		//Invoke("PlayBackgroundMusic", clip.length + 1f); 
	}
}
