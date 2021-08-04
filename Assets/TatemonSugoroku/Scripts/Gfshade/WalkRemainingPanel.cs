using UnityEngine;


namespace TatemonSugoroku.Scripts
{
    public class WalkRemainingPanel : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _Text0;
        [SerializeField]
        TMPro.TextMeshProUGUI _Text1;

        public TMPro.TextMeshProUGUI Current { get; private set; }
        public TMPro.TextMeshProUGUI Reserved { get; private set; }

        private void Awake()
        {
            Current = _Text0;
            Reserved = _Text1;
        }

        public void Swap()
        {
            TMPro.TextMeshProUGUI t = Current;
            Current = Reserved;
            Reserved = t;
        }
    }
}