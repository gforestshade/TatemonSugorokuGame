using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ サイコロの描画クラス
	/// </summary>
	public class DiceView : SMStandardMonoBehaviour {
		DiceState _state { get; set; }

		Rigidbody _rigidbody { get; set; }
		Transform[] _transforms { get; set; }
		Vector3 _firstPosition { get; set; }

		int _diceID { get; set; }
		public Vector3 _power { get; set; }
		public int _value { get; private set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		public void Setup( int diceID ) {
			_diceID = diceID;

			_rigidbody = GetComponent<Rigidbody>();
			_transforms = transform.GetChildren( true )
				.Select( go => go.transform )
				.ToArray();

			var first = -DiceModel.MAX_COUNT / 2 + 0.5f;
			transform.position += Vector3.left * ( first + diceID );
			_firstPosition = transform.position;

			_state = DiceState.Roll;
			ChangeState( DiceState.Hide ).Forget();


			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );
		}



		protected override void UpdateAfterInitialize() {
			switch ( _state ) {
				case DiceState.Rotate:
					transform.position = _firstPosition;
					break;
			}
		}



		public async UniTask ChangeState( DiceState state ) {
			if ( _state == state )	{ return; }
			_state = state;

			_value = -1;
			transform.position = _firstPosition;
			gameObject.SetActive( false );
			_rigidbody.useGravity = false;
			_canceler.Cancel();

			switch ( _state ) {
				case DiceState.Hide:
					return;

				case DiceState.Rotate:
					gameObject.SetActive( true );
					_rigidbody.AddTorque(
						new Vector3(
							Random.Range( -10, 10 ),
							Random.Range( -10, 10 ),
							Random.Range( -10, 10 )
						),
						ForceMode.Impulse
					);
					return;

				case DiceState.Roll:
					gameObject.SetActive( true );
					_rigidbody.useGravity = true;
					_rigidbody.AddForce(
						_power,
//						transform.forward * 10,
//						new Vector3( -1, 0.1f, 1 ) * 10,
						ForceMode.Impulse
					);

					await UTask.WaitWhile( _canceler, () => !_rigidbody.IsSleeping() );

					var maxY = 0f;
					var value = 0;
					_transforms
						.Where( t => t.position.y > maxY )
						.ForEach( t => {
							maxY = t.position.y;
							value = t.gameObject.name.ToInt();
						} );
					_value = value;

					return;
			}
		}
	}
}