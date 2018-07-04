using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer {

	private AudioSource audioSource;
	private Object[] popSounds;
	private Object[] correctAnswerSounds;
	private Object[] gameOverSounds;
	private Object startSound;

	public SoundPlayer(AudioSource audioSource) {
		this.audioSource = audioSource; 
		popSounds = Resources.LoadAll ("Sounds/BubblePop", typeof(AudioClip));
		correctAnswerSounds = Resources.LoadAll ("Sounds/CorrectAnswer", typeof(AudioClip));
		gameOverSounds = Resources.LoadAll ("Sounds/GameOver", typeof(AudioClip));
		startSound = Resources.Load ("Sounds/start", typeof(AudioClip));
	}

	public void PlayPopSound() {
		audioSource.PlayOneShot ((AudioClip)popSounds [(int)(Random.value * popSounds.Length)]);
	}

	public void PlayCorrectAnswerSounds() {
		audioSource.PlayOneShot ((AudioClip)correctAnswerSounds [(int)(Random.value * correctAnswerSounds.Length)]);
	}

	public void PlayGameOverSounds() {
		audioSource.PlayOneShot ((AudioClip)gameOverSounds [(int)(Random.value * gameOverSounds.Length)]);
	}

	public void PlayStartSound() {
		audioSource.PlayOneShot ((AudioClip)startSound);
	}
}
