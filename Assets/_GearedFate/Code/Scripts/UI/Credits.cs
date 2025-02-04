//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using System.Collections;
using BTG;

public class Credits : MonoBehaviour
{
    [SerializeField] private float _waitForCredits = 0.5f;

    private void Start()
    {
        this.StartCoroutine(this.DisplayCredits());
    }

    private IEnumerator DisplayCredits()
    {
        yield return new WaitForSeconds(this._waitForCredits);
        GameManager.Instance.LoadMainMenu();
    }
}
