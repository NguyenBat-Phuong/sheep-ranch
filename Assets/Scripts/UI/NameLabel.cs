using UnityEngine;
using TMPro;

public class NameLabel : MonoBehaviour
{
    public string characterName = "";

    private static readonly string[] sheepNames = {
        "Bông", "Mây", "Nàng", "Bé Trắng",
        "Mịn", "Bỏng Ngô", "Mochi",
        "Béo", "Bánh", "Tuyết", "Len", "Cỏn Con"
    };

    private TMP_Text label;

    void Start()
    {
        label = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(characterName))
            characterName = sheepNames[Random.Range(0, sheepNames.Length)];
        label.text = characterName;
    }
}
