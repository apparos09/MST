using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RM_MST
{
    // Displays the current combo.
    public class ComboDisplay : MonoBehaviour
    {
        // The stage manager.
        public StageManager stageManager;

        // The combo text.
        public TMP_Text comboText;

        // The animator.
        public Animator animator;

        // The combo animation.
        public string comboAnim = "Combo Display Animation";

        // The no animation anim.
        public string emptyAnim = "Empty Animation";

        // Start is called before the first frame update
        void Start()
        {
            // If the stage manager isn't set, set it.
            if (stageManager == null)
                stageManager = StageManager.Instance;

            // Clears the text.
            ClearText();
        }

        // Sets the position of the display to the provided meteor.
        public void SetDisplayPosition(Vector3 position)
        {
            transform.position = position;
        }

        // Sets the display position using this game object.
        public void SetDisplayPosition(GameObject go)
        {
            SetDisplayPosition(go.transform.position);
        }

        // Sets the text to the combo.
        public void SetTextToCombo()
        {
            comboText.text = "x" + stageManager.combo.ToString();
        }

        // Clears the text.
        public void ClearText()
        {
            comboText.text = "";
        }

        // Play the combo animation.
        public void PlayComboAnimation()
        {
            // Plays the combo animation.
            animator.Play(comboAnim);
        }

        // Plays the combo animation at the provided position.
        public void PlayComboAnimationAtPosition(Vector3 position)
        {
            SetDisplayPosition(position);
            PlayComboAnimation();
        }

        // Plays the combo animation at the provided game object's position.
        public void PlayComboAnimationAtPosition(GameObject go)
        {
            SetDisplayPosition(go);
            PlayComboAnimation();
        }

        // Called on the start of the combo animation.
        public void OnComboAnimationStart()
        {
            SetTextToCombo();
        }

        // Called on the end of the combo animation.
        public void OnComboAnimationEnd()
        {
            ClearText();
            animator.Play(emptyAnim);
        }
    }
}
