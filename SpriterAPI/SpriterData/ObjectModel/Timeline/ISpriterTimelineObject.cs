﻿// NGUI import/export plugin
// SpriterDataUnity.cs
// Spriter Data API - Unity
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//       Justin Whitfort <cptdefault@gmail.com>
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
// NGUI is (c) by Tasharen Entertainment. Spriter is (c) by BrashMonkey.
//

using System.Collections.Generic;
using UnityEngine;

namespace BrashMonkey.Spriter.Data.ObjectModel
{
	public interface ISpriterTimelineObject : ISpriterTimelineBone
	{
		SpriterAtlas targetAtlas { get;  }
		SpriterFile targetFile { get; }

		int atlas { get; }
		int folder { get; }
		int file { get; }
		string name { get; }
		//Vector2 position { get; }
		Vector2 pivot { get; }
		//float angle { get; }
		int pixelWidth { get; }
		int pixelHeight { get; }
		//Vector2 scale { get; }
		//Color color { get; }
		BlendMode blendMode { get; }
		string blendModeRaw { get; }
		object value { get; }
		object min { get; }
		object max { get; }
		int entityAnimation { get; }
		float entityT { get; }
		float volume { get; }
		float panning { get; }

		/// <summary>
		/// Timeline object meta data
		/// </summary>
		List<SpriterMetaData> metaData { get; }
	}
}
