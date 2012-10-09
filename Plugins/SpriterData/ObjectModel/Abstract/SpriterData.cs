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
		public string version { get; internal set; }
		
		/// <summary>
		/// The SCML generator used.
		/// </summary>
		public string generator { get; internal set; }
		
		/// <summary>
		/// The generator version used.
		/// </summary>
		public string generatorVersion { get; internal set; }
		
		/// <summary>
		/// If pixel art mode is enabled, renderers should use point filtering on textures.
		/// </summary>
		public bool pixelArtMode { get; internal set; }
		
		/// <summary>
		/// Valid values: "true", "false"
		/// </summary>
		public string pixelArtModeRaw { get; internal set; }
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
		
		/// <summary>
		/// Custom last modified timestamp. UNIX timestamp (64-bit).
		/// </summary>
		public string lastModified { get; internal set; }
		
		/// <summary>
		/// Custom notes.
		/// </summary>
		public string notes { get; internal set; }
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
		
#if UNITY_EDITOR || SCML_RUNTIME
		SCMLParser m_Parser;
#endif	
		public SpriterData()
		{
			versionInfo = new VersionInformation();
			metaData = new List<SpriterMetaData>();
			files = new List<SpriterFile>();
			atlases = new List<SpriterAtlas>();
			entity = new SpriterEntity();
			characterMap = new SpriterCharacterMap();
			documentInfo = new DocumentInformation();

#if UNITY_EDITOR || SCML_RUNTIME			
			m_Parser = new SCMLParser(this);
#endif		
			Reset();
		}
		
		/// <summary>
		/// Loads data from an SCML file.
		/// </summary>
		public void LoadData(string path)
		{
#if UNITY_EDITOR || SCML_RUNTIME			
			m_Parser.LoadSCML(path);
#endif
			ToImplementation();
		}
		/// <summary>
		/// Saves data to an SCML file.
		/// </summary>
		public void SaveData(string path)
		{
			FromImplementation();
#if UNITY_EDITOR || SCML_RUNTIME
			m_Parser.SaveSCML(path);
#endif
		}
		
		public void Reset()
		{
			this.atlases.Clear();
			
			this.characterMap.ID = 0;
			this.characterMap.maps.Clear();
			this.characterMap.name = "";
			
			this.documentInfo.author = "author not specified";
			this.documentInfo.copyright = "copyright info not specified";
			this.documentInfo.lastModified = "date and time not included";
			this.documentInfo.license = "no license specified";
			this.documentInfo.notes = "no additional notes";
			this.documentInfo.version = "version not specified";
			
			this.entity.animations.Clear();
			this.entity.ID = 0;
			this.entity.metaData.Clear();
			this.entity.name = "";
			
			this.files.Clear();
			
			this.metaData.Clear();
			
			this.versionInfo.generator = SpriterDataVersionInfo.generator;
			this.versionInfo.generatorVersion = SpriterDataVersionInfo.generatorVersion;
			this.versionInfo.pixelArtMode = false;
			this.versionInfo.pixelArtModeRaw = "false";
			this.versionInfo.version = "";
		}
		
		/// <summary>
		/// Finds a file by its folder and file ID.
		/// </summary>
		/// <remarks>
		/// Returns null if no file is found or the IDs are invalid.
		/// </remarks>
		public SpriterFile FindFile(int folderID, int fileID)
		{
			if (folderID < 0 || fileID < 0)
				return null;
			
			foreach(SpriterFile file in this.files)
			{
				if (file.ID == fileID && file.folderID == folderID)
					return file;
			}
			return null;
		}
		
		/// <summary>
		/// Finds an atlas by its ID.
		/// </summary>
		/// <remarks>
		/// Returns null if no atlas is found or the IDs are invalid.
		/// </remarks>
		public SpriterAtlas FindAtlas(int ID)
		{
			if (ID < 0)
				return null;
			
			foreach(SpriterAtlas atlas in this.atlases)
			{
				if (atlas.ID == ID)
					return atlas;
			}
			return null;
		}
		
		/// <summary>
		/// Finds a timeline object for an animation by its ID and time.
		/// </summary>
		///	<remarks>
		/// Returns null if no object is found or the IDs are invalid.
		/// </remarks>
		/// TODO: Timelines currently only track one object, can probably simplify object model and remove the list
		public SpriterTimelineObject FindTimelineObject(SpriterAnimation animation, int timelineID, int keyID, int time)
		{
			if (animation == null || timelineID < 0 || keyID < 0)
				return null;
			
			foreach(SpriterTimeline timeline in animation.timelines)
			{
				if (timeline.ID != timelineID)
					continue;
				
				foreach(SpriterTimelineKey key in timeline.keys)
				{
					if (key.ID != keyID)
						continue;
					
					foreach(SpriterTimelineObjectBase obj in key.objects)
					{
						if (obj is SpriterTimelineObject)
							return (SpriterTimelineObject)obj;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds a timeline bone for an animation by its ID and time.
		/// </summary>
		///	<remarks>
		/// Returns null if no bone is found or the IDs are invalid.
		/// </remarks>
		/// TODO: Timelines currently only track one object, can probably simplify object model and remove the list
		public SpriterTimelineBone FindTimelineBone(SpriterAnimation animation, int timelineID, int keyID, int time)
		{
			if (animation == null || timelineID < 0 || keyID < 0)
				return null;
			
			foreach(SpriterTimeline timeline in animation.timelines)
			{
				if (timeline.ID != timelineID)
					continue;
				
				foreach(SpriterTimelineKey key in timeline.keys)
				{
					if (key.ID != keyID)
						continue;
					
					foreach(SpriterTimelineObjectBase obj in key.objects)
					{
						if (obj is SpriterTimelineBone)
							return (SpriterTimelineBone)obj;
					}
				}
			}
			return null;
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