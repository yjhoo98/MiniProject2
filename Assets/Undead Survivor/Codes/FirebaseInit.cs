using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // ? 이 줄이 필수입니다
                app.Options.DatabaseUrl = new System.Uri("https://undeadsurvivor-77af8-default-rtdb.firebaseio.com/");

                Debug.Log("? Firebase 초기화 성공");
            }
            else
            {
                Debug.LogError("? Firebase 초기화 실패: " + task.Result);
            }
        });
    }
}
