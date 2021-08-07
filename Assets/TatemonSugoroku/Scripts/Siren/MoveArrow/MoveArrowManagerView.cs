using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印管理の描画クラス
	/// </summary>
	public class MoveArrowManagerView : SMStandardMonoBehaviour {
		/// <summary>矢印タイプから、角度に変換</summary>
		public static readonly Dictionary<MoveArrowType, Quaternion> ARROW_TYPE_TO_ROTATION
			= new Dictionary<MoveArrowType, Quaternion>
			{
				{ MoveArrowType.Up,     Quaternion.Euler( 0, 90, 0 ) },
				{ MoveArrowType.Right,  Quaternion.Euler( 0, 180, 0 ) },
				{ MoveArrowType.Down,   Quaternion.Euler( 0, 270, 0 ) },
				{ MoveArrowType.Left,   Quaternion.Euler( 0, 0, 0 ) }
			};

		readonly Dictionary<MoveArrowType, MoveArrowView> _views
			= new Dictionary<MoveArrowType, MoveArrowView>();

		[SerializeField] GameObject _prefab;
		[SerializeField] public Color _disableColor = Color.gray;
		[SerializeField] Color _enableColor = Color.yellow;

		public Color _color { get; private set; }
		Tween _colorTween { get; set; }



		void Start() {
			EnumUtils.GetValues<MoveArrowType>().ForEach( type => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<MoveArrowView>();
				v.Setup( type, _disableColor );
				_views[type] = v;
			} );

			_color = _disableColor;
			_colorTween = DOTween.To(
				() => _color,
				c => {
					_color = c;
					_views.ForEach( pair => pair.Value.UpdateColor( c ) );
				},
				_enableColor,
				1
			)
			.SetEase( Ease.InOutQuart )
			.SetLoops( -1, LoopType.Yoyo )
			.Play();

			_disposables.AddLast( () => {
				_colorTween?.Kill();
			} );
		}



		public void Place( params MoveArrowData[] arrowDatas ) {
			Hide();
			arrowDatas.ForEach( d => {
				var v = GetView( d._type );
				v.Place( d );
			} );
		}

		public void Hide()
			=> _views.ForEach(pair => pair.Value.Hide() );



		public MoveArrowView GetView( MoveArrowType type )
			=> _views[type];
	}
}