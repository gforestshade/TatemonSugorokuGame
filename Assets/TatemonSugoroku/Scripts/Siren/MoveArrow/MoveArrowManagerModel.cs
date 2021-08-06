using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ 移動矢印管理のモデルクラス
	/// </summary>
	///========================================================================================================
	public class MoveArrowManagerModel : SMStandardBase, IModel
	{
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>矢印タイプから、角度に変換</summary>

		// 順番変えさせてもらいます。(by Akio)

		public static readonly Dictionary<MoveArrowType, Quaternion> ARROW_TYPE_TO_ROTATION
			= new Dictionary<MoveArrowType, Quaternion>
			{
				{MoveArrowType.Up, Quaternion.Euler(0, 90, 0)},
				{MoveArrowType.Right, Quaternion.Euler(0, 180, 0)},
				{MoveArrowType.Down, Quaternion.Euler(0, 270, 0)},
				{MoveArrowType.Left, Quaternion.Euler(0, 0, 0)}
			};

		/// <summary>矢印モデルの一覧</summary>
		public readonly Dictionary<MoveArrowType, MoveArrowModel> _models;

		public readonly List<MoveArrowModel> Models = new List<MoveArrowModel>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public MoveArrowManagerModel() {
			_models = EnumUtils.GetValues<MoveArrowType>().ToDictionary(
				type => type,
				type => new MoveArrowModel( type )
			);

			Models.Add(new MoveArrowModel(MoveArrowType.Up));
			Models.Add(new MoveArrowModel(MoveArrowType.Right));
			Models.Add(new MoveArrowModel(MoveArrowType.Down));
			Models.Add(new MoveArrowModel(MoveArrowType.Left));
			
			_disposables.AddFirst( () => {
				_models.ForEach( pair => pair.Value.Dispose() );
			} );
		}

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 矢印タイプから、モデルを取得
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public MoveArrowModel GetModel( MoveArrowType type )
			=> _models.GetOrDefault( type );
	}
}