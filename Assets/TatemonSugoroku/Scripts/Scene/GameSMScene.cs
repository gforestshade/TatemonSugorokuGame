using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;

/// <summary>
/// ■ ゲームシーンのクラス
/// </summary>
public class GameSMScene : MainSMScene {

	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public GameSMScene() {

		// シーン初期化
		_enterEvent.AddLast( async canceler => {
			// ゲームシーンに必要なデータを読む
			var allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
			var itemDatas = allDataManager.Get<string, SampleItemData>();
			var data1 = itemDatas.Get( "パンフレット" );
			var data2 = itemDatas.Get( "たてもん" );
			var playData = allDataManager.Get<PlayDataManager>()._currentData;

			SMLog.Debug( $"ゲームシーンに必要なデータを、読み込みました。", SMLogTag.Scene );
			SMLog.Debug( $"{data1._name} : \n{data1._explanation}", SMLogTag.Data );
			SMLog.Debug( $"{data2._name} : \n{data2._explanation}", SMLogTag.Data );
			SMLog.Debug( $"勝ち数 : {playData._winCount}\n負け数 : {playData._loseCount}", SMLogTag.Data );

			await UTask.DontWait();
		} );


		// 非同期更新
		_asyncUpdateEvent.AddLast( async canceler => {
			// 5秒後に、成功か失敗シーンに、ランダム遷移
			SMLog.Debug( $"5秒後に、ランダムにシーン遷移します。", SMLogTag.Scene );
			await UTask.Delay( canceler, 5000 );

			if ( Random.value < 0.5f ) {
				_owner.GetFSM<MainSMScene>().ChangeState<GameClearSMScene>().Forget();
			} else {
				_owner.GetFSM<MainSMScene>().ChangeState<GameOverSMScene>().Forget();
			}
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