using RuntimeGizmos;
using UnityEngine;
using WowModelExporterUnityPlugin;

public class MainController : MonoBehaviour
{
    public void LoadCharacterButtonClickHandler()
    {
        if (_characterGo == null)
        {
            _characterGo = new CharacterBuilder().Build(WowheadModelLoader.WhRace.HUMAN, WowheadModelLoader.WhGender.MALE);

            var jaw = GameObject.Find("jaw");
            var collider = jaw.AddComponent<SphereCollider>();
            collider.radius = 0.1f;
            
        }
    }

    public void TranslateRotateButtonClickHandler()
    {
        var gizmo = Camera.main.GetComponent<TransformGizmo>();
        gizmo.type = gizmo.type == TransformType.Move ? TransformType.Rotate : TransformType.Move;
    }

    private GameObject _characterGo { get; set; }
}