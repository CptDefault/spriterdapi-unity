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
	/// Represents an object on the mainline.
	/// </summary>
	public sealed class SpriterMainlineObject : SpriterMainlineObjectBase
	{
		public int ID { get; internal set; }
		public int parent { get; internal set; }
		public ObjectType objectType { get; internal set; }
		public string objectTypeRaw { get; internal set; }
		
		public SpriterAtlas targetAtlas { get; internal set; }
		public SpriterFile targetFile { get; internal set; }
		
		public int atlas { get; internal set; }
		public int folder { get; internal set; }
		public int file { get; internal set; }
		public UsageType usage { get; internal set; }
		public string usageRaw { get; internal set; }
		public BlendMode blendMode { get; internal set; }
		public string blendModeRaw { get; internal set; }
		public string name { get; internal set; }
		public Vector2 position { get; internal set; }
		public Vector2 pivot { get; internal set; }
		public float angle { get; internal set; }
		public int pixelWidth { get; internal set; }
		public int pixelHeight { get; internal set; }
		public Vector2 scale { get; internal set; }
		public Color color { get; internal set; }
		public VariableType variableType { get; internal set; }
		public string variableTypeRaw { get; internal set; }
		public object value { get; internal set; }
		public object min { get; internal set; }
		public object max { get; internal set; }
		public int entityAnimation { get; internal set; }
		public float entityT { get; internal set; }
		public int zIndex { get; internal set; }
		public float volume { get; internal set; }
		public float panning { get; internal set; }
		
		/// <summary>
		/// Mainline object meta data
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		public SpriterMainlineObject()
		{
			metaData = new List<SpriterMetaData>();
		}
	}
}
