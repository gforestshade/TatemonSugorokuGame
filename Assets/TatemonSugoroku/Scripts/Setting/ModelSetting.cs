using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Debug;

using Sample;
using TatemonSugoroku.Scripts;
using TatemonSugoroku.Scripts.Akio;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ モデルの設定クラス
	/// </summary>
	public class ModelSetting : SMStandardBase, ISMService {
		/// <summary>モデルの管理者</summary>
		AllModelManager _allModelManager { get; set; }



		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public ModelSetting() {
			_disposables.AddLast( () => {
				_allModelManager = null;
			} );
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public void Setup( AllModelManager allModelManager ) {
			_allModelManager = allModelManager;

			// ------------------------------------------------------------------------------------------------
			//  ★ ここに、Modelを登録してください。
			// ------------------------------------------------------------------------------------------------
			// こんな感じに
			Register( new SampleModel() );

			Register( new DayModel() );
			Register( new TileManagerModel() );
			Register( new MoveArrowManagerModel() );
			Register( new DiceModel() );
			Register( new PieceManagerModel() );

			Register(new MotionModel());
			Register(new FieldModel());
			Register(new ScoreModel());
			Register(new MainGameManagementModel());

			_allModelManager.Get<MainGameManagementModel>().SetUpMotionModel(_allModelManager.Get<MotionModel>());
			_allModelManager.Get<MainGameManagementModel>().SetUpFieldModel(_allModelManager.Get<FieldModel>());
			_allModelManager.Get<MainGameManagementModel>().SetUpScoreModel(_allModelManager.Get<ScoreModel>());
		}

		/// <summary>
		/// ● モデルを登録
		///		モデルのTypeを鍵とする。
		/// </summary>
		void Register<T>( T model ) where T : class, IModel
			=> _allModelManager.Register( model );

	}
}