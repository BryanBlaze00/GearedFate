//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using BTG;

public class Credits : MonoBehaviour
{
    [SerializeField] private float _waitForCredits = 0.5f;

    private void Start()
    {
        StartCoroutine(DisplayCredits());
    }

    private IEnumerator DisplayCredits()
    {
        yield return new WaitForSeconds(_waitForCredits);
        GameManager.Instance.LoadMainMenu();
    }
}
