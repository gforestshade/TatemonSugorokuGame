using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印のモデルクラス
	/// </summary>
	public class MoveArrowModel : SMStandardBase {
		public string _name				{ get; private set; }
		public MoveArrowType _arrowType	{ get; private set; }
		public Quaternion _rotation		{ get; private set; }

		public readonly ReactiveProperty<TileModel> _placeTile = new ReactiveProperty<TileModel>();



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