using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionVoice : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Timer timer;

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnReplay;
    [SerializeField] private Slider sliderTime;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private Button muteBtn;
    [SerializeField] private Button unMuteBtn;

    private string _url;
    private Coroutine _coroutine;
    private float _oldVolume;

    private void Start()
    {
        btnPlay.onClick.AddListener(ContinuePlay);
        btnPause.onClick.AddListener(OnClickPause);
        btnReplay.onClick.AddListener(StartPlay);
        sliderVolume.onValueChanged.AddListener(OnVolumeChange);
        timer.OnTimeOver = EndVoice;
        muteBtn.onClick.AddListener(OnClickMute);
        unMuteBtn.onClick.AddListener(OnClickUnMute);
    }

    private void OnClickMute()
    {
        muteBtn.gameObject.SetActive(false);
        unMuteBtn.gameObject.SetActive(true);
        _oldVolume = audioSource.volume;
        sliderVolume.value = 0;
        audioSource.volume = 0;
    }

    private void OnClickUnMute()
    {
        muteBtn.gameObject.SetActive(true);
        unMuteBtn.gameObject.SetActive(false);
        sliderVolume.value = _oldVolume;
        audioSource.volume = _oldVolume;
    }

    private void OnVolumeChange(float value)
    {
        Debug.LogError("Hoan OnVolumeChange" + value);
        audioSource.volume = value;
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

    private void StartPlay()
    {
        sliderTime.value = 0;
        sliderTime.DOValue(1f, (float)audioSource.clip.length);
        timer.SetTimer((float)audioSource.clip.length);

        audioSource.Play();
        ShowBtnPause();
        timer.IsStartCountDown = true;
    }

    private void ContinuePlay()
    {
        // audioSource.time = Mathf.Min(_currentPlaybackTime,audioSource.clip.length);
        audioSource.UnPause();
        ShowBtnPause();
        sliderTime.DOPlayForward();
        timer.IsStartCountDown = true;
    }

    private void OnClickPause()
    {
        sliderTime.DOPause();
        audioSource.Pause();
        timer.IsStartCountDown = false;
        ShowBtnPlay();
    }

    public void Init(string url)
    {
        _url = url;
        sliderTime.value = 0;
        timer.SetTimer(0);
        DisableAllBtn();
        ShowBtnPlay();
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(LoadAudio());
    }

    IEnumerator LoadAudio()
    {
        using (var request = UnityWebRequestMultimedia.GetAudioClip(_url, AudioType.UNKNOWN))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.clip = audioClip;

                _oldVolume = 1;
                OnClickUnMute();
                sliderTime.value = 0;
                EnableAllBtn();
                StartPlay();
            }
            else
            {
                Debug.LogError("Error loading audio: " + request.error);
            }
        }
    }

    private void EndVoice()
    {
        sliderTime.DOKill();
        sliderTime.value = 1;
        ShowBtnReplay();
        if (audioSource.clip)
        {
            timer.SetTimer((float)audioSource.clip.length);
            timer.IsStartCountDown = false;
            audioSource.Stop();
        }
    }

    public void Clear()
    {
        DisableAllBtn();

        EndVoice();
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }


    private void ShowBtnPause()
    {
        btnPlay.gameObject.SetActive(false);
        btnPause.gameObject.SetActive(true);
        btnReplay.gameObject.SetActive(false);
    }

    private void ShowBtnPlay()
    {
        btnPlay.gameObject.SetActive(true);
        btnPause.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(false);
    }

    private void ShowBtnReplay()
    {
        btnPlay.gameObject.SetActive(false);
        btnPause.gameObject.SetActive(false);
        btnReplay.gameObject.SetActive(true);
    }

    private void DisableAllBtn()
    {
        btnPlay.interactable = false;
        btnPause.interactable = false;
        btnReplay.interactable = false;
        sliderTime.interactable = false;
        sliderVolume.interactable = false;
    }

    private void EnableAllBtn()
    {
        btnPlay.interactable = true;
        btnPause.interactable = true;
        btnReplay.interactable = true;
        sliderTime.interactable = true;
        sliderVolume.interactable = true;
    }
}