using UnityEngine;
using UniRx;
using SubmarineMirage.Base;

namespace TatemonSugoroku.Scripts
{



	/// <summary>
	/// ■ タイルの描画クラス
	/// </summary>
	public class TileView : SMStandardMonoBehaviour
	{
		public TileModel _model { get; private set; }
		MeshRenderer _renderer { get; set; }



		public void Setup(TileModel model)
		{
			_model = model;
			_renderer = GetComponent<MeshRenderer>();

			gameObject.name = model._name;
			transform.position = model._realPosition;
			transform.localScale = model._realScale;
			_renderer.material.color = TileManagerView.AREA_TYPE_TO_COLOR[model._areaType.Value];

			/*
			_model._areaType.Subscribe(type =>
				_renderer.material.color = TileManagerView.AREA_TYPE_TO_COLOR[type]
			);
			*/
		}

		public void ChangeColor(Color color)
		{
			_renderer.material.color = color;
		}
	}
}