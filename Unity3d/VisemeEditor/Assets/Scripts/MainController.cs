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
        _defaultVertexNormals = new Vector3[_characterMesh.vertices.Length];
        _characterMesh.normals.CopyTo(_defaultVertexNormals, 0);

        _basicVertexPositions = new Vector3[_characterMesh.vertices.Length];
        _characterMesh.vertices.CopyTo(_basicVertexPositions, 0);
        _basicVertexNormals = new Vector3[_characterMesh.vertices.Length];
        _characterMesh.normals.CopyTo(_basicVertexNormals, 0);

        SetBoneListboxItems();

        ClearBlendshapes();
        GetBlendshapesFromFile();

        SelectBlendshape(WowVrcFileData.BlendshapeData.basicBlendshapeName, false);
    }

    public void SaveFileButtonClickHandler()
    {
        if (_openedFile == null)
            return;

        UpdateChanges();

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

    public void ResetCurrentBoneButtonClickHandler()
    {
        TransformGizmo.ClearUndoRedo();
        SetBoneToDefault(_selectedBone);
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

    public void GenerateFromCatsButtonClickHandler()
    {
        if (_openedFile == null)
            return;

        UpdateChanges();

        var res = new Dictionary<string, Dictionary<string, BoneData>>();

        foreach (var catsVisemeMixerDataElement in _catsVisemeMixerData)
        {
            var blendshapeBoneChangesWithIntensity = catsVisemeMixerDataElement.Value
                .Select(x => new BlendShapeUtility.BlendshapeBoneChangesWithIntensity()
                {
                    BlendShapeBoneChanges = ConvertBlendshapeBonesToFile(_blendshapeData[x.Key].Bones),
                    Intensity = x.Value
                })
                .ToArray();

            res.Add(catsVisemeMixerDataElement.Key, ConvertBlendshapeBonesFromFile(BlendShapeUtility.MixBlendshapeBoneChanges(blendshapeBoneChangesWithIntensity)));
        }

        foreach (var resItem in res)
            _blendshapeData[resItem.Key].Bones = resItem.Value;

        // Если меняем на этот же блендшейп, то обновлять ненужно, т.к. смысл этой операции наоборот из измененного состояния обновить отображение (иначе получится затирание данных)
        SelectBlendshape(_selectedblendShapeName, false);
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

    private void SelectBlendshape(string blendshapeName, bool updateCurrentChangesBeforeSelecting = true)
    {
        TransformGizmo.ClearUndoRedo();

        if (updateCurrentChangesBeforeSelecting)
            UpdateChanges();

        if (blendshapeName == WowVrcFileData.BlendshapeData.basicBlendshapeName)
        {
            _selectedblendShapeName = WowVrcFileData.BlendshapeData.basicBlendshapeName;

            SetVerticesToDefault();
            SetBonesToDefault();
            UpdateBonesFromSelectedBlendshape();

            BlendshapesListbox.SetSelectedListboxItemButton(_basicBlendshapeButton, selectedColor);
        }
        else
        {
            var data = _blendshapeData[blendshapeName];
            _selectedblendShapeName = blendshapeName;

            SetVerticesToBasic();
            SetBonesToDefault();
            UpdateBonesFromSelectedBlendshape();

            BlendshapesListbox.SetSelectedListboxItemButton(data.ListboxButton, selectedColor);
        }
    }

    private void InitBlendShapes()
    {
        _blendshapeData = new Dictionary<string, BlendshapeData>(_blendshapeNames.Length + 1);

        BlendshapesListbox.AddNewListboxItem("BASIC", out _basicBlendshapeButton);
        _basicBlendshapeButton.onClick.AddListener(() => SelectBlendshape(WowVrcFileData.BlendshapeData.basicBlendshapeName));
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

    private void UpdateChanges()
    {
        UpdateBonesToSelectedBlendshape();

        // Если сейчас выбран basic блендшейп - запекаем в _basicVertexPositions измененные вершины на основании него
        if (_selectedblendShapeName == WowVrcFileData.BlendshapeData.basicBlendshapeName)
        {
            var basicVertexChanges = BlendShapeUtility.BakeBlendShape(
                _character.WowObject.MainMesh.Vertices,
                _character.WowObject.Bones,
                ConvertBlendshapeBonesToFile(_blendshapeData[WowVrcFileData.BlendshapeData.basicBlendshapeName].Bones));

            _defaultVertexPositions.CopyTo(_basicVertexPositions, 0);
            _defaultVertexNormals.CopyTo(_basicVertexNormals, 0);
            foreach (var basicVertexChange in basicVertexChanges)
            {
                _basicVertexPositions[basicVertexChange.Key] = new Vector3(basicVertexChange.Value.Position.X, basicVertexChange.Value.Position.Y, basicVertexChange.Value.Position.Z);
                _basicVertexNormals[basicVertexChange.Key] = new Vector3(basicVertexChange.Value.Normal.X, basicVertexChange.Value.Normal.Y, basicVertexChange.Value.Normal.Z);
            }
        }
    }

    private void SetVerticesToDefault()
    {
        _characterMesh.vertices = _defaultVertexPositions.ToArray();
        _characterMesh.normals = _defaultVertexNormals.ToArray();
    }

    private void SetVerticesToBasic()
    {
        _characterMesh.vertices = _basicVertexPositions.ToArray();
        _characterMesh.normals = _basicVertexNormals.ToArray();
    }

    private void SetBonesToDefault()
    {
        if (_bones == null)
            return;

        foreach (var bone in _bones)
            SetBoneToDefault(bone);
    }

    private void SetBoneToDefault(Transform bone)
    {
        if (bone == null)
            return;

        var defaultBone = _defaultBones[bone.name];

        bone.localPosition = defaultBone.LocalPosition;
        bone.localRotation = Quaternion.identity;
        bone.localScale = Vector3.one;
    }

    private void UpdateBonesFromSelectedBlendshape()
    {
        if (_selectedblendShapeName != null)
        {
            var bones = _blendshapeData[_selectedblendShapeName].Bones;

            foreach (var bone in _bones)
            {
                var defaultBone = _defaultBones[bone.name];

                BoneData boneData;
                if (bones.TryGetValue(bone.name, out boneData))
                {
                    bone.localPosition = defaultBone.LocalPosition + boneData.LocalPosition;
                    bone.localRotation = boneData.LocalRotation;
                    bone.localScale = boneData.LocalScale;
                }
            }
        }
    }

    private void UpdateBonesToSelectedBlendshape()
    {
        if (_selectedblendShapeName != null)
        {
            foreach (var bone in _bones)
            {
                var defaultBone = _defaultBones[bone.name];

                var boneData = new BoneData()
                {
                    LocalPosition = bone.localPosition - defaultBone.LocalPosition,
                    LocalRotation = bone.localRotation,
                    LocalScale = bone.localScale
                };

                var blendshapeData = _blendshapeData[_selectedblendShapeName];

                var isSavingBoneDefault = boneData.Equals(_identityBone);
                if (isSavingBoneDefault)
                    blendshapeData.Bones.Remove(bone.name);
                else
                    blendshapeData.Bones[bone.name] = boneData;
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
            .Where(x => x.Bones.Length > 0)
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
        return bones
            .Where(x => !x.Value.Equals(_identityBone))
            .Select(bone => new WowVrcFileData.BlendshapeData.BoneData()
            {
                Name = bone.Key,
                LocalTransform = new WowTransform()
                {
                    position = new Vec3(bone.Value.LocalPosition.x, bone.Value.LocalPosition.y, bone.Value.LocalPosition.z),
                    rotation = new Vec4(bone.Value.LocalRotation.x, bone.Value.LocalRotation.y, bone.Value.LocalRotation.z, bone.Value.LocalRotation.w),
                    scale = new Vec3(bone.Value.LocalScale.x, bone.Value.LocalScale.y, bone.Value.LocalScale.z)
                }
            })
            .ToArray();
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

    private static readonly string[] _blendshapeNames = new []
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

    private static readonly Dictionary<string, Dictionary<string, float>> _catsVisemeMixerData = new Dictionary<string, Dictionary<string, float>>
    {
        {
            "vrc.v_aa", new Dictionary<string, float>
            {
                { "CATS.AA", 0.9998f }
            }
        },
        {
            "vrc.v_ch", new Dictionary<string, float>
            {
                { "CATS.CH", 0.9996f }
            }
        },
        {
            "vrc.v_dd", new Dictionary<string, float>
            {
                { "CATS.AA", 0.3f },
                { "CATS.CH", 0.7f }
            }
        },
        {
            "vrc.v_e", new Dictionary<string, float>
            {
                { "CATS.CH", 0.7f },
                { "CATS.OH", 0.3f }
            }
        },
        {
            "vrc.v_ff", new Dictionary<string, float>
            {
                { "CATS.AA", 0.2f },
                { "CATS.CH", 0.4f }
            }
        },
        {
            "vrc.v_ih", new Dictionary<string, float>
            {
                { "CATS.AA", 0.5f },
                { "CATS.CH", 0.2f }
            }
        },
        {
            "vrc.v_kk", new Dictionary<string, float>
            {
                { "CATS.AA", 0.7f },
                { "CATS.CH", 0.4f }
            }
        },
        {
            "vrc.v_nn", new Dictionary<string, float>
            {
                { "CATS.AA", 0.2f },
                { "CATS.CH", 0.7f }
            }
        },
        {
            "vrc.v_oh", new Dictionary<string, float>
            {
                { "CATS.AA", 0.2f },
                { "CATS.OH", 0.8f }
            }
        },
        {
            "vrc.v_ou", new Dictionary<string, float>
            {
                { "CATS.OH", 0.9994f }
            }
        },
        {
            "vrc.v_pp", new Dictionary<string, float>
            {
                { "CATS.AA", 0.0004f },
                { "CATS.OH", 0.0004f }
            }
        },
        {
            "vrc.v_rr", new Dictionary<string, float>
            {
                { "CATS.CH", 0.5f },
                { "CATS.OH", 0.3f }
            }
        },
        {
            "vrc.v_sil", new Dictionary<string, float>
            {
                { "CATS.AA", 0.0002f },
                { "CATS.CH", 0.0002f }
            }
        },
        {
            "vrc.v_ss", new Dictionary<string, float>
            {
                { "CATS.CH", 0.8f }
            }
        },
        {
            "vrc.v_th", new Dictionary<string, float>
            {
                { "CATS.AA", 0.4f },
                { "CATS.OH", 0.15f }
            }
        }
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

    private BoneData _identityBone = new BoneData()
    {
        LocalPosition = Vector3.zero,
        LocalRotation = Quaternion.identity,
        LocalScale = Vector3.one
    };

    private Vector3[] _defaultVertexPositions;
    private Vector3[] _defaultVertexNormals;

    private Vector3[] _basicVertexPositions;
    private Vector3[] _basicVertexNormals;

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

            return
                (Mathf.Approximately(LocalPosition.x, other.LocalPosition.x) && Mathf.Approximately(LocalPosition.y, other.LocalPosition.y) && Mathf.Approximately(LocalPosition.z, other.LocalPosition.z)) &&
                (Mathf.Approximately(LocalRotation.x, other.LocalRotation.x) && Mathf.Approximately(LocalRotation.y, other.LocalRotation.y) && Mathf.Approximately(LocalRotation.z, other.LocalRotation.z) && Mathf.Approximately(LocalRotation.w, other.LocalRotation.w)) &&
                (Mathf.Approximately(LocalScale.x, other.LocalScale.x) && Mathf.Approximately(LocalScale.y, other.LocalScale.y) && Mathf.Approximately(LocalScale.z, other.LocalScale.z));
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}