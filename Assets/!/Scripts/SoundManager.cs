using System;
using System.Linq;
using UnityEngine;

namespace SoundNS
{
	public class SoundManager : MonoBehaviour
	{
		private const string SOUND_NAME_UI_BUTTON_CLICK = "ButtonClick";
		private const string SOUND_NAME_UI_SLIDER_CHANGE = "SliderChange";
		private const float CAN_PLAY_AGAIN_AFTER_TIME = 0.1f;

		private static SoundManager instance;
		public static SoundManager Instance { get => instance; }

		[Serializable]
		private class Sound
		{
			public string Name;
			public AudioSource AudioSourceReference;
			[HideInInspector]
			public float TimeSinceLastPlay;
		}

		[SerializeField]
		private Sound[] sounds;

		private void Awake()
		{
			if (instance == null)
				instance = this;
			else
				Destroy(this);
		}
		public void PlayUIButtonClick()
		{
			Sound s = FindSound(SOUND_NAME_UI_BUTTON_CLICK);
			PlaySoundOneShot(s);
		}
		public void PlayUISliderChange()
		{
			Sound s = FindSound(SOUND_NAME_UI_SLIDER_CHANGE);
			PlaySoundOneShot(s);
		}
		private Sound FindSound(string soundName)
		{
			Sound sound = sounds.FirstOrDefault(x => x.Name == soundName);
			if (sound == null)
			{
#if UNITY_EDITOR
				Debug.Log("There is no such sound!");
#endif
			}
			return sound;
		}
		private void PlaySoundOneShot(Sound sound)
		{
			float currentTime = Time.timeSinceLevelLoad;
			if (!sound.AudioSourceReference.isPlaying || currentTime - sound.TimeSinceLastPlay > CAN_PLAY_AGAIN_AFTER_TIME)
			{
				sound.AudioSourceReference.PlayOneShot(sound.AudioSourceReference.clip);
				sound.TimeSinceLastPlay = currentTime;
			}
		}
	}
}