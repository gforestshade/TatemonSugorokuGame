using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ クレジットシーンのクラス
	/// </summary>
	public class CreditSMScene : MainSMScene {
		public float _bgmSeconds	{ get; private set; }



		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public CreditSMScene() {

			// シーン初期化
			_enterEvent.AddLast( async canceler => {
				var audioManager = SMServiceLocator.Resolve<SMAudioManager>();
				var bgm = await audioManager._bgm.Load( SMBGM.Result );
				_bgmSeconds = bgm._audioClip.length;
				audioManager.Play( SMBGM.Result ).Forget();
				
				UTask.Void( async () => {
					await UTask.Delay( _asyncCancelerOnExit, ( int )( ( _bgmSeconds - 3 ) * 1000 ) );
					audioManager.Stop<SMBGM>().Forget();
				} );
			} );

			// シーン終了
			_exitEvent.AddFirst( async canceler => {
				await UTask.DontWait();
			} );

			// 更新（非同期的に実行）
			_asyncUpdateEvent.AddLast( async canceler => {
				await UTask.DontWait();
			} );

			// 物理更新
			_fixedUpdateEvent.AddLast().Subscribe( _ => {
			} );

			// 更新
			_updateEvent.AddLast().Subscribe( _ => {
			} );

			// 後更新
			_lateUpdateEvent.AddLast().Subscribe( _ => {
			} );

			// 破棄
			_disposables.AddFirst( () => {
			} );
		}
	}
}