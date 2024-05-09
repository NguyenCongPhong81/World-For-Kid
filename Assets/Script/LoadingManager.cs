using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : Singleton<LoadingManager>
{
    private LoadingPopup _popup;

    public void Show(float timeOut = 0, Action onTimeOut = null)
    {
        if (_popup == null)
        {
            if (!Application.isPlaying) return;
            _popup = Instantiate(Resources.Load<LoadingPopup>("LoadingPopup"));
            DontDestroyOnLoad(_popup);
        }

        _popup.gameObject.SetActive(true);
        _popup.Show(timeOut, onTimeOut);
    }

    public void Hide()
    {
        if (_popup != null)
        {
            _popup.Hide();
        }
    }
}
