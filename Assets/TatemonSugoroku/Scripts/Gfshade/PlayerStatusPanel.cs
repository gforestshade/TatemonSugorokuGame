using UnityEngine;


namespace TatemonSugoroku.Scripts
{
    public class PlayerStatusPanel : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _Name;
        public TMPro.TextMeshProUGUI Name => _Name;

        [SerializeField]
        TMPro.TextMeshProUGUI _Score;
        public TMPro.TextMeshProUGUI Score => _Score;

        [SerializeField]
        TMPro.TextMeshProUGUI _Tatemon;
        public TMPro.TextMeshProUGUI Tatemon => _Tatemon;
    }
}