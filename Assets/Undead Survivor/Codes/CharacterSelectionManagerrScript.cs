using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public void OnSelectCharacter0()
    {
        GameManager.Instance.SetCharacterId(0);
        GameManager.Instance.GameStart();
    }

    public void OnSelectCharacter1()
    {
        GameManager.Instance.SetCharacterId(1);
        GameManager.Instance.GameStart();
    }
}

