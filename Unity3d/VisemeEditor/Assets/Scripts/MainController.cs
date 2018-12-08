using Assets.Scripts.UI;
using RuntimeGizmos;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowheadModelLoader;
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

        if (_character != null)
        {
            Destroy(_character.Root);
            _character = null;
        }

        _openedFile = WowVrcFile.Open(fileName);
        _openedFilePath = fileName;

        _character = new CharacterBuilder(StandardShader).Build(_openedFile, true);

        _character.Root.transform.Rotate(new Vector3(0, 180, 0));
        _characterMesh = _character.Root.transform.GetSelfOrChildByName("character").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        _characterBonesRoot = _character.Root.transform.GetSelfOrChildByName("ROOT").gameObject;

        _defaultVertexPositions = new Vector3[_characterMesh.vertices.Length];
        _characterMesh.vertices.CopyTo(_defaultVertexPositions, 0);

        _basicVertexPositions = new Vector3[_characterMesh.vertices.Length];
        _characterMesh.vertices.CopyTo(_basicVertexPositions, 0);

        SetBoneListboxItems();

        ClearBlendshapes();
        GetBlendshapesFromFile();

        SelectBasicBlendshape();
    }

    public void SaveFileButtonClickHandler()
    {
        if (_openedFile == null)
            return;

        SaveChanges();

        SetBlendshapesToFile();

        _openedFile.SaveTo(_openedFilePath);
    }

    public void TransformModeButtonClickHandler()
    {
        switch (TransformGizmo.type)
        {
            case TransformType.Move:
                TransformGizmo.type = TransformType.Rotate;
                break;
            case TransformType.Rotate:
                TransformGizmo.type = TransformType.Scale;
                break;
            case TransformType.Scale:
                TransformGizmo.type = TransformType.Move;
                break;
        }
    }

    public void ResetCurrentBlendshapeButtonClickHandler()
    {
        TransformGizmo.ClearUndoRedo();
        SetBonesToDefault();
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

    private void SelectBasicBlendshape()
    {
        TransformGizmo.ClearUndoRedo();

        SaveChanges();

        _selectedblendShapeName = WowVrcFileData.BlendshapeData.basicBlendshapeName;

        SetVerticesToDefault();
        SetBonesToDefault();
        SetBonesFromSelectedBlendshape();

        BlendshapesListbox.SetSelectedListboxItemButton(_basicBlendshapeButton, selectedColor);
    }

    private void SelectBlendshape(string blendshapeName)
    {
        TransformGizmo.ClearUndoRedo();

        SaveChanges();

        var data = _blendshapeData[blendshapeName];
        _selectedblendShapeName = blendshapeName;

        SetVerticesToBasic();
        SetBonesToDefault();
        SetBonesFromSelectedBlendshape();

        BlendshapesListbox.SetSelectedListboxItemButton(data.ListboxButton, selectedColor);
    }

    private void InitBlendShapes()
    {
        _blendshapeData = new Dictionary<string, BlendshapeData>(_blendshapeNames.Length + 1);

        BlendshapesListbox.AddNewListboxItem("BASIC", out _basicBlendshapeButton);
        _basicBlendshapeButton.onClick.AddListener(SelectBasicBlendshape);
        _blendshapeData[WowVrcFileData.BlendshapeData.basicBlendshapeName] = new BlendshapeData()
        {
            Name = WowVrcFileData.BlendshapeData.basicBlendshapeName,
            Bones = new Dictionary<string, BoneData>(),
            ListboxButton = _basicBlendshapeButton
        };

        foreach (var blendshapeName in _blendshapeNames)
        {
            Button button;
            BlendshapesListbox.AddNewListboxItem(blendshapeName, out button);

            _blendshapeData[blendshapeName] = new BlendshapeData()
            {
                Name = blendshapeName,
                Bones = new Dictionary<string, BoneData>(),
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

    private void SaveChanges()
    {
        SaveBonesToSelectedBlendshape();

        // Если сейчас выбран basic блендшейп - запекаем в _basicVertexPositions измененные вершины на основании него
        if (_selectedblendShapeName == WowVrcFileData.BlendshapeData.basicBlendshapeName)
        {
            var basicVertexPositionChanges = BlendShapeBaker.BakeBlendShape(
                _character.WowObject.Mesh.Vertices,
                _character.WowObject.Bones,
                ConvertBlendshapeBonesToFile(_blendshapeData[WowVrcFileData.BlendshapeData.basicBlendshapeName].Bones));

            _defaultVertexPositions.CopyTo(_basicVertexPositions, 0);
            foreach (var basicVertexPosition in basicVertexPositionChanges)
                _basicVertexPositions[basicVertexPosition.Key] = new Vector3(basicVertexPosition.Value.X, basicVertexPosition.Value.Y, basicVertexPosition.Value.Z);
        }
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

                var blendshapeData = _blendshapeData[_selectedblendShapeName];

                var isSavingBoneDefault = boneData.Equals(_defaultBones[bone.name]);
                if (isSavingBoneDefault)
                    blendshapeData.Bones.Remove(bone.name);
                else
                    blendshapeData.Bones[bone.name] = boneData;
            }
        }
    }

    private void SetVerticesToDefault()
    {
        _characterMesh.vertices = _defaultVertexPositions.ToArray();
    }

    private void SetVerticesToBasic()
    {
        _characterMesh.vertices = _basicVertexPositions.ToArray();
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
        _openedFile.Blendshapes = _blendshapeData.Values
            .Select(x => new WowVrcFileData.BlendshapeData()
            {
                Name = x.Name,
                Bones = ConvertBlendshapeBonesToFile(x.Bones)
            })
            .ToArray();
    }

    private void GetBlendshapesFromFile()
    {
        if (_openedFile.Blendshapes == null)
            return;

        foreach (var blendshapeFromFile in _openedFile.Blendshapes)
            _blendshapeData[blendshapeFromFile.Name].Bones = ConvertBlendshapeBonesFromFile(blendshapeFromFile.Bones);
    }

    private WowVrcFileData.BlendshapeData.BoneData[] ConvertBlendshapeBonesToFile(Dictionary<string, BoneData> bones)
    {
        return bones.Select(bone => new WowVrcFileData.BlendshapeData.BoneData()
        {
            Name = bone.Key,
            LocalTransform = new WowTransform()
            {
                position = new Vec3(bone.Value.LocalPosition.x, bone.Value.LocalPosition.y, bone.Value.LocalPosition.z),
                rotation = new Vec4(bone.Value.LocalRotation.x, bone.Value.LocalRotation.y, bone.Value.LocalRotation.z, bone.Value.LocalRotation.w),
                scale = new Vec3(bone.Value.LocalScale.x, bone.Value.LocalScale.y, bone.Value.LocalScale.z)
            }
        }).ToArray();
    }

    private Dictionary<string, BoneData> ConvertBlendshapeBonesFromFile(WowVrcFileData.BlendshapeData.BoneData[] bones)
    {
        return bones.ToDictionary(x => x.Name, x => new BoneData()
        {
            LocalPosition = new Vector3(x.LocalTransform.position.X, x.LocalTransform.position.Y, x.LocalTransform.position.Z),
            LocalRotation = new Quaternion(x.LocalTransform.rotation.X, x.LocalTransform.rotation.Y, x.LocalTransform.rotation.Z, x.LocalTransform.rotation.W),
            LocalScale = new Vector3(x.LocalTransform.scale.X, x.LocalTransform.scale.Y, x.LocalTransform.scale.Z)
        });
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

    private Button _basicBlendshapeButton;

    private Transform[] _bones;

    /// <summary>
    /// Дефолтное состояние костей (то, которое получилось при загрузке перса)
    /// Ключ - название кости
    /// Тут есть только кости, которые могут подвергнуться трансформации (из списка костей слева то есть)
    /// </summary>
    private Dictionary<string, BoneData> _defaultBones;

    private Vector3[] _defaultVertexPositions;
    private Vector3[] _basicVertexPositions;

    private Transform _selectedBone;
    private string _selectedblendShapeName;

    private CharacterBuilder.Character _character;
    private Mesh _characterMesh;
    private GameObject _characterBonesRoot;

    private string _openedFilePath;
    private WowVrcFile _openedFile;

    private static readonly Color selectedColor = new Color(0.7f, 0.7f, 0.9f);

    private class BlendshapeData
    {
        public string Name { get; set; }

        public Button ListboxButton { get; set; }

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