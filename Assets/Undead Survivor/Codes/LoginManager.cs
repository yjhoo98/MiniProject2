using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public GameObject loginPanel;
    public GameObject characterGroup; // 캐릭터 선택 UI

    private string userId;

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
                        Debug.Log("? 회원가입 성공!");
                        userId = regTask.Result.User.UserId;
                        ShowCharacterSelection(userId);
                    }
                });
            }
            else
            {
                Debug.Log("? 로그인 성공!");
                userId = task.Result.User.UserId;
                ShowCharacterSelection(userId);
            }
        });
    }

    private void ShowCharacterSelection(string uid)
    {
        GameManager.Instance.SetUser(uid); // UID를 GameManager에 저장
        loginPanel.SetActive(false);
        characterGroup.SetActive(true);   // 캐릭터 선택 UI 활성화
    }

    public void OnCharacterSelected(int index)
    {
        GameManager.Instance.SetCharacterId(index); // 선택된 캐릭터 ID 저장
    }
}
