using UnityEngine;
using UnityEngine.UI;


namespace TatemonSugoroku.Scripts
{
    [RequireComponent(typeof(Image))]
    public class SwitchableImage : MonoBehaviour
    {
        [SerializeField]
        Sprite[] _Sprites;

        Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void Switch(int index)
        {
            image.sprite = _Sprites[index];
        }
    }
}
