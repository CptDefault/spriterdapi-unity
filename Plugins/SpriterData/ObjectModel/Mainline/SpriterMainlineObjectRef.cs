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

namespace BrashMonkey.Spriter.Data.ObjectModel
{
	/// <summary>
	/// Represents an object reference on the mainline.
	/// </summary>
	public class SpriterMainlineObjectRef : SpriterMainlineObjectBase
	{
		/// <summary>
		/// The object ID, unique to this object reference within its key
		/// </summary>
		public int ID { get; internal set; }
		
		/// <summary>
		/// The timeline object referenced
		/// </summary>
		public SpriterTimelineObject target { get; internal set; }
		
		/// <summary>
		/// Corresponds to the ID of the timeline referenced
		/// </summary>
		public int timeline { get; internal set; }
		/// <summary>
		/// Corresponds to the ID of the key within the timeline referenced
		/// </summary>
		public int key { get; internal set; }
		/// <summary>
		/// Order this object should be drawn
		/// </summary>
		public int zIndex { get; internal set; }
	}
}
