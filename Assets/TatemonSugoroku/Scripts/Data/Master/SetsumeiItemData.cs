using System.Collections.Generic;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Data;
namespace Sample {
	///========================================================================================================
	/// <summary>
	/// ■ サンプルアイテムの情報クラス
	/// </summary>
	///========================================================================================================
	public class SetsumeiItemData : SMCSVData<int> {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override int _registerKey => Id;

		/// <summary>番号</summary>
		[SMShow] public int Id			{ get; private set; }
		/// <summary>説明文</summary>
		[SMShow] public string Explanation	{ get; private set; }

		///----------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( string fileName, int index, List<string> texts ) {
			Id			= index;
			Explanation = texts[0];
		}
	}
}