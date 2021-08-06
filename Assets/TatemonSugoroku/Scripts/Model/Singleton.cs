using System;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
///========================================================================================================
/// <summary>
/// ■ シングルトンのクラス
/// </summary>
///========================================================================================================
public class Singleton<T> : SMStandardBase where T : class, IDisposable, new() {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
	/// <summary>シングルトン</summary>
	static T s_instanceObject;

	/// <summary>作成済か？</summary>
	public static bool s_isCreated => s_instanceObject != null;

	///----------------------------------------------------------------------------------------------------
	/// ● アクセサ
	///----------------------------------------------------------------------------------------------------
	/// <summary>シングルトン取得</summary>
	public static T s_instance {
		get {
			if ( !s_isCreated )	{ Create(); }
			return s_instanceObject;
		}
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 作成
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● インスタンス作成
	/// </summary>
	protected static void Create() {
		if ( s_isCreated )	{ return; }

		s_instanceObject = new T();
//		SMLog.Debug( $"作成 : { s_instanceObject.GetAboutName() }", SMLogTag.Singleton );
	}

	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	protected Singleton() {
/*
		_disposables.AddFirst( () => {
			s_instanceObject = null;
		} );
*/
	}
}