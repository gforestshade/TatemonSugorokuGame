using System;
using System.Collections.Generic;
using KoganeUnityLib;
using SubmarineMirage.Service;
using SubmarineMirage.Task;



/// <summary>
/// ■ 全モデルの管理クラス
/// </summary>
public class AllModelManager : SMTask, ISMService {
	/// <summary>タスク実行タイプ（生成のみ）</summary>
	public override SMTaskRunType _type => SMTaskRunType.Dont;
	/// <summary>全モデルの辞書</summary>
	readonly Dictionary<Type, IModel> _models = new Dictionary<Type, IModel>();



	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public AllModelManager() {
		_disposables.AddLast( () => {
			_models.ForEach( pair => pair.Value?.Dispose() );
			_models.Clear();
		} );
	}

	/// <summary>
	/// ● 作成
	/// </summary>
	public override void Create() {
		var setting = SMServiceLocator.Resolve<ModelSetting>();
		setting.Setup( this );
		SMServiceLocator.Unregister<ModelSetting>();
	}



	/// <summary>
	/// ● モデルを登録
	///		モデルのTypeを鍵とする。
	/// </summary>
	public T Register<T>( T model ) where T : class, IModel {
		CheckDisposeError( nameof( Register ) );

		var key = typeof( T );

		var last = _models.GetOrDefault( key );
		if ( last != null ) {
			throw new InvalidOperationException( string.Join( "\n",
				$"すでに同型のModelが登録済 : ",
				$"{nameof( key )} : {key}",
				$"{nameof( last )} : {last}",
				$"{nameof( model )} : {model}",
				$"{nameof( AllModelManager )}.{nameof( Register )}",
				$"{this}"
			) );
		}

		_models[key] = model;
		return model;
	}

	/// <summary>
	/// ● モデル登録を解除
	///		解除時に、Disposeも行う。
	/// </summary>
	public void Unregister<T>() where T : class, IModel {
		CheckDisposeError( nameof( Unregister ) );

		var key = typeof( T );

		var value = _models.GetOrDefault( key );
		value?.Dispose();

		_models.Remove( key );
	}

	/// <summary>
	/// ● モデルを取得
	/// </summary>
	public T Get<T>() where T : class, IModel {
		CheckDisposeError( nameof( Get ) );

		var key = typeof( T );
		return _models.GetOrDefault( key ) as T;
	}
}