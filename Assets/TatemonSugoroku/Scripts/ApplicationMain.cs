using UnityEngine;
using DG.Tweening;
using UniRx;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage.Scene;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
using TatemonSugoroku.Scripts.Setting;
using SystemTask = System.Threading.Tasks.Task;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ ゲーム起動開始のクラス
	///		ゲーム起動時に、このクラスから実行される。
	/// </summary>
	public static class ApplicationMain {

		/// <summary>
		/// ● 外部ライブラリを初期化
		/// </summary>
		static async UniTask InitializePlugin() {
			// DOTWeen初期化
			DOTween.Init(
				false,
				!SMDebugManager.IS_DEVELOP,
				LogBehaviour.Verbose
			);
			DOTween.defaultAutoPlay = AutoPlay.None;

			Resources.Load<GameObject>( "Prefabs/EffectTap1" ).Instantiate();

			await SystemTask.Delay( 1 );

			// UniTask初期化
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Error;
		}

		/// <summary>
		/// ● 設定データを登録
		/// </summary>
		static async UniTask RegisterSettings() {
			// 入力設定を登録
			SMServiceLocator.Register<BaseSMInputSetting>( new SMInputSetting() );
			// データ設定を登録
			SMServiceLocator.Register<BaseSMDataSetting>( new SMDataSetting() );
			// シーン設定を登録
			SMServiceLocator.Register<BaseSMSceneSetting>( new SMSceneSetting() );

			await UTask.DontWait();
		}

		/// <summary>
		/// ● 実行開始
		///		ゲームは、ここから開始される。
		/// </summary>
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static async void Main() {
			var framework = SMServiceLocator.Register( new SubmarineMirageFramework() );
			await framework.TakeOff( InitializePlugin, RegisterSettings );
		}
	}
}