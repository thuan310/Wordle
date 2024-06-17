using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private Button shopButton;

    private void Awake()
    {
        shopButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
    }
}
