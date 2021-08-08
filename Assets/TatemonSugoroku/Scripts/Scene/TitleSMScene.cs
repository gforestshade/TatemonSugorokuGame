using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイトルシーンのクラス
	/// </summary>
	public class TitleSMScene : MainSMScene {

		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public TitleSMScene() {
			var audioManager = SMServiceLocator.Resolve<SMAudioManager>();

			// シーン初期化
			_enterEvent.AddLast( async canceler => {
//				await audioManager.StopAll();
				audioManager.Play( SMBGM.Title ).Forget();
				await UTask.DontWait();
			} );

			// シーン終了
			_exitEvent.AddFirst( async canceler => {
//				audioManager.StopAll().Forget();
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