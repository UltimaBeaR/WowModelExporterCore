using RuntimeGizmos;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowModelExporterUnityPlugin;

public class MainController : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject ListboxItemPrefab;

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

    public void LoadCharacterButtonClickHandler()
    {
        if (_characterRoot != null)
        {
            Destroy(_characterRoot);
        }

        WowheadModelLoader.WhRace race;
        Enum.TryParse(_raceDropdown.options[_raceDropdown.value].text, out race);
        var gender = _genderToggle.isOn ? WowheadModelLoader.WhGender.MALE : WowheadModelLoader.WhGender.FEMALE;

        _characterRoot = new CharacterBuilder().Build(race, gender);
        _characterRoot.transform.Rotate(new Vector3(0, 180, 0));
        _characterBonesRoot = GetSelfOrChildByName(_characterRoot.transform, "ROOT").gameObject;

        SetBoneListboxItems();

        var jaw = GetSelfOrChildByName(_characterRoot.transform, "face_jaw");
        var collider = jaw.gameObject.AddComponent<SphereCollider>();
        collider.radius = 0.1f;
    }

    public void TranslateRotateButtonClickHandler()
    {
        var gizmo = Camera.main.GetComponent<TransformGizmo>();
        gizmo.type = gizmo.type == TransformType.Move ? TransformType.Rotate : TransformType.Move;
    }

    private void SetBoneListboxItems()
    {
        ClearListboxItems(_bonesListboxContent);

        var bones = EnumerateSelfAndChildren(_characterBonesRoot.transform).Where(x => x.name.StartsWith("face_"));

        foreach (var bone in bones)
        {
            AddNewListboxItem(_bonesListboxContent, bone.name, x => { });
        }
    }

    private void ClearListboxItems(RectTransform listboxContent)
    {
        foreach (RectTransform child in listboxContent)
            Destroy(child.gameObject);
    }

    private void AddNewListboxItem(RectTransform listboxContent, string itemTitle, Action<string> clickHandler)
    {
        var item = Instantiate(ListboxItemPrefab);
        item.transform.SetParent(listboxContent);

        var text = GetSelfOrChildByName(item.transform, "Text").GetComponent<Text>();
        var button = item.GetComponent<Button>();

        text.text = itemTitle;
        button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => clickHandler(itemTitle)));
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