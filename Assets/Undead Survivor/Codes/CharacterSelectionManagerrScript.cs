using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public void OnSelectCharacter0()
    {
        GameManager.Instance.GameStart(0); // ����� �κ�
    }

    public void OnSelectCharacter1()
    {
        GameManager.Instance.GameStart(1); // ����� �κ�
    }

    public void OnSelectCharacter2()
    {
        GameManager.Instance.GameStart(2);
    }

    public void OnSelectCharacter3()
    {
        GameManager.Instance.GameStart(3);
    }
}
