using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Parser.UI
{
    public class UI
    {
        private GameObject _parserMenu;
        private readonly Parser _parser;
        private readonly ExtensionButton _extensionBtn = new ExtensionButton();

        public UI(Parser paster)
        {
            this._parser = paster;

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Parser.Icon.png");
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);

            Texture2D texture2D = new Texture2D(256, 256);
            texture2D.LoadImage(data);

            _extensionBtn.Icon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f);
            _extensionBtn.Tooltip = "Parser";
            ExtensionButtons.AddButton(_extensionBtn);
        }

        public void AddMenu(MapEditorUI mapEditorUI)
        {
            CanvasGroup parent = mapEditorUI.MainUIGroup[5];
            _parserMenu = new GameObject("Parser Menu");
            _parserMenu.transform.parent = parent.transform;

            AttachTransform(_parserMenu, 260, 190, 1, 1, 0, 0, 1, 1);

            Image image = _parserMenu.AddComponent<Image>();
            image.sprite = PersistentUI.Instance.Sprites.Background;
            image.type = Image.Type.Sliced;
            image.color = new Color(0.24f, 0.24f, 0.24f);

            // Options
            AddDropdown(_parserMenu.transform, "Type", "", new Vector2(-27, -35));
            AddDropdown(_parserMenu.transform, "Drop", "", new Vector2(-27, -75));

            AddTextInput(_parserMenu.transform, "ShowName", "", new Vector2(57, -35));

            // Button
            AddButton(_parserMenu.transform, "SetName", "Set Name", new Vector2(86, -75), () =>
            {
                _parser.SetName();
            });
            AddButton(_parserMenu.transform, "Remove", "Remove", new Vector2(-87, -35), () =>
            {
                _parser.Remove();
            });
            AddButton(_parserMenu.transform, "Copy", "Copy", new Vector2(-87, -75), () =>
            {
                _parser.Copy();
            });
            AddButton(_parserMenu.transform, "Paste", "Paste", new Vector2(-87, -115), () =>
            {
                _parser.Paste();
            });

            AddButton(_parserMenu.transform, "Rename", "Rename", new Vector2(86, -115), () =>
            {
                _parser.Rename();
            });

            AddButton(_parserMenu.transform, "Load", "Load", new Vector2(86, -155), () =>
            {
                _parser.Load();
            });
            AddButton(_parserMenu.transform, "Save", "Save", new Vector2(-87, -155), () =>
            {
                _parser.Save();
            });

            _parserMenu.SetActive(false);
            _extensionBtn.Click = () =>
            {
                _parserMenu.SetActive(!_parserMenu.activeSelf);
            };
        }

        private void AddButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick)
        {
            var button = Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
            MoveTransform(button.transform, 60, 25, 0.5f, 1, pos.x, pos.y);

            button.name = title;
            button.Button.onClick.AddListener(onClick);

            button.SetText(text);
            button.Text.enableAutoSizing = false;
            button.Text.fontSize = 12;
        }

        private void AddTextInput(Transform parent, string title, string text, Vector2 pos)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = ((RectTransform)entryLabel.transform);
            rectTransform.SetParent(parent);

            MoveTransform(rectTransform, 50, 16, 0.5f, 1, pos.x - 27.5f, pos.y);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = TextAlignmentOptions.Right;
            textComponent.fontSize = 12;
            textComponent.text = text;

            var textInput = Object.Instantiate(PersistentUI.Instance.TextInputPrefab, parent);
            MoveTransform(textInput.transform, 55, 20, 0.5f, 1, pos.x + 27.5f, pos.y);
            textInput.GetComponent<Image>().pixelsPerUnitMultiplier = 3;
            textInput.InputField.text = "";
            textInput.InputField.onFocusSelectAll = false;
            textInput.InputField.textComponent.alignment = TextAlignmentOptions.Left;
            textInput.InputField.textComponent.fontSize = 10;

            textInput.InputField.Select();
            textInput.InputField.ActivateInputField();
            textInput.name = "Default";
            Parser.name = textInput;
        }

        private void AddDropdown(Transform parent, string title, string text, Vector2 pos)
        {
            var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
            var rectTransform = ((RectTransform)entryLabel.transform);
            rectTransform.SetParent(parent);

            MoveTransform(rectTransform, 50, 16, 0.5f, 1, pos.x - 27.5f, pos.y);
            var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

            textComponent.name = title;
            textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
            textComponent.alignment = TextAlignmentOptions.Right;
            textComponent.fontSize = 12;
            textComponent.text = text;

            var dropdown = Object.Instantiate(PersistentUI.Instance.DropdownPrefab, parent);
            MoveTransform(dropdown.transform, 100, 20, 0.5f, 1, pos.x + 27.5f, pos.y);
            dropdown.GetComponent<Image>().pixelsPerUnitMultiplier = 3;
            dropdown.Dropdown.options.Clear();

            if(title == "Drop")
            {
                Parser.dropdown = dropdown;
            }
            if (title == "Type")
            {
                var ev = new TMP_Dropdown.OptionData
                {
                    text = "Event"
                };
                var no = new TMP_Dropdown.OptionData
                {
                    text = "Note"
                };
                var obs = new TMP_Dropdown.OptionData
                {
                    text = "Obstacle"
                };
                dropdown.Dropdown.options.Add(ev);
                dropdown.Dropdown.options.Add(no);
                dropdown.Dropdown.options.Add(obs);
                dropdown.Dropdown.onValueChanged.AddListener(TypeChange);
                Parser.type = dropdown;
            }
        }

        private void TypeChange(int type)
        {
            Parser.dropdown.Dropdown.options.Clear();

            if (type == 0)
            {
                Parser.dropdown.Dropdown.options.AddRange(Parser.data.EventOptions);
            }
            else if(type == 1)
            {
                Parser.dropdown.Dropdown.options.AddRange(Parser.data.NoteOptions);
            }
            else if(type == 2)
            {
                Parser.dropdown.Dropdown.options.AddRange(Parser.data.ObstacleOptions);
            }

            Parser.dropdown.Dropdown.value = Parser.dropdown.Dropdown.options.Count - 1;
            Parser.dropdown.Dropdown.RefreshShownValue();
        }

        private RectTransform AttachTransform(GameObject obj, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            RectTransform rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
            rectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY, 0);

            return rectTransform;
        }

        private void MoveTransform(Transform transform, float sizeX, float sizeY, float anchorX, float anchorY, float anchorPosX, float anchorPosY, float pivotX = 0.5f, float pivotY = 0.5f)
        {
            if (!(transform is RectTransform rectTransform)) return;

            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
            rectTransform.anchoredPosition = new Vector3(anchorPosX, anchorPosY, 0);
        }
    }
}
