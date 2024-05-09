using System;
using UnityEngine;
public class Notice : Singleton<Notice>
{
    private NoticePopup _popup;

    public void Show(string tittle, string content, string confirm, string cancel, Action onConfirm, Action onCancel, bool isHideConfirm = false)
    {
        if (_popup == null)
        {
            if (!Application.isPlaying) return;
            _popup = Instantiate(Resources.Load<NoticePopup>("NoticePopup"));
            DontDestroyOnLoad(_popup);
        }
        _popup.gameObject.SetActive(true);
        _popup.Init(tittle, content, confirm, cancel, onConfirm, onCancel, isHideConfirm);
    }

    public bool IsPopupActive()
    {
        if (_popup == null) return false;
        if (_popup.gameObject == null) return false;
        
        return _popup.gameObject.activeInHierarchy;
    }

    public void Hide()
    {
        if(_popup!=null)
        _popup.gameObject.SetActive(false);
    }
}