using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer {

	private AudioSource audioSource;
	private Object[] popSounds;
	private Object[] correctAnswerSounds;
	private Object[] gameOverSounds;
	private Object startSound;
	private Object bubblingSound;
	private Object[] backgroundMusic;

	private int index;
	private int backgroundMusicIndex;

	public SoundPlayer(AudioSource audioSource) {
		this.audioSource = audioSource; 
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

	public void PlayPopSound(float volume) {
		audioSource.PlayOneShot ((AudioClip)popSounds [(int)(Random.value * popSounds.Length)], volume);
	}

	public void PlayCorrectAnswerSounds() {
		if (index >= correctAnswerSounds.Length) {
			index = 0;
			Shuffle(correctAnswerSounds);
		}
		audioSource.PlayOneShot ((AudioClip)correctAnswerSounds [index++]);
	}

	public void PlayGameOverSounds() {
		audioSource.PlayOneShot ((AudioClip)gameOverSounds [(int)(Random.value * gameOverSounds.Length)]);
	}

	public void PlayStartSound() {
		audioSource.PlayOneShot ((AudioClip)startSound);
	}

	public void PlayBackgroundBubbling() {
		audioSource.Play ();
	}

	private void Shuffle(Object[] array) {
		int size = array.Length;
		while (size > 1) {
			int k = Random.Range(0, size--);
			Object temp = array[size];
			array[size] = array[k];
			array[k] = temp;
		}
	}

	public AudioClip PlayBackgroundMusic() {
		if (backgroundMusicIndex >= backgroundMusic.Length) {
			backgroundMusicIndex = 0;
			Shuffle (backgroundMusic);
		}
		var clip = (AudioClip)backgroundMusic [backgroundMusicIndex++];
		audioSource.PlayOneShot (clip, 0.4f);
		return clip;
	}
}
