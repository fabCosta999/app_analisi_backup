using UnityEngine;

public class lessonMenu : MonoBehaviour
{
    public scenes scenesScript;

    public void ButtonHandler(int _100topic_lesson)
    {
        UpdatePrefs(_100topic_lesson / 100, _100topic_lesson % 100);
        scenesScript.ChangeScene(8);
    }

    public void QuizButtonHandler(int _100topic_lesson)
    {
        UpdatePrefs(_100topic_lesson / 100, _100topic_lesson % 100);
        scenesScript.ChangeScene(9);
    }

    public void FinalQuiz(int topic)
    {
        PlayerPrefs.SetInt(keysGeneral.TOPIC, topic);
        scenesScript.ChangeScene(12);
    }

    private void UpdatePrefs(int topic, int lesson)
    {
        PlayerPrefs.SetInt(keysGeneral.TOPIC, topic);
        PlayerPrefs.SetInt(keysGeneral.LESSON, lesson);
    }
}
