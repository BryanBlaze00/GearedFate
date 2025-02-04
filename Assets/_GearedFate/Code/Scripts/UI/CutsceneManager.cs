// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    /// <summary>
    /// Cutscene Manager class to manage cutscene dialogues and transitions.
    /// </summary>
using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CutsceneManager : MonoBehaviour
    {
        [Header("Cutscene Settings")]
        [Header("Dialogue Settings")]
        [SerializeField]
        private TextMeshProUGUI _dialogueText;

        [SerializeField]
        private string[] _dialogueLines;
        [SerializeField]
        private float _typingSpeed = 0.05f; // Adjust this for typing speed

        [Header("Button Settings")]
        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private Button _skipButton;

        [Header("Avatar Settings")]
        [SerializeField]
        private GameObject _bossAvatar;

        private UISpriteAnimation _bossAvatarAnim;
        private int _currentLineIndex = 0;

        private void Awake()
        {
            _bossAvatarAnim = _bossAvatar.GetComponent<UISpriteAnimation>();
            _nextButton.onClick.AddListener(NextDialogue);
            _skipButton.onClick.AddListener(SkipCutscene);
        }

        private void Start()
        {
            Elevator.Instance.ActivateElevatorAnim();
            _dialogueText.text = string.Empty;

            // Start the first dialogue
            StartCoroutine(TypeText(_dialogueLines[_currentLineIndex]));
            _bossAvatarAnim.PlayUIAnim();
        }

        /// <summary>
        /// Type the text letter by letter.
        /// </summary>
        private IEnumerator TypeText(string line)
        {
            _nextButton.interactable = false;

            _dialogueText.text = string.Empty;
            foreach (var letter in line)
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }

            _nextButton.interactable = true;

            // _bossAvatarAnim.StopUIAnim(); // Stop the avatar animation
        }

        /// <summary>
        /// Move to the next dialogue line.
        /// </summary>
        public void NextDialogue()
        {
            // Move to the next line
            _currentLineIndex++;

            // Check if we've reached the end of the dialogue
            if (_currentLineIndex >= _dialogueLines.Length)
            {
                // Load the next level if we've reached the end of the dialogue
                GameManager.Instance.LoadNextLevel();
                return;
            }

            StartCoroutine(TypeText(_dialogueLines[_currentLineIndex]));

            // _bossAvatarAnim.PlayUIAnim(); // Play the avatar animation
        }

        /// <summary>
        /// Skip the cutscene.
        /// </summary>
        public void SkipCutscene()
        {
            // Load the next level if we skip the cutscene
            GameManager.Instance.LoadNextLevel();
        }
    }
}
