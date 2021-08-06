//#define TestMoveArrow
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;

namespace TatemonSugoroku.Scripts
{



	/// <summary>
	/// ■ 移動矢印の描画クラス
	/// </summary>
	public class MoveArrowView : SMStandardMonoBehaviour
	{
		MoveArrowModel _model { get; set; }
		MeshRenderer[] _renderers { get; set; }

		[SerializeField] Vector3 _offset = new Vector3(0, 0.6f, 0);



		public void Setup(MoveArrowModel model)
		{
			_model = model;

			_renderers = GetComponentsInChildren<MeshRenderer>();

			gameObject.name = _model._name;
			transform.rotation = _model._rotation;
			gameObject.SetActive(false);

			/*
			_model._placeTile.Subscribe(tile =>
			{
				gameObject.SetActive(tile != null);
				if (tile == null)
				{
					return;
				}

				transform.position = tile._realPosition + _offset;
				transform.localScale = tile._realScale;
			});
			*/

#if TestMoveArrow
			_offset.y += 0.3f;
			transform.rotation *= Quaternion.Euler( 90, 0, 0 );
#endif
		}



		public void SetColor(Color color)
		{
			_renderers.ForEach(r => r.material.color = color);
		}
		
		// 勝手ながら追加させていただきます。(by Akio)
		public void HideArrow()
		{
			gameObject.SetActive(false);
		}

		public void ShowArrow(Vector3 actualPosition, Color color)
		{
			Debug.Log("反応確認用");
			gameObject.SetActive(true);
			transform.position = actualPosition + _offset;
			_renderers.ForEach(r => { r.material.color = color; });
		}
	}
}