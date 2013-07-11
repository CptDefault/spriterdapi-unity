// Spriter Data API - Unity
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//       Justin Whitfort <cptdefault@gmail.com>
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
	/// The mainline hierarchy. Contains bones and bone refs.
	/// </summary>
	/// TODO: Comments
	public sealed class SpriterHierarchy
	{
		public List<SpriterMainlineBoneBase> bones { get; internal set; }
		
		public SpriterHierarchy()
		{
			bones = new List<SpriterMainlineBoneBase>();
		}
	}
	
	public abstract class SpriterMainlineBoneBase
	{
		public int ID { get; internal set; }
		public int parent { get; internal set; }

		protected SpriterMainlineBoneBase()
		{
			parent = -1;
		}
	}

	public sealed class SpriterMainlineBone : SpriterMainlineBoneBase, ISpriterTimelineBone
	{
		
		public Vector2 position { get; internal set; }
		public float angle { get; internal set; }
		public Vector2 scale { get; internal set; }
		public Color color { get; internal set; }
		
		/// <summary>
		/// Mainline bone meta data
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		public SpriterMainlineBone()
		{
			metaData = new List<SpriterMetaData>();
		}
	}
	
	public sealed class SpriterMainlineBoneRef : SpriterMainlineBoneBase
	{
		
		public SpriterTimelineBone target { get; internal set; }
		
		public int timeline { get; internal set; }
		public int key { get; internal set; }
	}
}
