using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPopup: MonoBehaviour
{
    [SerializeField] private Transform iconLoading;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textLoading;
    [SerializeField] private float speedRotate;

    private void Update()
    {
        var rotate = iconLoading.localRotation.eulerAngles;
        rotate.z -= speedRotate * Time.deltaTime;
        iconLoading.localRotation=Quaternion.Euler(rotate);

        textLoading.text = "Loading... " + (int)(slider.value) + "%";
    }


    public void Show(float timeOut = 0, Action onTimeOut = null)
    {
        if (timeOut > 0)
        {
            slider.gameObject.SetActive(true);
            slider.value = 0;
            slider.DOKill();
            slider.maxValue = 100;
            slider.DOValue(100, timeOut).SetEase(Ease.Linear).OnComplete(() =>
            {
                onTimeOut?.Invoke();
                Hide();
            });
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        slider.DOKill();
    }
}
