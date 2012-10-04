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
	/// Represents a file.
	/// </summary>
	public sealed class SpriterFile
	{
		/// <summary>
		/// The type of file.
		/// </summary>
		public FileType type { get; internal set; }
		
		/// <summary>
		/// The raw type of file. Supported values are "image", "sound_effect", "atlas_image", or "entity" (.scml file). Any other value is accepted, but will be registered as "Unknown"
		/// </summary>
		public string typeRaw { get; internal set; }
		
		/// <summary>
		/// Integer unique to this file, within its folder
		/// </summary>
		public int ID { get; internal set; }
		
		/// <summary>
		/// Integer unique to this file's folder
		/// </summary>
		public int folderID { get; internal set; }
		
		/// <summary>
		/// Name unique to this file, within its folder
		/// </summary>
		public string name { get; internal set; }
		
		/// <summary>
		/// The default pivot, in UV space. (0, 0) = bottom left, (1, 1) = top right
		/// </summary>
		public Vector2 pivot { get; internal set; }
		
		/// <summary>
		/// Image width
		/// </summary>
		public int width { get; internal set; }
		
		/// <summary>
		/// Image height
		/// </summary>
		public int height { get; internal set; }
		
		/// <summary>
		/// Atlas X location
		/// </summary>
		public int atlasX { get; internal set; }
		
		/// <summary>
		/// Atlas Y location
		/// </summary>
		public int atlasY { get; internal set; }
		
		/// <summary>
		/// Atlas X offset
		/// </summary>
		public int offsetX { get; internal set; }
		
		/// <summary>
		/// Atlas Y offset
		/// </summary>
		public int offsetY { get; internal set; }
		
		/// <summary>
		/// Atlas original width
		/// </summary>
		public int originalWidth { get; internal set; }
		
		/// <summary>
		/// Atlas original height
		/// </summary>
		public int originalHeight { get; internal set; }
	}
}