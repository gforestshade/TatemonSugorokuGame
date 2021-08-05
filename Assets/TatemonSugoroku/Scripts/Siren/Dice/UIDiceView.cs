using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火画面の描画クラス
	/// </summary>
	public class UIDiceView : SMStandardMonoBehaviour {
		DiceModel _model { get; set; }
		CanvasGroup _group { get; set; }



		void Start() {
			_group = GetComponent<CanvasGroup>();
			_group.alpha = 0;
			_group.blocksRaycasts = false;
			_group.interactable = false;
		}

		protected override void StartAfterInitialize() {
			_model = SMServiceLocator.Resolve<AllModelManager>().Get<DiceModel>();

			_model._state.Subscribe( s => {
				switch ( s ) {
					case DiceState.Hide:
					case DiceState.Roll:
						_group.alpha = 0;
						_group.blocksRaycasts = false;
						_group.interactable = false;
						break;

					case DiceState.Rotate:
						_group.alpha = 1;
						_group.blocksRaycasts = true;
						_group.interactable = true;
						break;
				}
			} );

			var button = GetComponentInChildren<Button>();
			button.onClick.AddListener( () => {
				_model.SetPower(
					new Vector3(
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 )
					).normalized * 10
				);
				_model.ChangeState( DiceState.Roll );
			} );
		}
	}
}