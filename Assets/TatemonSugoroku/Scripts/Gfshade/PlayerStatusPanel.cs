using UnityEngine;


namespace TatemonSugoroku.Scripts
{
    public class PlayerStatusPanel : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _Score;
        public TMPro.TextMeshProUGUI Score => _Score;

        [SerializeField]
        TMPro.TextMeshProUGUI _Tatemon;
        public TMPro.TextMeshProUGUI Tatemon => _Tatemon;

        [SerializeField]
        TMPro.TextMeshProUGUI _MaxTatemon;
        public TMPro.TextMeshProUGUI MaxTatemon => _MaxTatemon;
    }
}