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
	public class SirenDebugView2 : SMStandardMonoBehaviour {
		protected override void UpdateAfterInitialize() {
			var audioManager = SMServiceLocator.Resolve<SMAudioManager>();

			// BGM（音楽）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad7 ) ) {
				audioManager.Play( SMBGM.TestBattle ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad8 ) ) {
				audioManager.Play( SMBGM.TestTitle ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad9 ) ) {
				audioManager.Stop<SMBGM>().Forget();
			}

			// BGS（環境音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad4 ) ) {
				audioManager.Play( SMBGS.TestWater ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad5 ) ) {
				audioManager.Play( SMBGS.TestWind ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad6 ) ) {
				audioManager.Stop<SMBGS>().Forget();
			}

			// ジングル（ファンファーレ音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Keypad1 ) ) {
				audioManager.Play( SMJingle.TestGameClear ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad2 ) ) {
				audioManager.Play( SMJingle.TestGameOver ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.Keypad3 ) ) {
				audioManager.Stop<SMJingle>().Forget();
			}

			// SE（効果音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.Z ) ) {
				audioManager.Play( SMSE.TestDecision ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.X ) ) {
				audioManager.Play( SMSE.TestGun ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.C ) ) {
				audioManager.Stop<SMSE>().Forget();
			}

			// ループSE（効果音）の再生、停止
			if ( Input.GetKeyDown( KeyCode.A ) ) {
				audioManager.Play( SMLoopSE.TestTalk1 ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.S ) ) {
				audioManager.Play( SMLoopSE.TestTalk2 ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.D ) ) {
				audioManager.Stop<SMLoopSE>().Forget();
			}

			// 声音の再生、停止
			if ( Input.GetKeyDown( KeyCode.Q ) ) {
				audioManager.Play( SMVoice.TestRidicule ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.W ) ) {
				audioManager.Play( SMVoice.TestScream ).Forget();
			}
			if ( Input.GetKeyDown( KeyCode.E ) ) {
				audioManager.Stop<SMVoice>().Forget();
			}

			// 全音を停止
			if ( Input.GetKeyDown( KeyCode.Space ) ) {
				audioManager.StopAll().Forget();
			}
		}
	}
}