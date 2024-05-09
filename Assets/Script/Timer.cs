using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text txtClock;
    private float _time;
    private float _totalTime;

    public bool IsStartCountDown { get; set; }
    public Action OnTimeOver { get; set; }

    public int DeltaTime => (int)TimeSpan.FromSeconds(_totalTime - _time).TotalMilliseconds;


    private void Update()
    {
        if (!IsStartCountDown)
        {
            return;
        }
        _time -= Time.deltaTime;
        if (_time <= 0)
        {
            IsStartCountDown = false;
            _time = 0;
            OnTimeOver?.Invoke();
        }
        UpdateTimeText();
    }

    public void SetTimer(float totalTime)
    {
        _totalTime = totalTime;
        _time = totalTime;
        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        var minute = Mathf.RoundToInt(_time) / 60;
        var second = Mathf.RoundToInt(_time) % 60;
        var strMinute = minute < 10 ? "0" + minute : minute.ToString();
        var strSecond = second < 10 ? "0" + second : second.ToString();

        txtClock.text = strMinute + ":" + strSecond;
    }
}
