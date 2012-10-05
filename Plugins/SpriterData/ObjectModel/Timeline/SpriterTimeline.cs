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
	/// Represents a persistent object timeline associated with an entity
	/// </summary>
	public sealed class SpriterTimeline
	{
		/// <summary>
		/// The timeline ID, unique to this timeline within this animation
		/// </summary>
		public int ID { get; internal set; }
		
		/// <summary>
		/// The name of the entity.
		/// </summary>
		public string name { get; internal set; }
		
		/// <summary>
		/// The object type.
		/// </summary>
		public ObjectType objectType { get; internal set; }
		
		/// <summary>
		/// Valid values: "point", "box", "sprite", "sound", "entity", "variable"
		/// </summary>
		public string objectTypeRaw { get; internal set; }
		
		/// <summary>
		/// The type of variable (for tweened variables)
		/// </summary>
		public VariableType variableType { get; internal set; }
		
		/// <summary>
		/// Valid values: "point", "box", "sprite", "sound", "entity", "variable"
		/// </summary>
		public string variableTypeRaw { get; internal set; }
		
		/// <summary>
		/// The use case for the object
		/// </summary>
		public UsageType usage { get; internal set; }
		
		/// <summary>
		/// Valid values: "display", "collision", "both", "neither"
		/// </summary>
		public string usageRaw { get; internal set; }
		
		/// <summary>
		/// Timeline meta data
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		/// <summary>
		/// The list of timeline keys.
		/// </summary>
		public List<SpriterTimelineKey> keys { get; internal set; } 
		
		public SpriterTimeline()
		{
			metaData = new List<SpriterMetaData>();
			keys = new List<SpriterTimelineKey>();
		}
	}
}
