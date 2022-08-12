using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperComicWorld
{
    public class MultiActionButton : SCScriptBase, IPointerClickHandler
    {
        [Flags]
        public enum Mode
        {
            Text = 0x1,
            Event = 0x2,
            Visible = 0x4,
            // PosAndSize = 0x8,
            // All = Text | Event | Visible | PosAndSize
        }

        public Mode mode = Mode.Text;
        public string[] texts;
        public UnityEvent[] onClicked;
        public uint[] visible_compressed;

        private Text ui_text;
        private int current_state;

        protected override void Awake()
        {
            if ((mode & Mode.Text) != 0)
                ui_text = GetComponentInChildren<Text>(true);

            current_state = -1;
        }

        public void ChangeState(int new_state)
        {
            Debug.Assert(new_state >= 0);

            if (current_state != new_state)
            {
                var md = mode;

                if ((md & Mode.Visible) != 0)
                    gameObject.SetActive((visible_compressed[new_state >> 5] & (1 << (new_state & 0x1F))) != 0);

                if ((md & Mode.Text) != 0)
                    ui_text.text = texts[new_state];

                current_state = new_state;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData arg)
        {
            if (arg.button == PointerEventData.InputButton.Left)
                OnButtonClicked(arg, current_state);
        }

        protected virtual void OnButtonClicked(PointerEventData arg, int state)
        {
            if ((mode & Mode.Event) != 0)
                onClicked[state].Invoke();
        }
    }
}
