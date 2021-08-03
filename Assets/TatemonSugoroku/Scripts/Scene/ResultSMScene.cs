using UniRx;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;

/// <summary>
/// ■ 勝敗リザルトシーンのクラス
/// </summary>
public class ResultSMScene : MainSMScene {

	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public ResultSMScene() {

		// シーン初期化
		_enterEvent.AddLast( async canceler => {
			await UTask.DontWait();
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