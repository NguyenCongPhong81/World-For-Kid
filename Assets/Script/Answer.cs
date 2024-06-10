using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class Answer : MonoBehaviour
    {
        [SerializeField] private Text txtAnswer;
        [SerializeField] private TMP_Text txtIndexAnswer;
        [SerializeField] private Outline outline;

        private QuestionPanel _questionPanel;
        private AnswerData _answerData;
        private int _questionId;
        private bool _interactable;
        private Vector3 _originScale;

        public StatusAns Status { get; set; }

        private void Awake()
        {
            _originScale = txtAnswer.transform.localScale;
        }

        public void SetData(int index, int questionId, AnswerData answer, QuestionPanel questionPanel)
        {
            _questionPanel = questionPanel;
            _questionId = questionId;
            ResetSate();
            char c = (char)(index + 'A');
            txtIndexAnswer.text = c.ToString();

            Transform transform1;
            (transform1 = txtAnswer.transform).DOKill();
            transform1.localScale = Vector3.zero;
            txtAnswer.transform.DOScale(_originScale, 0.5f);

            txtAnswer.text = answer.Text;
            _answerData = answer;
            SetInteractable(true);
        }

        private void OnClickBtnAnswer()
        {
            _questionPanel.BlockInteractionAll();
            SetStatus(StatusAns.Select);
            InGameManager.Instance.myPlayer.SendChooseServerRpc(_questionId, _answerData.Id);
        }

        public void SetInteractable(bool interactable)
        {
            _interactable = interactable;
        }

        private void OnMouseEnter()
        {
            //if (InGameManager.Instance.myPlayer == null ||
            //    InGameManager.Instance.myPlayer.GetInGamePlayerObject().IsDead())
            //{
            //    return;
            //}
            if (!_interactable) return;
            
            outline.enabled = true;
            outline.OutlineColor = Color.blue;
        }

        private void OnMouseDown()
        {
            //if (InGameManager.Instance.myPlayer == null ||
            //    InGameManager.Instance.myPlayer.GetInGamePlayerObject().IsDead())
            //{
            //    return;
            //}
            
            if (!_interactable) return;
            OnClickBtnAnswer();
        }

        private void OnMouseExit()
        {
            //if (InGameManager.Instance.myPlayer == null ||
            //    InGameManager.Instance.myPlayer.GetInGamePlayerObject().IsDead())
            //{
            //    return;
            //}
            if (!_interactable) return;
            outline.enabled = false;
        }

        public int GetUniqueId()
        {
            if (_answerData == null)
            {
                return -1;
            }

            return _answerData.Id;
        }

        public void SetStatus(StatusAns statusAns)
        {
            ResetSate();
            switch (statusAns)
            {
                case StatusAns.Normal:
                    Status = StatusAns.Normal;
                    outline.enabled = false;
                    break;
                case StatusAns.Select:
                    Status = StatusAns.Select;
                    outline.enabled = true;
                    outline.OutlineColor = Color.blue;
                    break;
                case StatusAns.Correct:
                    Status = StatusAns.Correct;
                    outline.enabled = true;
                    outline.OutlineColor = Color.green;
                    break;
                case StatusAns.Wrong:
                    Status = StatusAns.Wrong;
                    outline.enabled = true;
                    outline.OutlineColor = Color.red;
                    break;
            }
        }

        private void ResetSate()
        {
            Status = StatusAns.Normal;
            outline.enabled = false;
        }
    }

    public enum StatusAns
    {
        Normal,
        Select,
        Correct,
        Wrong,
    }
}