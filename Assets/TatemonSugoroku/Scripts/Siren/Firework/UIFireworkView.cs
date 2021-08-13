using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火画面の描画クラス
	/// </summary>
	public class UIFireworkView : SMStandardMonoBehaviour {
		CanvasGroup _group { get; set; }
		SMAudioManager _audioManager { get; set; }



		void Start() {
			_group = GetComponent<CanvasGroup>();

			var button = GetComponentInChildren<Button>();
			button.onClick.AddListener( () => {
				_audioManager?.Play( SMSE.Decide ).Forget();
				SetActive( false );
			} );

			var fireworkManager = FindObjectOfType<FireworkManagerView>();
			fireworkManager._launchEvent.Subscribe( _ => SetActive( true ) );

			UTask.Void( async () => {
				_audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
			} );

			SetActive( false );
		}



		void SetActive( bool isActive ) {
			_group.alpha = isActive ? 1 : 0;
			_group.blocksRaycasts = isActive;
		}
	}
}