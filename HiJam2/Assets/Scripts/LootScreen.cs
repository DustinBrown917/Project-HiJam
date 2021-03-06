﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootScreen : MonoBehaviour {

    [SerializeField] Text mainText;
    [SerializeField] Text spaceText;
    [SerializeField] Text scoreText;
    [SerializeField] private PartyMemberDisplay[] pmDisplays;
    [SerializeField] Slider timeSlider;
    [SerializeField] Slider rewardSlider;
    [SerializeField] private Image rewardImage;
    [SerializeField] private Sprite newPartyMemberSprite;
    [SerializeField] private Sprite timesTwoSprite;
    [SerializeField] private float spamTime = 10;
    [SerializeField] private int clicksForReward = 60;
    [SerializeField] private AudioClip countDownSound;
    [SerializeField] private AudioClip spaceBarSound;
    [SerializeField] private AudioClip timeUpWin;
    [SerializeField] private AudioClip timeUpLose;
    [SerializeField] private AudioClip spamMusic;

    private Image sliderImage;
    private HumanPlayer humanPlayer;
    private int score = 0;
    private AudioSource audioSource;

    private void Awake()
    {
        sliderImage = timeSlider.GetComponentInChildren<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        score = 0;
        scoreText.text = "0 exp";
        spaceText.enabled = false;
        timeSlider.value = 1;
        timeSlider.gameObject.SetActive(false);
        sliderImage.color = Color.green;
        mainText.text = "Ready?";
        rewardSlider.value = 0;

        if (humanPlayer == null) { humanPlayer = (HumanPlayer)GameManager.Instance.HumanPlayer; }

        for(int i =0; i < pmDisplays.Length; i++)
        {
            if(i >= humanPlayer.CurrentPartySize)
            {
                pmDisplays[i].gameObject.SetActive(false);
            } else
            {
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].AssignTroop(humanPlayer.GetTroop(i));
            }
        }

        StartCoroutine(ExperienceEvent());
    }


    private IEnumerator ExperienceEvent()
    {
        Vector3 spaceTextScale = new Vector3(1,1,1);
        bool rewardWon = false;
        mainText.text = "Ready?";
        rewardImage.color = new Color(1, 1, 1, 0.5f);

        if(humanPlayer.CurrentPartySize < Player.MAX_PARTY_SIZE)
        {
            rewardImage.sprite = newPartyMemberSprite;
        } else
        {
            rewardImage.sprite = timesTwoSprite;
        }

        audioSource.clip = countDownSound;

        yield return new WaitForSeconds(1.5f);

        float waitTime = 3.0f;

        while(waitTime > 0)
        {
            audioSource.Play();
            mainText.text = waitTime.ToString();
            StartCoroutine(PopObject(mainText.gameObject, 2.0f, 0.8f));
            waitTime--;
            yield return new WaitForSeconds(1.0f);
        }

        mainText.text = "GO!";

        MusicPlayer.Instance.Play(spamMusic);

        spaceText.enabled = true;
        timeSlider.gameObject.SetActive(true);
        audioSource.clip = spaceBarSound;
        float time = spamTime;
        while(time > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                score++;
                audioSource.Play();
                StartCoroutine(PopObject(scoreText.gameObject, 1.3f, 0.3f, Vector3.one));
                rewardSlider.value = (float)score / (float)clicksForReward;
                if(rewardSlider.value == 1.0f && !rewardWon)
                {
                    rewardImage.color = new Color(1, 1, 1, 1);
                    StartCoroutine(PopObject(rewardImage.gameObject, 1.5f, 0.5f));
                    rewardWon = true;
                }
                scoreText.text = score.ToString() + " exp";
            }

            timeSlider.value = time / spamTime;

            sliderImage.color = Color.Lerp(Color.red, Color.green, time / spamTime);

            spaceText.transform.localScale = spaceTextScale * (((Mathf.Sin(Time.realtimeSinceStartup * 10.0f)+1) * 0.2f) + 0.5f);

            time -= Time.deltaTime;
            yield return null;
        }

        mainText.text = "Done!";
        MusicPlayer.Instance.ReturnToDesired();
        spaceText.enabled = false;
        timeSlider.gameObject.SetActive(false);

        if (rewardSlider.value >= 1)
        {
            audioSource.clip = timeUpWin;
            if (humanPlayer.CurrentPartySize < Player.MAX_PARTY_SIZE)
            {
                
                audioSource.Play();
                GameMenu.Instance.GiveNewPartyMember = true;
            }
            else
            {
                score *= 2;
                scoreText.text = score.ToString() + " exp";
            }
        } else
        {
            audioSource.clip = timeUpLose;
        }

        yield return new WaitForSeconds(1.0f);

        for(int i = 0; i < pmDisplays.Length; i++)
        {
            if (pmDisplays[i].gameObject.activeSelf)
            {
                pmDisplays[i].GiveExperienceToTroop(score);
            }
        }

        yield return new WaitForSeconds((Time.deltaTime * score) + 1.0f);

        OnExperienceEventDone();
    }

    private IEnumerator PopObject(GameObject obj, float popSize, float popTime)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.localScale = obj.transform.localScale * popSize;

        float t = popTime;

        while (t > 0)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, originalScale * popSize, t / popTime);
            t -= Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = originalScale;
    }

    private IEnumerator PopObject(GameObject obj, float popSize, float popTime, Vector3 originalScale)
    {
        obj.transform.localScale = obj.transform.localScale * popSize;

        float t = popTime;

        while (t > 0)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, originalScale * popSize, t / popTime);
            t -= Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = originalScale;
    }


    public event EventHandler ExperienceEventDone;

    private void OnExperienceEventDone()
    {
        EventHandler handler = ExperienceEventDone;

        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
