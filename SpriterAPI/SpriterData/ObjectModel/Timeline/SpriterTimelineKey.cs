// Spriter Data API - Unity
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//
// 
// Copyright (c) 2012 Thinksquirrel Software, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Spriter is (c) by BrashMonkey.
//
using System.Collections.Generic;
using UnityEngine;

namespace BrashMonkey.Spriter.Data.ObjectModel
{
	/// <summary>
	/// Represents a timeline key.
	/// </summary>
	/// TODO: Comments
	public sealed class SpriterTimelineKey
	{
		/// <summary>
		/// Unique to this key within this timeline
		/// </summary>
		public int ID { get; internal set; }
		
		// TODO: This would be much better as a double precision float value in seconds, rather than integer milliseconds
		/// <summary>
		/// The time of the keyframe, in miliseconds
		/// </summary>
		public int time { get; internal set; }
		
		public CurveType curveType { get; internal set; }
		public string curveTypeRaw { get; internal set; }
		public Vector2 curveTangents { get; internal set; }
		
		/// <summary>
		/// Valid values: - 1, -1
		/// </summary>
		public int spin { get; internal set; }
		
		/// <summary>
		/// Timeline key meta data
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		/// <summary>
		/// The list of keyframe objects.
		/// </summary>
		public List<SpriterTimelineObjectBase> objects { get; internal set; }
		
		public SpriterTimelineKey()
		{
			metaData = new List<SpriterMetaData>();
			objects = new List<SpriterTimelineObjectBase>();
		}
	}
}
