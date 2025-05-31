using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Follow : MonoBehaviour
{
    RectTransform rect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) // ����� �κ�
            return; // ����� �κ�

        if (Camera.main == null) // ����� �κ�
        {
            Debug.LogWarning("?? Camera.main is null"); // ����� �κ�
            return; // ����� �κ�
        }

        rect.position = Camera.main.WorldToScreenPoint(GameManager.Instance.player.transform.position);
    }

}
