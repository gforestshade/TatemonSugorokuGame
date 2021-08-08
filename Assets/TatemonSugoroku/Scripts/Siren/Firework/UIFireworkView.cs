using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火画面の描画クラス
	/// </summary>
	public class UIFireworkView : SMStandardMonoBehaviour {
		CanvasGroup _group { get; set; }



		void Start() {
			_group = GetComponent<CanvasGroup>();
			_group.alpha = 0;
			_group.blocksRaycasts = false;

			var fireworkManager = FindObjectOfType<FireworkManagerView>();
//			fireworkManager._launchEvent.Subscribe( _ => Launch() );
		}



		void Launch() {
			_group.alpha = 1;
			_group.blocksRaycasts = false;
		}
	}
}