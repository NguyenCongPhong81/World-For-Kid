using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private List<Answer> answers = new List<Answer>();
        [SerializeField] private Text title;
        private int _numberAns;
        public QuestionData QuestionData { get; set; }
    
        private Vector3 _originScale;

        private void Awake()
        {
            _originScale = title.transform.localScale;
        }

        public void ShowQuestion(QuestionDataRPC questionDataRPC)
        {
            QuestionData = questionDataRPC.ConvertToNormal();
            Transform transform1;
            (transform1 = title.transform).DOKill();
            transform1.localScale = Vector3.zero;
            title.transform.DOScale(_originScale, 0.5f);

            title.text = QuestionData.Text;
            SetData(QuestionData.Id, QuestionData.AnswerDatas);
        }

        public void SetData(int questionId, List<AnswerData> listAnswer)
        {
            _numberAns = listAnswer.Count;

            for (int i = 0; i < _numberAns; ++i)
            {
                int rd = Random.Range(0, _numberAns);
                (listAnswer[i], listAnswer[rd]) = (listAnswer[rd], listAnswer[i]);
            }
    
            for (int i = 0; i < listAnswer.Count; i++)
            {
                answers[i].gameObject.SetActive(true);
                answers[i].SetData(i,questionId, listAnswer[i], this);
            }
    
            for (int i = listAnswer.Count; i < answers.Count; ++i)
            {
                answers[i].gameObject.SetActive(false);
            }
        }
    

        public void BlockInteractionAll()
        {
            for (int i = 0; i < _numberAns; ++i)
            {
                answers[i].SetInteractable(false);
            }
        }
        public void ShowResult(int idAnsCorrect,int idSelected)
        {
            for (int i = 0; i < _numberAns; ++i)
            {
                answers[i].SetInteractable(false);
                if (answers[i].GetUniqueId().Equals(idAnsCorrect))
                {
                    answers[i].SetStatus(StatusAns.Correct);
                    continue;
                }

                if (answers[i].GetUniqueId().Equals(idSelected))
                {
                    answers[i].SetStatus(StatusAns.Wrong);
                    continue;
                }
                
                answers[i].SetStatus(StatusAns.Normal);
            }
        }
    }
}
