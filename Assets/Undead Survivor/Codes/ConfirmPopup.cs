using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmPopup : MonoBehaviour
{
    public Text messageText;
    public Button yesButton;
    public Button noButton;

    static ConfirmPopup instance;

    void Awake() => instance = this;

    public static void Show(string message, Action onConfirm)
    {
        instance.gameObject.SetActive(true);
        instance.messageText.text = message;

        instance.yesButton.onClick.RemoveAllListeners();
        instance.noButton.onClick.RemoveAllListeners();

        instance.yesButton.onClick.AddListener(() =>
        {
            instance.gameObject.SetActive(false);
            onConfirm?.Invoke();
        });

        instance.noButton.onClick.AddListener(() =>
        {
            instance.gameObject.SetActive(false);
        });
    }
}
