using UnityEngine;
using UnityEngine.UI;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming

public class ShowFPS : MonoBehaviour
{
    public Text Text;

    void Update()
    {
        Text.text = ( 1.0f / Time.deltaTime).ToString("##.#");
    }
}
