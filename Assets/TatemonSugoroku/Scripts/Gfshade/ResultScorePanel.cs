using UnityEngine;


namespace TatemonSugoroku.Scripts
{
    public class ResultScorePanel : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _Score;
        public TMPro.TextMeshProUGUI Score => _Score;

        [SerializeField]
        SwitchableImage _Image;
        public SwitchableImage Image => _Image;
    }
}