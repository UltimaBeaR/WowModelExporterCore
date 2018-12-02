using Assets.Scripts.UI;
using RuntimeGizmos;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowModelExporterCore;
using WowModelExporterUnityPlugin;

public class MainController : MonoBehaviour
{
    public Shader StandardShader;
    public TransformGizmo TransformGizmo;

    public Listbox BonesListbox;
    public Listbox BlendshapesListbox;

    void Start()
    {
        _bonesToButtons = new Dictionary<Transform, Button>();

        TransformGizmo.TargetAdded += TransformGizmo_TargetAdded;
        TransformGizmo.TargetRemoved += TransformGizmo_TargetRemoved;

        InitBlendShapes();
    }

    public void OpenFileButtonClickHandler()
    {
        var fileName = WowVrcFileDialogs.Open();

        if (fileName == null)
            return;

        if (_characterRoot != null)
            Destroy(_characterRoot);

        _openedFile = WowVrcFile.Open(fileName);
        _openedFilePath = fileName;

        _characterRoot = new CharacterBuilder(StandardShader).Build(_openedFile);

        _characterRoot.transform.Rotate(new Vector3(0, 180, 0));
        _characterBonesRoot = _characterRoot.transform.GetSelfOrChildByName("ROOT").gameObject;

        SetBoneListboxItems();

        ClearBlendshapes();
        GetBlendshapesFromFile();

        SelectBlendshape(_blendshapeNames[0]);
    }

