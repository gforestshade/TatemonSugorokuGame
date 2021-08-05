using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火管理のモデルクラス
	/// </summary>
	public class FireworkManagerModel : SMStandardBase, IModel {
		public const int MAX_COUNT = 5;
		public const int LAUNCH_HOUR = 19;
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		public readonly Subject<Unit> _launch = new Subject<Unit>();



		public FireworkManagerModel() {
			UTask.Void( async () => {
				var allModelManager = await SMServiceLocator.WaitResolve<AllModelManager>( _canceler );
				var day = allModelManager.Get<DayModel>();
				day._hour
					.Select( h => {
						SMLog.Debug( $"ゲーム内時刻 : {h}" );
						return h;
					} )
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