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
	/// Represents an animation of a Spriter character.
	/// </summary>
	public sealed class SpriterAnimation
	{
		/// <summary>
		/// The ID of the animation, unique to each animation within an entity
		/// </summary>
		public int ID { get; internal set; }
		
		/// <summary>
		/// The name of the animation, unique to each animation within an entity
		/// </summary>
		public string name { get; internal set; }
		
		// TODO: This would be much better as a double precision float value in seconds, rather than integer milliseconds
		/// <summary>
		/// The total length of the animation, in milliseconds
		/// </summary>
		public int length { get; internal set; }
		
		/// <summary>
		/// Animation playback type.
		/// </summary>
		public PlaybackType playbackType { get; internal set; }
		
		/// <summary>
		/// Raw animation playback type. Can be "true" (looping), "false" (not looping), "ping_pong" (loop back and forth). All other values are considered unknown
		/// </summary>
		public string playbackTypeRaw { get; internal set; }
		
		/// <summary>
		/// The ID of the key to loop back to.
		/// </summary>
		public int loopTo { get; internal set; }
		
		/// <summary>
		/// Animation meta data.
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		/// <summary>
		/// The main timeline for the animation.
		/// </summary>
		public SpriterMainline mainline { get; internal set; }
		
		/// <summary>
		/// Additional timelines for persistent objects.
		/// </summary>
		public List<SpriterTimeline> timelines { get; internal set; }
		
		public SpriterAnimation()
		{
			metaData = new List<SpriterMetaData>();
			mainline = new SpriterMainline();
			timelines = new List<SpriterTimeline>();
		}
	}
}