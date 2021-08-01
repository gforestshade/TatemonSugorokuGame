//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug.ToString.Data {
	using System;
	using Debug;



	public class SMToLineStringData : BaseSMToStringData {
		[SMShowLine] public Func<string> _valueEvent	{ get; set; }


		public SMToLineStringData( Func<string> valueEvent )
			=> _valueEvent = valueEvent;
	}
}