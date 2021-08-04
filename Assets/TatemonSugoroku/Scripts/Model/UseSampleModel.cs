using System.Collections;
using Cysharp.Threading.Tasks;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;



/// <summary>
/// ■ モデルの使用クラス
///		Modelは、こんな感じで使って下さい。
///			SMStandardMonoBehaviourを継承すると、楽です。
/// </summary>
public class UseSampleModel : SMStandardMonoBehaviour {
	/// <summary>偽のサンプルデータ</summary>
	byte[] _data	{ get; set; } = null;



	///------------------------------------------------------------------------------------------------
	/// ● 初期化
	///		フレームワーク初期化を待たないと、エラーになります。
	///		それを考慮し、4パターンの初期化方法があります。
	///			StartAfterInitialize()の使用が、一番簡単です。
	///------------------------------------------------------------------------------------------------

	/// <summary>
	/// ● フレームワーク初期化後に呼ばれる、初期化
	/// </summary>
	protected override void StartAfterInitialize() {
		// StartAfterInitializeは、フレームワーク初期化を待機してから、呼ばれます。
		// なので、初期化を意識せず、Start()と同じように、普通に使えます！
		
		var modelManager = SMServiceLocator.Resolve<AllModelManager>();	// サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();					// モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );								// モデルから、データを取得
	}


	/// <summary>
	/// ● コルーチンによる、初期化（Unityコールバック）
	/// </summary>
	IEnumerator Start() {
		// フレームワーク初期化を、コルーチンで待機する
		yield return _framework.WaitInitializeCoroutine();
		// フレームワーク初期化前に、アクセスするとエラー

		var modelManager = SMServiceLocator.Resolve<AllModelManager>();	// サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();					// モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );								// モデルから、データを取得
	}


	/// <summary>
	/// ● 初期化（Unityコールバック）
	/// </summary>
	// エラーになるので、Start1と命名してます（本来はStart）
	void Start1() {
		// フレームワークが初期化されてない場合、処理しない
		if ( !_isFrameworkInitialized )	{ return; }
		// フレームワーク初期化前に、アクセスするとエラー

		var modelManager = SMServiceLocator.Resolve<AllModelManager>();	// サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();					// モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );								// モデルから、データを取得
	}


	/// <summary>
	/// ● UniTaskによる、初期化
	/// </summary>
	// エラーになるので、Start2と命名してます（本来はStart）
	async UniTask Start2() {
		// フレームワーク初期化を、UniTaskで待機する
		await _framework.WaitInitialize();
		// フレームワーク初期化前に、アクセスするとエラー

		var modelManager = SMServiceLocator.Resolve<AllModelManager>(); // サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();                    // モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );                             // モデルから、データを取得
	}



	///------------------------------------------------------------------------------------------------
	/// ● 更新
	///		フレームワーク初期化を待たないと、エラーになります。
	///		それを考慮し、2パターンの更新方法があります。
	///			UpdateAfterInitialize()の使用が、一番簡単です。
	///------------------------------------------------------------------------------------------------

	/// <summary>
	/// ● フレームワーク初期化後に呼ばれる、更新
	/// </summary>
	protected override void UpdateAfterInitialize() {
		// UpdateAfterInitializeは、フレームワーク初期化を待機してから、呼ばれます。
		// なので、初期化を意識せず、Update()と同じように、普通に使えます！

		SMLog.Debug( _data );
	}


	/// <summary>
	/// ● 更新（Unityコールバック）
	/// </summary>
	protected override void Update() {
		base.Update();

		// フレームワークが初期化されてない場合、処理しない
		if ( !_isFrameworkInitialized ) { return; }
		// フレームワーク初期化前に、アクセスするとエラー

		SMLog.Debug( _data );
	}


	// 他にもフレームワーク初期化後に呼ばれる、FixedUpdate、LateUpdate、があります。
	// 適宜ご利用ください。
	protected override void FixedUpdateAfterInitialize() {}
	protected override void LateUpdateAfterInitialize() {}
}





/// <summary>
/// ■ モデルの使用クラス2
///		Modelは、こんな感じで使って下さい。
///			SMStandardMonoBehaviourを継承しない、パターンです。
/// </summary>
public class UseSampleModel2 {
	/// <summary>偽のサンプルデータ</summary>
	byte[] _data { get; set; } = null;



	/// <summary>
	/// ● コルーチンによる、初期化
	/// </summary>
	public IEnumerator Initialize() {
		// フレームワーク初期化を、コルーチンで待機する
		var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
		yield return framework.WaitInitializeCoroutine();
		// フレームワーク初期化前に、アクセスするとエラー

		var modelManager = SMServiceLocator.Resolve<AllModelManager>();	// サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();					// モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );								// モデルから、データを取得
	}

	/// <summary>
	/// ● UniTaskによる、初期化
	/// </summary>
	public void Initialize2() {
		UTask.Void( async () => {
			// フレームワーク初期化を、UniTaskで待機する
			var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
			await framework.WaitInitialize();
			// フレームワーク初期化前に、アクセスするとエラー

			var modelManager = SMServiceLocator.Resolve<AllModelManager>(); // サービスロケーターから、モデル管理クラスを取得
			var model = modelManager.Get<SampleModel>();                    // モデル管理クラスから、モデルを取得
			_data = model.GetData( "Player1" );                             // モデルから、データを取得
		} );
	}

	/// <summary>
	/// ● UniTaskによる、初期化2
	///		これが一番やりやすいけど、async/awaitと、UniTaskを知らないと使えない・・・。
	/// </summary>
	public async UniTask Initialize3() {
		// フレームワーク初期化を、UniTaskで待機する
		var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
		await framework.WaitInitialize();
		// フレームワーク初期化前に、アクセスするとエラー

		var modelManager = SMServiceLocator.Resolve<AllModelManager>(); // サービスロケーターから、モデル管理クラスを取得
		var model = modelManager.Get<SampleModel>();                    // モデル管理クラスから、モデルを取得
		_data = model.GetData( "Player1" );                             // モデルから、データを取得
	}



	/// <summary>
	/// ● 更新
	/// </summary>
	public void Update() {
		// フレームワークが初期化されてない場合、処理しない
		var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
		if ( !framework._isInitialized )	{ return; }
		// フレームワーク初期化前に、アクセスするとエラー

		// フレームワーク初期化済でも、データがまだ設定されてない事がある。（1フレーム遅れる）
		// フレームワーク初期化で判断よりも、データ未設定で判断した方がいいかも？
		if ( _data == null )	{ return; }
		// データ設定前に、アクセスするとエラー

		SMLog.Debug( _data );
	}
}