using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    private int score = 0;

    public int scoreBonus = 0;

    public AudioSource playerDeathAudio;

    private IEnumerator scoreBonusReset;


    public AudioMixer mixer;
    public AudioMixerSnapshot lowBonusSnapshot;
    public AudioMixerSnapshot highBonusSnapshot;

    private AudioMixerSnapshot[] snapshots;

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;

        snapshots = new AudioMixerSnapshot[]{
            lowBonusSnapshot, highBonusSnapshot
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        // reset score
        score = 0;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);

        ++scoreBonus;
        // Janky way to reset the score timer
        if (scoreBonusReset != null)
        {
            StopCoroutine(scoreBonusReset);
        }
        scoreBonusReset = DelayedResetScoreBonus();
        StartCoroutine(scoreBonusReset);

        mixer.TransitionToSnapshots(snapshots, genSnapshotWeights(), 0.0f);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    public void PlayDie()
    {
        playerDeathAudio.PlayOneShot(playerDeathAudio.clip);
    }

    IEnumerator DelayedResetScoreBonus()
    {
        yield return new WaitForSeconds(3.0f);
        ResetScoreBonus();
    }

    void ResetScoreBonus()
    {
        // Debug.Log("Score bonus resetting!");
        scoreBonus = 0;
        mixer.TransitionToSnapshots(snapshots, genSnapshotWeights(), 0.0f);
    }

    float[] genSnapshotWeights()
    {
        float highWeight = Math.Max(0.0f, Math.Min(7.0f, scoreBonus - 1) / 7.0f);
        float lowWeight = 1.0f - highWeight;

        // Debug.Log(lowWeight + " " + highWeight);

        return new float[]{
            lowWeight, highWeight
        };
    }
}