// Spriter object model - ISpriterData
// ISpriterData.cs
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
	/// This interface provides methods for importing and exporting Spriter data to various implementations.
	/// </summary>
	public interface ISpriterData
	{
		/// <summary>
		/// The Spriter character to import/export.
		/// </summary>
		/// <remarks>
		/// There is only one character per SCML file.
		/// </remarks>
		ISpriterCharacter character
		{
			get;
			set;
		}
		
		/// <summary>
		/// The list of render frames to import/export.
		/// </summary>
		/// <remarks>
		/// The names of these frames are referenced by keyframes on each character animation.
		/// </remarks>
		List<ISpriterFrame> frames
		{
			get;
			set;
		}
		
		/// <summary>
		/// Loads data from SCML.
		/// </summary>
		void LoadSCML(string path);
		/// <summary>
		/// Saves data to SCML.
		/// </summary>
		void SaveSCML(string path);
		
		/// <summary>
		/// Exports the data from the specific implementation.
		/// </summary>
		void ExportData();
		
		/// <summary>
		/// Imports the data to the specific implementation.
		/// </summary>
		void ImportData();
	}
}