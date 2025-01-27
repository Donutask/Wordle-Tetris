using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Donutask.Wordfall
{
    public class BlankLetterChooser : MonoBehaviour
    {
        static BlankLetterChooser Instance;
        public static bool choosingLetter { get; private set; }

        [SerializeField] GameObject chooserUI;
        [SerializeField] Image letterImage;
        [SerializeField] float holdSpeed;
        int selectedLetter;
        Action<char> actionWhenChosen;

        private void Awake()
        {
            Instance = this;
        }

        public static void ChooseBlank(Action<char> completeAction)
        {
            Instance.actionWhenChosen = completeAction;
            Instance.OpenChooserUI();
        }

        void OpenChooserUI()
        {
            choosingLetter = true;
            chooserUI.SetActive(true);

            selectedLetter = 0;
            UpdatePreview();

            StartCoroutine(SelectLetterLoop());
            StartCoroutine(DropLoop());

            AudioManager.instance.Play("Open");
        }

        void ConfirmLetter()
        {
            choosingLetter = false;
            chooserUI.SetActive(false);
            actionWhenChosen.Invoke(WordManager.alphabet[selectedLetter]);

            AudioManager.instance.Play("Confirm Letter");
        }

        int heldFrames;
        IEnumerator SelectLetterLoop()
        {
            while (choosingLetter)
            {
                if (heldFrames == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(holdSpeed / (float)(heldFrames));
                }

                int movementX = Mathf.RoundToInt(ControlsManager.GetLetterMovement().x);
                if (movementX == -1)
                {
                    ChangeValue(-1);
                }
                else if (movementX == 1)
                {
                    ChangeValue(1);
                }
                else
                {
                    heldFrames = 0;
                }
            }
        }
        void ChangeValue(int amount)
        {
            heldFrames++;
            selectedLetter += amount;

            if (selectedLetter < 0)
            {
                selectedLetter = 25;
            }
            if (selectedLetter > 25)
            {
                selectedLetter = 0;
            }
            UpdatePreview();

            float pitchMultiplier = Mathf.Clamp(1 + (heldFrames / 10), 1, 1.5f);
            if (!AudioManager.instance.IsPlaying())
                AudioManager.instance.Play("Change Blank", pitchMultiplier);
        }

        //Wait until not dropping, wait a little extra just to make sure, then check for drop every frame
        IEnumerator DropLoop()
        {
            yield return new WaitUntil(() => ControlsManager.IsDropping() == false);
            yield return new WaitForSeconds(0.25f);

            while (choosingLetter)
            {
                yield return new WaitForEndOfFrame();
                if (ControlsManager.IsDropping())
                {
                    ConfirmLetter();
                }
            }
        }

        void UpdatePreview()
        {
            letterImage.sprite = WordManager.GetLetterSprite(WordManager.alphabet[selectedLetter]);
        }
    }
}
