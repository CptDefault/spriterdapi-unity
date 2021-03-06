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
	/// Represents an atlas.
	/// </summary>
	/// TODO: Comments
	public sealed class SpriterAtlas
	{
		public int ID { get; internal set; }
		public string dataPath { get; internal set; }
		public string imagePath { get; internal set; }
		
		public List<SpriterAtlasImage> images { get; internal set; }
		
		public SpriterAtlas()
		{
			images = new List<SpriterAtlasImage>();
		}
	}
	
	/// <summary>
	/// Represents an atlas image.
	/// </summary>
	public sealed class SpriterAtlasImage
	{
		public int ID { get; internal set; }
		public string fullPath { get; internal set; }
		public int folderID { get; internal set; }
		public string folderName { get; internal set; }
	}
}
