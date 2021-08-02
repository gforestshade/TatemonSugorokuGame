using UnityEngine;

namespace TatemonSugoroku.Scripts
{
    public class AkioModelChecker : MonoBehaviour
    {
        private readonly FieldModel _fieldModel = new FieldModel();

        void Start()
        {
            _fieldModel.Initialize(2);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
