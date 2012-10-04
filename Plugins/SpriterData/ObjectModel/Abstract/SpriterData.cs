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
using BrashMonkey.Spriter.Data.IO;

namespace BrashMonkey.Spriter.Data.ObjectModel
{
	/// <summary>
	/// SCML Version information.
	/// </summary>
	public sealed class VersionInformation
	{
		/// <summary>
		/// The version of the SCML file.
		/// </summary>
		public string version { get; private set; }
		
		/// <summary>
		/// The SCML generator used.
		/// </summary>
		public string generator { get; private set; }
		
		/// <summary>
		/// The generator version used.
		/// </summary>
		public string generatorVersion { get; private set; }
		
		/// <summary>
		/// If pixel art mode is enabled, renderers should use point filtering on textures.
		/// </summary>
		public bool pixelArtMode { get; private set; }
		
		/// <summary>
		/// Valid values: "true", "false"
		/// </summary>
		public string pixelArtModeRaw { get; private set; }
		
		/// <summary>
		/// Initializes all version information.
		/// </summary>
		public void Initialize(string version, string generator, string generatorVersion, bool pixelArtMode, string pixelArtModeRaw)
		{
			this.version = version;
			this.generator = generator;
			this.generatorVersion = generatorVersion;
			this.pixelArtMode = pixelArtMode;
			this.pixelArtModeRaw = pixelArtModeRaw;
		}
	}
	
	public sealed class DocumentInformation
	{
		/// <summary>
		/// Custom author information.
		/// </summary>
		public string author { get; internal set; }
		
		/// <summary>
		/// Custom copyright information.
		/// </summary>
		public string copyright { get; internal set; }
		
		/// <summary>
		/// Custom license information.
		/// </summary>
		public string license { get; internal set; }
		
		/// <summary>
		/// Custom version information.
		/// </summary>
		public string version { get; internal set; }
		
		/// TODO: This should have a recommended format in the specification (UNIX timestamp?)
		/// <summary>
		/// Custom last modified timestamp.
		/// </summary>
		public string lastModified { get; internal set; }
		
		/// <summary>
		/// Custom notes.
		/// </summary>
		public string notes { get; internal set; }
	
		/// <summary>
		/// Initializes all document information.
		/// </summary>
		public void Initialize(string author, string copyright, string license, string version,
			string lastModified, string notes)
		{
			this.author = author;
			this.copyright = copyright;
			this.license = license;
			this.version = version;
			this.lastModified = lastModified;
			this.notes = notes;
		}
	}
	
	/// <summary>
	/// This class provides methods for importing and exporting Spriter data to various implementations.
	/// </summary>
	public abstract class SpriterData
	{
		/// <summary>
		/// SCML version information.
		/// </summary>
		public VersionInformation versionInfo { get; internal set; }
		
		/// <summary>
		/// Document meta data.
		/// </summary>
		public List<SpriterMetaData> metaData { get; internal set; }
		
		/// <summary>
		/// The list of files to import/export.
		/// </summary>
		/// <remarks>
		/// These files are referenced by animations.
		/// </remarks>
		public List<SpriterFile> files { get; internal set; }
		
		/// <summary>
		/// The list of atlases in the SCML file.
		/// </summary>
		public List<SpriterAtlas> atlases { get; internal set; }
		
		/// <summary>
		/// The Spriter entity to import/export.
		/// </summary>
		/// <remarks>
		/// There is only one entity per SCML file.
		/// </remarks>
		public SpriterEntity entity { get; internal set; }
		
		/// <summary>
		/// Character maps allow dynamic mapping of files, mainly for skinning purposes.
		/// </summary>
		public SpriterCharacterMap characterMap { get; internal set; }
		
		/// <summary>
		/// SCML document information.
		/// </summary>
		public DocumentInformation documentInfo { get; internal set; }
		
		public SpriterData()
		{
			versionInfo = new VersionInformation();
			metaData = new List<SpriterMetaData>();
			files = new List<SpriterFile>();
			atlases = new List<SpriterAtlas>();
			entity = new SpriterEntity();
			characterMap = new SpriterCharacterMap();
			documentInfo = new DocumentInformation();
		}
		
		/// <summary>
		/// Loads data from an SCML file.
		/// </summary>
		public void LoadData(string path)
		{
			SCMLParse.LoadSCML(this, path);
			ToImplementation();
		}
		/// <summary>
		/// Saves data to an SCML file.
		/// </summary>
		public void SaveData(string path)
		{
			SCMLParse.SaveSCML(this, path);
			FromImplementation();
		}
		
		/// <summary>
		/// Sends the data to the specific implementation.
		/// </summary>
		protected abstract void ToImplementation();
		
		/// <summary>
		/// Saves data from the specific implementation
		/// </summary>
		protected abstract void FromImplementation();
	}
}