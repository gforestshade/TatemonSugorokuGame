using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
using TatemonSugoroku.Scripts;
namespace TatemonSugoroku.Siren {



	/// <summary>
	/// ■ Siren用、デバッグの描画クラス2
	///		音のテスト用。
	/// </summary>
	public class SirenDebugView2 : MonoBehaviour {
		SMAudioManager _audioManager { get; set; }



		async void Start() {
			_audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
		}



		void Update() {
			if ( _audioManager == null )	{ return; }

			// BGM（音楽）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad7 ) ) {
				_audioManager.Play( SMBGM.TestBattle ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad8 ) ) {
				_audioManager.Play( SMBGM.TestTitle ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad9 ) ) {
				_audioManager.Stop<SMBGM>().Forget();
			}

			// BGS（環境音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad4 ) ) {
				_audioManager.Play( SMBGS.TestWater ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad5 ) ) {
				_audioManager.Play( SMBGS.TestWind ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad6 ) ) {
				_audioManager.Stop<SMBGS>().Forget();
			}

			// ジングル（ファンファーレ音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad1 ) ) {
				_audioManager.Play( SMJingle.TestGameClear ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad2 ) ) {
				_audioManager.Play( SMJingle.TestGameOver ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad3 ) ) {
				_audioManager.Stop<SMJingle>().Forget();
			}

			// SE（効果音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Z ) ) {
				_audioManager.Play( SMSE.Decide ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.X ) ) {
				_audioManager.Play( SMSE.Dice ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.C ) ) {
				_audioManager.Play( SMSE.Walk ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.V ) ) {
				_audioManager.Play( SMSE.Tatemon ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.B ) ) {
				_audioManager.Stop<SMSE>().Forget();
			}

			// ループSE（効果音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				_audioManager.Play( SMLoopSE.TestTalk1 ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.S ) ) {
				_audioManager.Play( SMLoopSE.TestTalk2 ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.D ) ) {
				_audioManager.Stop<SMLoopSE>().Forget();
			}

			// 声音の再生、停止
			if ( Input.GetKeyDown( KeyCode.Q ) ) {
				_audioManager.Play( SMVoice.TestRidicule ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.W ) ) {
				_audioManager.Play( SMVoice.TestScream ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.E ) ) {
				_audioManager.Stop<SMVoice>().Forget();
			}

			// 全音を停止
			if ( Input.GetKeyDown( KeyCode.Space ) ) {
				_audioManager.StopAll().Forget();
			}
		}
	}
}