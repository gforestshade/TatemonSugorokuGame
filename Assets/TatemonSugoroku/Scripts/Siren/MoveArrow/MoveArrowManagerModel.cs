using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印管理のモデルクラス
	/// </summary>
	public class MoveArrowManagerModel : SMStandardBase, IModel {
		public static readonly Dictionary<MoveArrowType, Quaternion> ARROW_TYPE_TO_ROTATION
			= new Dictionary<MoveArrowType, Quaternion>() {
				{ MoveArrowType.Down,   Quaternion.Euler(0, 270, 0) },
				{ MoveArrowType.Left,   Quaternion.Euler(0, 0, 0) },
				{ MoveArrowType.Right,  Quaternion.Euler(0, 180, 0) },
				{ MoveArrowType.Up,     Quaternion.Euler(0, 90, 0) },
			};

		public readonly Dictionary<MoveArrowType, MoveArrowModel> _models;



		public MoveArrowManagerModel() {
			_models = EnumUtils.GetValues<MoveArrowType>().ToDictionary(
				type => type,
				type => new MoveArrowModel( type )
			);

			_disposables.AddFirst( () => {
				_models.ForEach( pair => pair.Value.Dispose() );
			} );
		}



		public MoveArrowModel GetModel( MoveArrowType type )
			=> _models.GetOrDefault( type );
	}
}