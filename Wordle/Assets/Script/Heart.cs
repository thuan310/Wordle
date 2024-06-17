using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private int maxHeart;
    [SerializeField] public int currentHeart;
    [SerializeField] private int rechargeSeconds;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI heartNumber;


    private DateTime nextLifeTime;
    private DateTime nextLifeFullTime;
    private TimeSpan rechargeTime;

    private void Awake()
    {
        rechargeTime = TimeSpan.FromSeconds(rechargeSeconds);
        heartNumber.text = currentHeart.ToString();
        SetTimer();
    }
    private void Update()
    {
        if (currentHeart < maxHeart && DateTime.Now >= nextLifeTime)
        {
            GainHeart();
            SetTimer();
        }

        UpdateTimerText();
    }


    public void GainHeart()
    {
        currentHeart++;
        heartNumber.text = currentHeart.ToString();
    }

    public void LoseHeart()
    {
        currentHeart--;
        if (currentHeart == maxHeart - 1) SetTimer();
        heartNumber.text = currentHeart.ToString();


    }

    private void SetTimer()
    {
        if (currentHeart < maxHeart)
        {
            nextLifeTime = DateTime.Now.Add(rechargeTime);
        }
    }
    private void UpdateTimerText()
    {
        if (currentHeart < maxHeart)
        {
            TimeSpan remainingTime = nextLifeTime - DateTime.Now;
            string timeRemainingString = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            timeText.text = timeRemainingString;
        }
        else timeText.text = "Full";

    }
}
