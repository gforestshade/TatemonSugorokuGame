using UnityEngine;


namespace TatemonSugoroku.Scripts
{
    public class TatemonRotator : MonoBehaviour
    {
        public float Speed { get; set; } = 30f;
        public bool IsRotate { get; set; } = false;
        public Transform Camera { get; set; }
        public bool IsLookAtCamera { get; set; } = true;

        private void Awake()
        {
            Camera = FindObjectOfType<Camera>().transform;
        }

        void Update()
        {
            if (IsLookAtCamera)
            {
                var cameraPosition = Camera.position;
                var lookat = new Vector3(cameraPosition.x, 0f, cameraPosition.z);
                transform.LookAt(lookat);
            }
            else if (IsRotate)
            {
                float dy = Time.deltaTime * Speed;
                transform.Rotate(0f, dy, 0f);
            }
        }
    }
}
