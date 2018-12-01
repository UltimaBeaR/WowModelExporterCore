using RuntimeGizmos;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowModelExporterUnityPlugin;

public class MainController : MonoBehaviour
{
    public Shader StandardShader;
    public Canvas Canvas;
    public GameObject ListboxItemPrefab;
    public GameObject ListboxItemLeftRightPrefab;

    public TransformGizmo TransformGizmo;

    void Start()
    {
        _raceDropdown = GetSelfOrChildByName(Canvas.transform, "RaceDropdown").GetComponent<Dropdown>();

        _raceDropdown.AddOptions(
            ((WowheadModelLoader.WhRace[])Enum.GetValues(typeof(WowheadModelLoader.WhRace)))
            .Where(x => !x.ToString().StartsWith("Undefined"))
            .Select(x => new Dropdown.OptionData(x.ToString()))
            .ToList()
        );

        _genderToggle = GetSelfOrChildByName(Canvas.transform, "GenderToggle").GetComponent<Toggle>();

        _bonesListboxContent = GetSelfOrChildByName(GetSelfOrChildByName(Canvas.transform, "BonesListbox"), "Content").GetComponent<RectTransform>();
        _bonesToButtons = new Dictionary<Transform, Button>();

        TransformGizmo.TargetAdded += TransformGizmo_TargetAdded;
        TransformGizmo.TargetRemoved += TransformGizmo_TargetRemoved;
    }

    public void LoadCharacterButtonClickHandler()
    {
        if (_characterRoot != null)
        {
            Destroy(_characterRoot);
        }

        WowheadModelLoader.WhRace race;
        Enum.TryParse(_raceDropdown.options[_raceDropdown.value].text, out race);
        var gender = _genderToggle.isOn ? WowheadModelLoader.WhGender.MALE : WowheadModelLoader.WhGender.FEMALE;

        _characterRoot = new CharacterBuilder(StandardShader).Build(race, gender);

        _characterRoot.transform.Rotate(new Vector3(0, 180, 0));
        _characterBonesRoot = GetSelfOrChildByName(_characterRoot.transform, "ROOT").gameObject;

        SetBoneListboxItems();
    }

    public void TranslateRotateButtonClickHandler()
    {
        TransformGizmo.type = TransformGizmo.type == TransformType.Move ? TransformType.Rotate : TransformType.Move;
    }

    public void GlobalLocalButtonClickHandler()
    {
        TransformGizmo.space = TransformGizmo.space == TransformSpace.Global ? TransformSpace.Local : TransformSpace.Global;
    }

    private void SetBoneListboxItems()
    {
        ClearListboxItems(_bonesListboxContent);
        _bonesToButtons.Clear();

        var bones = EnumerateSelfAndChildren(_characterBonesRoot.transform).Where(x => x.name.StartsWith("face_")).ToArray();

        var bonesDict = new Dictionary<string, Transform[]>();

        foreach (var bone in bones)
        {
            bool? isLeft = bone.name.EndsWith(".L") ? true : (bone.name.EndsWith(".R") ? false : (bool?)null);

            var name = bone.name.Remove(0, "face_".Length).Replace('_', ' ');

            if (isLeft == true || isLeft == false)
                name = name.Remove(name.Length - 2, 2);

            if (!isLeft.HasValue)
            {
                bonesDict[name] = new Transform[] { bone };
            }
            else
            {
                if (!bonesDict.ContainsKey(name))
                    bonesDict[name] = new Transform[2];

                var val = bonesDict[name];

                val[isLeft.Value ? 0 : 1] = bone;
            }
        }

        foreach (var bone in bonesDict.OrderBy(x => x.Key))
        {
            if (bone.Value.Length == 1)
            {
                Button button;
                AddNewListboxItem(_bonesListboxContent, bone.Key, out button);

                button.onClick.AddListener(() => TransformGizmo.ClearAndAddTarget(bone.Value[0]));
                _bonesToButtons[bone.Value[0]] = button;
            }
            else
            {
                Button leftButton, rightButton;
                AddNewListboxItemLeftRight(_bonesListboxContent, bone.Key, out leftButton, out rightButton);

                leftButton.onClick.AddListener(() => TransformGizmo.ClearAndAddTarget(bone.Value[0]));
                _bonesToButtons[bone.Value[0]] = leftButton;

                rightButton.onClick.AddListener(() => { TransformGizmo.ClearAndAddTarget(bone.Value[1]); });
                _bonesToButtons[bone.Value[1]] = rightButton;
            }
        }
    }

    private void TransformGizmo_TargetRemoved(Transform obj)
    {
        SetSelectedListboxItemButton(_bonesListboxContent, null);
        GetComponent<MouseCameraControl>().Target = null;
    }

    private void TransformGizmo_TargetAdded(Transform obj)
    {
        SetSelectedListboxItemButton(_bonesListboxContent, _bonesToButtons[obj]);
        GetComponent<MouseCameraControl>().Target = obj;
    }

    private void ClearListboxItems(RectTransform listboxContent)
    {
        foreach (RectTransform child in listboxContent)
            Destroy(child.gameObject);
    }

    private void AddNewListboxItem(RectTransform listboxContent, string itemTitle, out Button button)
    {
        var item = Instantiate(ListboxItemPrefab);
        item.transform.SetParent(listboxContent);

        var text = GetSelfOrChildByName(item.transform, "Caption").GetComponent<Text>();
        text.text = itemTitle;

        button = item.GetComponent<Button>();
    }

    private void AddNewListboxItemLeftRight(RectTransform listboxContent, string itemTitle, out Button leftButton, out Button rightButton)
    {
        var item = Instantiate(ListboxItemLeftRightPrefab);
        item.transform.SetParent(listboxContent);

        var text = GetSelfOrChildByName(item.transform, "Caption").GetComponent<Text>();
        text.text = itemTitle;

        leftButton = GetSelfOrChildByName(item.transform, "LButton").GetComponent<Button>();
        rightButton = GetSelfOrChildByName(item.transform, "RButton").GetComponent<Button>();
    }

    private void SetSelectedListboxItemButton(RectTransform listboxContent, Button button)
    {
        foreach (var buttonImage in EnumerateSelfAndChildren(listboxContent)
            .Where(x => x.GetComponent<Button>() != null)
            .Select(x => x.GetComponent<Image>()))
        {
            buttonImage.color = Color.white;
        }

        if (button != null)
            button.gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.9f);
    }

    private static Transform GetSelfOrChildByName(Transform transform, string name)
    {
        return EnumerateSelfAndChildren(transform).FirstOrDefault(x => x.name == name);
    }

    private static IEnumerable<Transform> EnumerateSelfAndChildren(Transform transform)
    {
        yield return transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            foreach (var deeperTransform in EnumerateSelfAndChildren(transform.GetChild(i)))
                yield return deeperTransform;
        }

        yield break;
    }

    private Dropdown _raceDropdown;
    private Toggle _genderToggle;
    private RectTransform _bonesListboxContent;
    private Dictionary<Transform, Button> _bonesToButtons;

    private GameObject _characterRoot;
    private GameObject _characterBonesRoot;
}