using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    private static MusicPlayer _instance;
    public static MusicPlayer Instance { get { return _instance; } }

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip battleMusic;

    private AudioClip desiredClip;

    private AudioSource audioSource;

    private Coroutine playingOtherSong;

    private void Awake()
    {
        _instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        GameManager.Instance.GameStateChanged += GameManager_GameStateChanged;
	}

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        if(e.newState == GameStates.BATTLING)
        {
            desiredClip = battleMusic;
            if(playingOtherSong == null)
            {
                audioSource.clip = battleMusic;
                audioSource.Play();
            }
        } else if(e.newState == GameStates.MAIN_MENU || e.newState == GameStates.BATTLE_ENDED)
        {
            if(audioSource.clip == menuMusic) { return; }
            desiredClip = menuMusic;
            if (playingOtherSong == null)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
            }
        }
    }

    public void PlayOnce(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        playingOtherSong = StartCoroutine(PlayThenReturnToDesired(clip.length));
    }

    public void Play(AudioClip clip)
    {
        StopPlayingOtherCoroutine();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void ReturnToDesired()
    {
        StopPlayingOtherCoroutine();
        audioSource.clip = desiredClip;
        audioSource.Play();
    }

    private void StopPlayingOtherCoroutine()
    {
        if(playingOtherSong != null)
        {
            StopCoroutine(playingOtherSong);
            playingOtherSong = null;
        }
    }

    private IEnumerator PlayThenReturnToDesired(float clipDuration)
    {
        yield return new WaitForSeconds(clipDuration);

        audioSource.clip = desiredClip;
        audioSource.Play();

        playingOtherSong = null;
    }
}
