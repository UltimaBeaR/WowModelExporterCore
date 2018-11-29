using UnityEngine;
using WowModelExporterUnityPlugin;

public class MainController : MonoBehaviour
{
    void Start()
    {
        var character = new CharacterBuilder().Build(WowheadModelLoader.WhRace.HUMAN, WowheadModelLoader.WhGender.MALE);
    }
}
