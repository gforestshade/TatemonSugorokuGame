using System.Linq;
using UnityEngine;
using UniRx;
using DG.Tweening;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火画面の描画クラス
	/// </summary>
	public class UIFireworkView : SMStandardMonoBehaviour {
		FireworkManagerModel _model { get; set; }
		CanvasGroup _group { get; set; }



		void Start() {
			_group = GetComponent<CanvasGroup>();
			_group.alpha = 0;
			_group.blocksRaycasts = false;
		}

		protected override void StartAfterInitialize() {
			_model = AllModelManager.s_instance.Get<FireworkManagerModel>();
			_model._launch.Subscribe( _ => {
				_group.alpha = 1;
				_group.blocksRaycasts = false;
			} );
		}
	}
}