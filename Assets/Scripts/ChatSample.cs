using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gpt4All.Samples
{
    public class ChatSample : MonoBehaviour
    {
        public LlmManager manager;

        [Header("Chat")]
        public TMPro.TMP_InputField input;
        public ScrollRect outputArea;
        public TMPro.TMP_Text output;
        public Button submit;

        [Header("Appearance")]
        [SerializeField]
        protected RectTransform textArea;
        [SerializeField]
        protected Button showChatButton;
        [SerializeField]
        protected Button hideChatButton;

        private string _previousText;

        private void Awake()
        {
            input.onEndEdit.AddListener(OnSubmit);
            submit.onClick.AddListener(OnSubmitPressed);
            manager.OnResponseUpdated += OnResponseHandler;
            showChatButton.onClick.AddListener(OnShowChatPressed);
            hideChatButton.onClick.AddListener(OnHideChatPressed);
        }

        private void OnDestroy()
        {
            input.onEndEdit.RemoveListener(OnSubmit);
            submit.onClick.RemoveListener(OnSubmitPressed);
            manager.OnResponseUpdated -= OnResponseHandler;
            showChatButton.onClick.RemoveListener(OnShowChatPressed);
            hideChatButton.onClick.RemoveListener(OnHideChatPressed);
        }

        private void Start()
        {
            ShowChat(false);
        }

        private void OnShowChatPressed()
        {
            ShowChat(true);
        }

        private void OnHideChatPressed()
        {
            ShowChat(false);
        }

        private void ShowChat(bool show)
        {
            showChatButton.gameObject.SetActive(!show);
            hideChatButton.gameObject.SetActive(show);
            textArea.gameObject.SetActive(show);
        }

        private void OnSubmit(string prompt)
        {
            if (!Input.GetKey(KeyCode.Return))
                return;
            SendToChat(input.text);
        }

        private void OnSubmitPressed()
        {
            SendToChat(input.text);
        }

        private async void SendToChat(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return;

            input.text = "";
            output.text += $"<b>User:</b> {prompt}\n<b>Answer</b>: ";
            _previousText = output.text;

            await manager.Prompt(prompt);
            output.text += "\n";
            outputArea.normalizedPosition = Vector2.zero;
        }

        private void OnResponseHandler(string response)
        {
            output.text = _previousText + response;
            outputArea.normalizedPosition = Vector2.zero;
        }
    }
}