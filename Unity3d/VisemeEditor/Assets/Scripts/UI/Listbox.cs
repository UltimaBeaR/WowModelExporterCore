using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class Listbox: MonoBehaviour
    {
        public GameObject ListboxItemPrefab;
        public GameObject ListboxItemLeftRightPrefab;

        public void ClearListboxItems()
        {
            foreach (RectTransform child in _content)
                Destroy(child.gameObject);
        }

        public void AddNewListboxItem(string itemTitle, out Button button)
        {
            var item = Instantiate(ListboxItemPrefab);
            item.transform.SetParent(_content);

            var text = item.transform.GetSelfOrChildByName("Caption").GetComponent<Text>();
            text.text = itemTitle;

            button = item.GetComponent<Button>();
        }

        public void AddNewListboxItemLeftRight(string itemTitle, out Button leftButton, out Button rightButton)
        {
            var item = Instantiate(ListboxItemLeftRightPrefab);
            item.transform.SetParent(_content);

            var text = item.transform.GetSelfOrChildByName("Caption").GetComponent<Text>();
            text.text = itemTitle;

            leftButton = item.transform.GetSelfOrChildByName("LButton").GetComponent<Button>();
            rightButton = item.transform.GetSelfOrChildByName("RButton").GetComponent<Button>();
        }

        public void SetSelectedListboxItemButton(Button button, Color selectedColor)
        {
            foreach (var buttonImage in _content.EnumerateSelfAndChildren()
                .Where(x => x.GetComponent<Button>() != null)
                .Select(x => x.GetComponent<Image>()))
            {
                buttonImage.color = Color.white;
            }

            if (button != null)
                button.gameObject.GetComponent<Image>().color = selectedColor;
        }

        void Awake()
        {
            _content = transform
                .GetSelfOrChildByName("Content")
                .GetComponent<RectTransform>();
        }

        private RectTransform _content;
    }
}
