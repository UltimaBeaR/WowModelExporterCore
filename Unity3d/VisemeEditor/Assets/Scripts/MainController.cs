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
    }

    void Awake()
    {
        //TransformGizmo = GetComponent<TransformGizmo>();
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
        var gizmo = Camera.main.GetComponent<TransformGizmo>();
        gizmo.type = gizmo.type == TransformType.Move ? TransformType.Rotate : TransformType.Move;
    }

    private void SetBoneListboxItems()
    {
        ClearListboxItems(_bonesListboxContent);

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
                AddNewListboxItem(_bonesListboxContent, bone.Key,
                    () =>
                    {
                        TransformGizmo.AddTarget(bone.Value[0]);
                    });
            }
            else
            {
                AddNewListboxItemLeftRight(_bonesListboxContent, bone.Key,
                    () =>
                    {
                        TransformGizmo.AddTarget(bone.Value[0]);
                    },
                    () =>
                    {
                        TransformGizmo.AddTarget(bone.Value[1]);
                    });
            }
        }
    }

    private void ClearListboxItems(RectTransform listboxContent)
    {
        foreach (RectTransform child in listboxContent)
            Destroy(child.gameObject);
    }

    private void AddNewListboxItem(RectTransform listboxContent, string itemTitle, Action clickHandler)
    {
        var item = Instantiate(ListboxItemPrefab);
        item.transform.SetParent(listboxContent);

        var text = GetSelfOrChildByName(item.transform, "Caption").GetComponent<Text>();
        var button = item.GetComponent<Button>();

        text.text = itemTitle;
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(clickHandler));
    }

    private void AddNewListboxItemLeftRight(RectTransform listboxContent, string itemTitle, Action leftClickHandler, Action rightClickHandler)
    {
        var item = Instantiate(ListboxItemLeftRightPrefab);
        item.transform.SetParent(listboxContent);

        var text = GetSelfOrChildByName(item.transform, "Caption").GetComponent<Text>();
        var leftButton = GetSelfOrChildByName(item.transform, "LButton").GetComponent<Button>();
        var rightButton = GetSelfOrChildByName(item.transform, "RButton").GetComponent<Button>();

        text.text = itemTitle;
        leftButton.onClick.AddListener(new UnityEngine.Events.UnityAction(leftClickHandler));
        rightButton.onClick.AddListener(new UnityEngine.Events.UnityAction(rightClickHandler));
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

    private GameObject _characterRoot;
    private GameObject _characterBonesRoot;
}