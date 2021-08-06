using System.Collections.Generic;
using KoganeUnityLib;
using TatemonSugoroku.Scripts;
namespace Sample {



	/// <summary>
	/// ■ モデルの使用例クラス
	///		Modelは、こんな感じで作成して下さい。
	///		作ったら、ModelSetting.csに、登録して下さい。
	/// </summary>
	public class SampleModel : IModel {
		/// <summary>偽データの一覧</summary>
		readonly Dictionary<string, byte[]> _dummyDatas = new Dictionary<string, byte[]>();



		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SampleModel() {
			_dummyDatas["Player1"] = new byte[100];
			_dummyDatas["Player2"] = new byte[100];
		}

		/// <summary>
		/// ● 初期化
		///		モデル管理クラスから、呼ばれます。
		///		他のモデル取得に、ご活用下さい。
		/// </summary>
		public void Initialize( AllModelManager manager ) {
		}

		/// <summary>
		/// ● 解放
		///		AllModelManager内部で破棄するので、必ず実装して下さい。
		/// </summary>
		public void Dispose() {
			_dummyDatas.ForEach( pair => pair.Value.Clear() );
			_dummyDatas.Clear();
		}



		/// <summary>
		/// ● データ取得
		/// </summary>
		public byte[] GetData( string key )
			=> _dummyDatas.GetOrDefault( key );
	}
}