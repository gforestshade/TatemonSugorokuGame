using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ タイルのモデルクラス
	///		純粋なタイル機能のみ実装しています。
	///		コマ、たてもん等へのアクセスは、別のクラスを参照下さい。
	/// </summary>
	///========================================================================================================
	public class TileModel : SMStandardBase {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>ゲームオブジェクトに付ける名前</summary>
		public string _name				{ get; private set; }
		/// <summary>タイル番号</summary>
		public int _tileID				{ get; private set; }
		/// <summary>タイル位置</summary>
		public Vector2Int _tilePosition	{ get; private set; }
		/// <summary>実際の位置（メートル）</summary>
		public Vector3 _realPosition	{ get; private set; }
		/// <summary>実際の大きさ（メートル）</summary>
		public Vector3 _realScale		{ get; private set; }

		/// <summary>プレイヤー占領の領域タイプ</summary>
		public readonly ReactiveProperty<TileAreaType> _areaType = new ReactiveProperty<TileAreaType>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public TileModel( int tileID ) {
			_tileID = tileID;
			_tilePosition = TileManagerModel.ToTilePosition( tileID );
			_realPosition = TileManagerModel.ToRealPosition( _tilePosition );
			_realScale = TileManagerModel.REAL_SCALE;

			_name = $"Tile {_tileID} ( {_tilePosition.x}, {_tilePosition.y} )";

			_disposables.AddFirst( () => {
				_areaType.Dispose();
			} );
		}

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 領域を設定
		///		陣地が塗り替わったら、呼んで下さい。
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public void SetAreaType( TileAreaType type ) {
			_areaType.Value = type;
		}
	}
}