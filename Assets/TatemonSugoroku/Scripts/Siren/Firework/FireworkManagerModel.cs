using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ 花火管理のモデルクラス
	///		たてもん回転や、花火UIが参照します。
	/// </summary>
	///========================================================================================================
	public class FireworkManagerModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>花火の最大数</summary>
		public const int MAX_COUNT = 5;
		/// <summary>花火の打ち上げ時刻</summary>
		public const int LAUNCH_HOUR = 19;
		/// <summary>非同期停止の識別子</summary>
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		/// <summary>打ち上げ通知イベント</summary>
		public readonly Subject<Unit> _launch = new Subject<Unit>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public FireworkManagerModel() {
			UTask.Void( async () => {
				var allModelManager = await SMServiceLocator.WaitResolve<AllModelManager>( _canceler );
				var day = allModelManager.Get<DayModel>();
				day._hour
					.Where( h => h >= LAUNCH_HOUR )
					.Subscribe( h => {
						_launch.OnNext( Unit.Default );
					} );
			} );

			_disposables.AddFirst( () => {
				_canceler.Dispose();
				_launch.Dispose();
			} );
		}
	}
}