using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuestionVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnReplay;
    [SerializeField] private Button bigPlayBtn;
    [SerializeField] private Button bigPauseBtn;
    [SerializeField] private Button bigReplayBtn;
    [SerializeField] private Slider sliderTime;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private Timer timer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Button muteBtn;
    [SerializeField] private Button unMuteBtn;

    private float _oldVolume;

    public void Start()
    {
        sliderVolume.onValueChanged.AddListener(OnVolumeChange);
        btnPlay.onClick.AddListener(ContinuePlay);
        bigPlayBtn.onClick.AddListener(() => { btnPlay.onClick.Invoke(); });

        btnPause.onClick.AddListener(OnClickPause);
        bigPauseBtn.onClick.AddListener(() => { btnPause.onClick.Invoke(); });

        btnReplay.onClick.AddListener(StartPlay);
        bigReplayBtn.onClick.AddListener(() => { btnReplay.onClick.Invoke(); });

        videoPlayer.loopPointReached += OnVideoEnd;
        muteBtn.onClick.AddListener(OnClickMute);
        unMuteBtn.onClick.AddListener(OnClickUnMute);
    }


    private void OnClickMute()
    {
        muteBtn.gameObject.SetActive(false);
        unMuteBtn.gameObject.SetActive(true);
        _oldVolume = videoPlayer.GetDirectAudioVolume(0);
        sliderVolume.value = 0;
        videoPlayer.SetDirectAudioVolume(0, 0);
    }

    private void OnClickUnMute()
    {
        muteBtn.gameObject.SetActive(true);
        unMuteBtn.gameObject.SetActive(false);
        sliderVolume.value = _oldVolume;
        videoPlayer.SetDirectAudioVolume(0, _oldVolume);
    }

    private void OnVolumeChange(float value)
    {
        videoPlayer.SetDirectAudioVolume(0, value);
        if (value == 0)
        {
            muteBtn.gameObject.SetActive(false);
            unMuteBtn.gameObject.SetActive(true);
        }
        else
        {
            muteBtn.gameObject.SetActive(true);
            unMuteBtn.gameObject.SetActive(false);
        }
    }

    public async void Init(string url)
    {
        rawImage.gameObject.SetActive(false);
        videoPlayer.url = url;
        videoPlayer.Prepare();
        DisableAll();

        sliderTime.value = 0;
        videoPlayer.SetDirectAudioVolume(0, 1);
        sliderVolume.value = 1;
        timer.SetTimer(0);

        while (!videoPlayer.isPrepared)
        {
            await Task.Delay(100);
        }

        EnableAll();

        rawImage.gameObject.SetActive(true);
        timer.SetTimer((float)videoPlayer.length);
        _oldVolume = 1;
        OnClickUnMute();
        StartPlay();
    }

    private void StartPlay()
    {
        sliderTime.value = 0;
        timer.SetTimer((float)videoPlayer.length);
        sliderTime.value = 0;
        sliderTime.DOValue(1f, (float)videoPlayer.length);

        videoPlayer.Play();
        ShowBtnPause();

        timer.IsStartCountDown = true;
    }

    private void ContinuePlay()
    {
        // bigButtons.SetActive(false);
        sliderTime.DOPlayForward();
        videoPlayer.Play();
        ShowBtnPause();

        timer.IsStartCountDown = true;
    }

    private void OnClickPause()
    {
        videoPlayer.Pause();
        btnPause.gameObject.SetActive(false);
        sliderTime.DOPause();
        timer.IsStartCountDown = false;
        ShowBtnPlay();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        sliderTime.DOKill();
        sliderTime.value = 1;
        timer.IsStartCountDown = false;

        ShowBtnRepLay();

        timer.SetTimer((float)videoPlayer.length);
    }

    public void Clear()
    {
        OnVideoEnd(videoPlayer);
        videoPlayer.Stop();
        DisableAll();
    }

    private void DisableAll()
    {
        btnPlay.interactable = false;
        btnPause.interactable = false;
        btnReplay.interactable = false;
        bigPlayBtn.interactable = false;
        bigPauseBtn.interactable = false;
        bigReplayBtn.interactable = false;
        muteBtn.interactable = false;
        unMuteBtn.interactable = false;
        sliderTime.interactable = false;
        sliderVolume.interactable = false;
    }

    private void EnableAll()
    {
        btnPlay.interactable = true;
        btnPause.interactable = true;
        btnReplay.interactable = true;
        bigPlayBtn.interactable = true;
        bigPauseBtn.interactable = true;
        bigReplayBtn.interactable = true;
        muteBtn.interactable = true;
        unMuteBtn.interactable = true;
        sliderTime.interactable = true;
        sliderVolume.interactable = true;
    }

    private void ShowBtnPlay()
    {
        btnPause.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(true);
        btnReplay.gameObject.SetActive(false);

        bigPauseBtn.gameObject.SetActive(false);
        bigPlayBtn.gameObject.SetActive(true);
        bigReplayBtn.gameObject.SetActive(false);
    }

    private void ShowBtnRepLay()
    {
        btnPause.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(true);

        bigPauseBtn.gameObject.SetActive(false);
        bigPlayBtn.gameObject.SetActive(false);
        bigReplayBtn.gameObject.SetActive(true);
    }

    private void ShowBtnPause()
    {
        btnPause.gameObject.SetActive(true);
        btnPlay.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(false);

        bigPauseBtn.gameObject.SetActive(true);
        bigPlayBtn.gameObject.SetActive(false);
        bigReplayBtn.gameObject.SetActive(false);
    }
}