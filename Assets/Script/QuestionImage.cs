using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private RectTransform rectTransform;
    private Coroutine _coroutine;

    // public void Start()
    // {
    //     Init("test","https://smartblogger.com/wp-content/uploads/2222/12/symbolism-examples-tw.png");
    // }

    public void Init(string url)
    {
        rawImage.gameObject.SetActive(false);
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine=StartCoroutine(DownloadImage(url));
    }
        
        
    IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                //CustomLogger.LogErrorOfOther("Download Image failed:"+www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                rawImage.gameObject.SetActive(true);
                var rect = rectTransform.rect;
                rawImage.rectTransform.sizeDelta = texture.width * rect.height < rect.width * texture.height
                    ? new Vector2(texture.width * rect.height / texture.height, rect.height)
                    : new Vector2(rect.width, texture.height * rect.width / texture.width);
                rawImage.texture = texture;
            }
        }
    }

    public void Clear()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }
}
