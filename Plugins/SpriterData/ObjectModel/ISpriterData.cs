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
	/// SCML Version information.
	/// </summary>
	public class VersionInfo
	{
		/// <summary>
		/// The version of the SCML file.
		/// </summary>
		public string version
		{
			get;
			private set;
		}
		
		/// <summary>
		/// The SCML generator used.
		/// </summary>
		public string generator
		{
			get;
			private set;
		}
		
		/// <summary>
		/// The generator version used.
		/// </summary>
		public string generatorVersion
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="BrashMonkey.Spriter.Data.ObjectModel.VersionInfo"/> class.
		/// </summary>
		/// <param name='version'>
		/// Version string.
		/// </param>
		/// <param name='generator'>
		/// Generator string.
		/// </param>
		/// <param name='generatorVersion'>
		/// Generator version string.
		/// </param>
		public VersionInfo(string version, string generator, string generatorVersion)
		{
			Initialize(version, generator, generatorVersion);
		}
		
		/// <summary>
		/// Initializes all version information.
		/// </summary>
		public void Initialize(string version, string generator, string generatorVersion)
		{
			this.version = version;
			this.generator = generator;
			this.generatorVersion = generatorVersion;
		}
	}
	
	/// <summary>
	/// This interface provides methods for importing and exporting Spriter data to various implementations.
	/// </summary>
	public interface ISpriterData
	{
		/// <summary>
		/// SCML version information.
		/// </summary>
		VersionInfo versionInfo
		{
			get;
		}
		
		/// <summary>
		/// The list of files to import/export.
		/// </summary>
		/// <remarks>
		/// These files are referenced by animations.
		/// </remarks>
		List<SpriterFile> files
		{
			get;
		}
		
		/// <summary>
		/// The Spriter entity to import/export.
		/// </summary>
		/// <remarks>
		/// There is only one entity per SCML file.
		/// </remarks>
		SpriterEntity entity
		{
			get;
		}
		
		/// <summary>
		/// Loads data from an SCML file.
		/// </summary>
		void LoadData(string path);
		/// <summary>
		/// Saves data to an SCML file.
		/// </summary>
		void SaveData(string path);
		
		/// <summary>
		/// Sends the data to the specific implementation.
		/// </summary>
		void ToImplementation();
		
		/// <summary>
		/// Saves data from the specific implementation
		/// </summary>
		void FromImplementation();
	}
}