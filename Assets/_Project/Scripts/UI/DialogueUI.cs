using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Image speakerPortrait;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button tapToContinue;
        [SerializeField] private float typewriterSpeed = 0.03f;

        private DialogueLine[] lines;
        private int currentLineIndex;
        private bool isTyping;
        private Coroutine typewriterCoroutine;

        public System.Action OnDialogueComplete;

        private void Start()
        {
            tapToContinue.onClick.AddListener(AdvanceDialogue);
            dialoguePanel.SetActive(false);
        }

        public void StartDialogue(DialogueLine[] dialogueLines)
        {
            lines = dialogueLines;
            currentLineIndex = 0;
            dialoguePanel.SetActive(true);
            ShowLine(lines[0]);
        }

        private void ShowLine(DialogueLine line)
        {
            speakerNameText.text = line.speakerName;
            if (line.speakerPortrait != null)
            {
                speakerPortrait.sprite = line.speakerPortrait;
                speakerPortrait.gameObject.SetActive(true);
            }
            else
            {
                speakerPortrait.gameObject.SetActive(false);
            }

            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);

            typewriterCoroutine = StartCoroutine(TypewriterEffect(line.text));
        }

        private IEnumerator TypewriterEffect(string text)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }

            isTyping = false;
        }

        private void AdvanceDialogue()
        {
            if (isTyping)
            {
                StopCoroutine(typewriterCoroutine);
                dialogueText.text = lines[currentLineIndex].text;
                isTyping = false;
                return;
            }

            currentLineIndex++;
            if (currentLineIndex >= lines.Length)
            {
                dialoguePanel.SetActive(false);
                OnDialogueComplete?.Invoke();
            }
            else
            {
                ShowLine(lines[currentLineIndex]);
            }
        }
    }
}
