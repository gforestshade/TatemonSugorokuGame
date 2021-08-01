using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage;
using SubmarineMirage.Service;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;

/// <summary>
/// ■ タイトルシーンのクラス
/// </summary>
public class TitleSMScene : MainSMScene {

	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public TitleSMScene() {

		// 決定キーが押された場合、ゲームシーンへ遷移
		var inputManager = SMServiceLocator.Resolve<SMInputManager>();
		inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast()
			.Where( _ => _isEntered )
			.Subscribe( _ => {
				_owner.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
			} );


		// 終了キーが押された場合、ゲーム終了
		inputManager.GetKey( SMInputKey.Quit )._enabledEvent.AddLast()
			.Where( _ => _isEntered )
			.Subscribe( _ => {
				SubmarineMirageFramework.Shutdown();
			} );


		// シーン初期化
		_enterEvent.AddLast( async canceler => {
			SMLog.Debug( $"Enterキーでゲームシーンへ、ESCキーでアプリを終了します。", SMLogTag.Scene );
			await UTask.DontWait();
		} );



		// シーン終了
		_exitEvent.AddFirst( async canceler => {
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