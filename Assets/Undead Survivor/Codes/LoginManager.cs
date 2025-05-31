using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;

    private void Awake()
    {
        // AudioListener �ߺ� ���� (Unity 2023+ ���� ���)
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None); // ? ������ ��

        if (listeners.Length > 1)
        {
            foreach (var listener in listeners)
            {
                if (listener.gameObject != Camera.main.gameObject)
                    Destroy(listener);
            }
        }
    }


    public void OnLoginButtonClick()
    {
        string email = inputEmail.text.Trim();
        string password = inputPassword.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("? �̸��� �Ǵ� ��й�ȣ�� ��� �ֽ��ϴ�.");
            return;
        }

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("? �α��� ���� �� ȸ������ �õ�");

                auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(regTask =>
                {
                    if (regTask.IsCanceled || regTask.IsFaulted)
                    {
                        Debug.LogError("? ȸ������ ����: " + regTask.Exception?.Message);
                    }
                    else
                    {
                        GameManager.Instance.SetUser(regTask.Result.User.UserId); // ����� �κ�
                        SceneManager.LoadScene("GameScene"); // ����� �κ�
                    }
                });
            }
            else
            {
                GameManager.Instance.SetUser(task.Result.User.UserId); // ����� �κ�
                SceneManager.LoadScene("GameScene"); // ����� �κ�
            }
        });
    }
}
