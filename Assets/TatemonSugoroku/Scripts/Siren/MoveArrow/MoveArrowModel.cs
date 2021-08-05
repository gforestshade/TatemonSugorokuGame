using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
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
			} );
		}
	}
}