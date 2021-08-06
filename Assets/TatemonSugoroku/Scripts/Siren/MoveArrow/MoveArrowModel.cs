using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ 移動矢印のモデルクラス
	/// </summary>
	///========================================================================================================
	public class MoveArrowModel : SMStandardBase {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>ゲームオブジェクトに付ける名前</summary>
		public string _name				{ get; private set; }
		/// <summary>矢印のタイプ</summary>
		public MoveArrowType _arrowType	{ get; private set; }
		/// <summary>矢印の回転角度</summary>
		public Quaternion _rotation		{ get; private set; }

		/// <summary>非同期停止の識別子</summary>
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();
		/// <summary>タイル管理者のモデル</summary>
		TileManagerModel _tileManagerModel { get; set; }

		/// <summary>配置タイルのモデル、配置切り替え検知により、更新</summary>
		public readonly ReactiveProperty<TileModel> _placeTile = new ReactiveProperty<TileModel>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public MoveArrowModel( MoveArrowType arrowType ) {
			_arrowType = arrowType;
			_rotation = MoveArrowManagerModel.ARROW_TYPE_TO_ROTATION[_arrowType];

			_name = $"MoveArrow {_arrowType}";

			_disposables.AddFirst( () => {
				_placeTile.Dispose();
				_canceler.Dispose();
			} );

			UTask.Void( async () => {
				var allModelManager = await SMServiceLocator.WaitResolve<AllModelManager>( _canceler );
				_tileManagerModel = allModelManager.Get<TileManagerModel>();
			} );
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル場所に、矢印を配置
		/// </summary>
		public void Place( TileModel tileModel ) {
			_placeTile.Value = tileModel;
		}

		/// <summary>
		/// ● タイル番号に、矢印を配置
		/// </summary>
		public void Place( int tileID ) {
			var tile = _tileManagerModel.GetModel( tileID );
			Place( tile );
		}

		/// <summary>
		/// ● タイル位置に、矢印を配置
		/// </summary>
		public void Place( Vector2Int tilePosition ) {
			var tile = _tileManagerModel.GetModel( tilePosition );
			Place( tile );
		}
	}
}