using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour {

    public static QuizManager instance;

    public static QuizManager GetInstance() {
        return instance;
    }

    public List<QuestionAndAnswers> QuestionAndAnswersList;
    public GameObject[] answerOptions;

    public float QUIZ_TIME;
    public int currentQuestion;

    public Text QuestionText;
    public Text CorrectFeedbackText;
    public Text WrongFeedbackText;

    void Awake() {
        instance = this;
        CorrectFeedbackText.gameObject.SetActive(false);
        WrongFeedbackText.gameObject.SetActive(false);
    }

    void Start() {
        QUIZ_TIME = GameManager.GetInstance().QUIZ_TIME;
        generateQuestion();
    }

    public void Correct() {
        QuestionAndAnswersList.RemoveAt(currentQuestion);
        Time.timeScale = 1;
        GameManager.GetInstance().QUIZ_TIME += QUIZ_TIME;
        GameManager.GetInstance().quizPause = false;
        GameManager.GetInstance().answered = true;
        StartCoroutine(AnswerFeedback(true));
        EnemyController.GetInstance().DestroyEnemy();
    }

    public void Wrong() {
        Time.timeScale = 1;
        GameManager.GetInstance().QUIZ_TIME += QUIZ_TIME;
        GameManager.GetInstance().quizPause = false;
        GameManager.GetInstance().answered = true;
        if(GameManager.GetInstance().livingAllies > 0) {
            StartCoroutine(AnswerFeedback(false));
            FriendController.GetInstance().DestroyAlly();
        }        
    }

    void SetAnswers() {
        for (int i = 0; i < answerOptions.Length; i++) {
            answerOptions[i].GetComponent<AnswerScript>().isCorrect = false;
            answerOptions[i].transform.GetChild(0).GetComponent<Text>().text = QuestionAndAnswersList[currentQuestion].Answers[i]; 

            if(QuestionAndAnswersList[currentQuestion].CorrectAnswer == i + 1) {
                answerOptions[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    public void generateQuestion() {
        currentQuestion = Random.Range(0, QuestionAndAnswersList.Count);

        QuestionText.text = QuestionAndAnswersList[currentQuestion].Question;

        SetAnswers();
    }


    public IEnumerator AnswerFeedback(bool correct) {
        if(correct) {
            SoundManager.PlaySound(SoundManager.Sound.CorrectAnswer);

            CorrectFeedbackText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.5f);
            CorrectFeedbackText.gameObject.SetActive(false);
        } else {
            SoundManager.PlaySound(SoundManager.Sound.WrongAnswer);

            WrongFeedbackText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.5f);
            WrongFeedbackText.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(3f);
    }

}
