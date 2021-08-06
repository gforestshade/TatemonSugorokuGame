using System;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ モデルのインターフェース
	/// </summary>
	public interface IModel : IDisposable {
		void Initialize( AllModelManager manager );
	}
}