    public void SaveFileButtonClickHandler()
    {
        if (_openedFile == null)
            return;

        SaveBonesToSelectedBlendshape();

        SetBlendshapesToFile();

        _openedFile.SaveTo(_openedFilePath);
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
        BonesListbox.ClearListboxItems();
        _bonesToButtons.Clear();

        var bones = _characterBonesRoot.transform.EnumerateSelfAndChildren().Where(x => x.name.StartsWith("face_")).ToArray();

        SetBones(bones);

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
                BonesListbox.AddNewListboxItem(bone.Key, out button);

                button.onClick.AddListener(() => TransformGizmo.ClearAndAddTarget(bone.Value[0]));
                _bonesToButtons[bone.Value[0]] = button;
            }
            else
            {
                Button leftButton, rightButton;
                BonesListbox.AddNewListboxItemLeftRight(bone.Key, out leftButton, out rightButton);

                leftButton.onClick.AddListener(() => TransformGizmo.ClearAndAddTarget(bone.Value[0]));
                _bonesToButtons[bone.Value[0]] = leftButton;

                rightButton.onClick.AddListener(() => { TransformGizmo.ClearAndAddTarget(bone.Value[1]); });
                _bonesToButtons[bone.Value[1]] = rightButton;
            }
        }
    }

    private void TransformGizmo_TargetRemoved(Transform obj)
    {
        _selectedBone = null;
        BonesListbox.SetSelectedListboxItemButton(null, selectedColor);
        GetComponent<MouseCameraControl>().Target = null;
    }

    private void TransformGizmo_TargetAdded(Transform obj)
    {
        _selectedBone = obj;
        BonesListbox.SetSelectedListboxItemButton(_bonesToButtons[obj], selectedColor);
        GetComponent<MouseCameraControl>().Target = obj;
    }

    private void SelectBlendshape(string blendshapeName)
    {
        TransformGizmo.ClearUndoRedo();

        SaveBonesToSelectedBlendshape();

        var data = _blendshapeData[blendshapeName];
        data.IsSelectedInListbox = true;
        _selectedblendShapeName = blendshapeName;

        SetBonesToDefault();
        SetBonesFromSelectedBlendshape();

        BlendshapesListbox.SetSelectedListboxItemButton(data.ListboxButton, selectedColor);
    }

    private void InitBlendShapes()
    {
        _blendshapeData = new Dictionary<string, BlendshapeData>(_blendshapeNames.Length);

        foreach (var blendshapeName in _blendshapeNames)
        {
            Button button;
            BlendshapesListbox.AddNewListboxItem(blendshapeName, out button);

            _blendshapeData[blendshapeName] = new BlendshapeData()
            {
                Name = blendshapeName,
                Bones = new Dictionary<string, BoneData>(),
                IsSelectedInListbox = false,
                ListboxButton = button
            };

            button.onClick.AddListener(() => SelectBlendshape(blendshapeName));
        }
    }

    private void ClearBlendshapes()
    {
        foreach (var blendshape in _blendshapeData.Values)
            blendshape.Bones.Clear();
    }

    private void SaveBonesToSelectedBlendshape()
    {
        if (_selectedblendShapeName != null)
        {
            foreach (var bone in _bones)
            {
                var boneData = new BoneData()
                {
                    LocalPosition = bone.localPosition,
                    LocalRotation = bone.localRotation,
                    LocalScale = bone.localScale
                };

                var defaultBone = _defaultBones[bone.name];

                if (!boneData.Equals(defaultBone))
                {
                    _blendshapeData[_selectedblendShapeName].Bones[bone.name] = boneData;
                }
            }
        }
    }

    private void SetBonesToDefault()
    {
        foreach (var bone in _bones)
        {
            var defaultBone = _defaultBones[bone.name];

            bone.localPosition = defaultBone.LocalPosition;
            bone.localRotation = defaultBone.LocalRotation;
            bone.localScale = defaultBone.LocalScale;
        }
    }

    private void SetBonesFromSelectedBlendshape()
    {
        if (_selectedblendShapeName != null)
        {
            var bones = _blendshapeData[_selectedblendShapeName].Bones;

            foreach (var bone in _bones)
            {
                BoneData boneData;
                if (bones.TryGetValue(bone.name, out boneData))
                {
                    bone.localPosition = boneData.LocalPosition;
                    bone.localRotation = boneData.LocalRotation;
                    bone.localScale = boneData.LocalScale;
                }
            }
        }
    }

    private void SetBones(Transform[] bones)
    {
        _bones = bones;

        _defaultBones = bones.ToDictionary(x => x.name, x => new BoneData() {
            LocalPosition = x.localPosition,
            LocalRotation = x.localRotation,
            LocalScale = x.localScale
        });
    }

    private void SetBlendshapesToFile()
    {
        _openedFile.Blendshapes = _blendshapeData.Values.Select(x => {
            var data = new WowVrcFileData.BlendshapeData() { Name = x.Name };

            data.Bones = x.Bones.Select(bone => new WowVrcFileData.BlendshapeData.BoneData() {
                Name = bone.Key,
                LocalPosition = new WowheadModelLoader.Vec3(bone.Value.LocalPosition.x, bone.Value.LocalPosition.y, bone.Value.LocalPosition.z),
                LocalRotation = new WowheadModelLoader.Vec4(bone.Value.LocalRotation.x, bone.Value.LocalRotation.y, bone.Value.LocalRotation.z, bone.Value.LocalRotation.w),
                LocalScale = new WowheadModelLoader.Vec3(bone.Value.LocalScale.x, bone.Value.LocalScale.y, bone.Value.LocalScale.z)
            }).ToArray();

            return data;
        }).ToArray();
    }

    private void GetBlendshapesFromFile()
    {
        if (_openedFile.Blendshapes == null)
            return;

        foreach (var blendshapeFromFile in _openedFile.Blendshapes)
        {
            var blendshapeData = _blendshapeData[blendshapeFromFile.Name];

            blendshapeData.Bones = blendshapeFromFile.Bones.ToDictionary(x => x.Name, x => new BoneData() {
                LocalPosition = new Vector3(x.LocalPosition.X, x.LocalPosition.Y, x.LocalPosition.Z),
                LocalRotation = new Quaternion(x.LocalRotation.X, x.LocalRotation.Y, x.LocalRotation.Z, x.LocalRotation.W),
                LocalScale = new Vector3(x.LocalScale.X, x.LocalScale.Y, x.LocalScale.Z)
            });
        }
    }

    private readonly string[] _blendshapeNames = new []
    {
        "CATS.AA",
        "CATS.OH",
        "CATS.CH",

        "vrc.blink_left",
        "vrc.blink_right",

        "vrc.lowerlid_left",
        "vrc.lowerlid_right",

        "vrc.v_sil",
        "vrc.v_pp",
        "vrc.v_ff",
        "vrc.v_th",
        "vrc.v_dd",
        "vrc.v_kk",
        "vrc.v_ch",
        "vrc.v_ss",
        "vrc.v_nn",
        "vrc.v_rr",
        "vrc.v_aa",
        "vrc.v_e",
        "vrc.v_ih",
        "vrc.v_oh",
        "vrc.v_ou"
    };

    private Dictionary<Transform, Button> _bonesToButtons;

    // Ключ - название blendshape
    private Dictionary<string, BlendshapeData> _blendshapeData;

    private Transform[] _bones;

    /// <summary>
    /// Дефолтное состояние костей (то, которое получилось при загрузке перса)
    /// Ключ - название кости
    /// Тут есть только кости, которые могут подвергнуться трансформации (из списка костей слева то есть)
    /// </summary>
    private Dictionary<string, BoneData> _defaultBones;

    private Transform _selectedBone;
    private string _selectedblendShapeName;

    private GameObject _characterRoot;
    private GameObject _characterBonesRoot;

    private string _openedFilePath;
    private WowVrcFile _openedFile;

    private static readonly Color selectedColor = new Color(0.7f, 0.7f, 0.9f);

    private class BlendshapeData
    {
        public string Name { get; set; }

        public Button ListboxButton { get; set; }

        public bool IsSelectedInListbox { get; set; }

        /// <summary>
        /// ключ - имя кости (типа "face_jaw"), Значение - измененная трансформация кости.
        /// Если кость по ключу не найдена - значит должна использоваться базовая трансформация кости
        /// </summary>
        public Dictionary<string, BoneData> Bones { get; set; }
    }

    private class BoneData
    {
        public Vector3 LocalPosition { get; set; }
        public Quaternion LocalRotation { get; set; }
        public Vector3 LocalScale { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as BoneData;

            if (other == null)
                return false;

            return LocalPosition == other.LocalPosition
                && LocalRotation == other.LocalRotation
                && LocalScale == other.LocalScale;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}