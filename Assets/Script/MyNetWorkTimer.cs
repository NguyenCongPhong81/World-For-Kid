using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Script
{
    public class MyNetWorkTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtTime;

        public Action OnTimeOut;

        private float _time;

        private float _startTime;

        public void SetStartTime(float time)
        {
            _startTime = _time = time;
            UpdateUI();
        }

        private void Update()
        {
            if (InGameManager.Instance.gameState != GameState.PLaying || NetworkManager.Singleton == null)
            {
                return;
            }

            _time = _startTime -
                    (NetworkManager.Singleton.ServerTime.TimeAsFloat - InGameManager.Instance.timeStart.Value);

            if (_time < 0)
            {
                _time = 0;
                OnTimeOut?.Invoke();
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            int minus = Mathf.RoundToInt(_time) / 60;
            int second = Mathf.RoundToInt(_time) % 60;

            string txtMinus = ((minus <= 9) ? "0" : "") + minus;
            string txtSecond = ((second <= 9) ? "0" : "") + second;
            txtTime.text = txtMinus + ":" + txtSecond;
        }
    }
}