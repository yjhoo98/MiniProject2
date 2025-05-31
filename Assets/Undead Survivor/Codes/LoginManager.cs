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
        // AudioListener 중복 제거 (Unity 2023+ 권장 방식)
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None); // ? 수정된 줄

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
            Debug.LogWarning("? 이메일 또는 비밀번호가 비어 있습니다.");
            return;
        }

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning("? 로그인 실패 → 회원가입 시도");

                auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(regTask =>
                {
                    if (regTask.IsCanceled || regTask.IsFaulted)
                    {
                        Debug.LogError("? 회원가입 실패: " + regTask.Exception?.Message);
                    }
                    else
                    {
                        GameManager.Instance.SetUser(regTask.Result.User.UserId); // 변경된 부분
                        SceneManager.LoadScene("GameScene"); // 변경된 부분
                    }
                });
            }
            else
            {
                GameManager.Instance.SetUser(task.Result.User.UserId); // 변경된 부분
                SceneManager.LoadScene("GameScene"); // 변경된 부분
            }
        });
    }
}
