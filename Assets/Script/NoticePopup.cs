using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : MonoBehaviour
{
    [SerializeField] Text txtTittle;
    [SerializeField] Text txtContent;
    [SerializeField] Text txtConfirm;
    [SerializeField] Text txtCancel;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    public void Init(string tittle, string content, string confirm, string cancel, Action onConfirm, Action onCancel, bool isHideConfirm = false)
    {

        txtTittle.text = tittle;
        txtContent.text = content;
        txtConfirm.text = confirm;
        txtCancel.text = cancel;

        btnConfirm.onClick.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();

        btnConfirm.gameObject.SetActive(!isHideConfirm);
        btnConfirm.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            onConfirm?.Invoke();
        });
        btnCancel.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            onCancel?.Invoke();
        });
    }
}
