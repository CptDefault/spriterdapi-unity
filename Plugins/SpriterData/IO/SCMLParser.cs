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
#if UNITY_EDITOR || SCML_RUNTIME
using System;
using System.Collections.Generic;
using System.Xml;
using BrashMonkey.Spriter.Data.ObjectModel;
using UnityEngine;

namespace BrashMonkey.Spriter.Data.IO
{
	internal class SCMLParser
	{	
		public XmlDocument scml { get; private set; }
		
		SpriterData m_Data;
		
		public SCMLParser(SpriterData data)
		{
			m_Data = data;
			scml = new XmlDocument();
		}
		
		#region Load
		/// <summary>
		/// Loads spriter information from SCML.
		/// </summary>
		public void LoadSCML(string path)
		{	
			// Load document
			scml.Load(path);
			
			// Reset all data
			m_Data.Reset();
			
			// Convert from scml to 
			foreach(XmlElement element in scml.DocumentElement)
			{
				// Spriter data
				if (element.Name.Equals("spriter_data"))
				{
					// Version info
					ReadVersionInfo(element);
					
					foreach(XmlElement child in element)
					{
						// Document meta data
						if (child.Name.Equals("meta_data"))
							ReadMetaData(child, m_Data.metaData);
						
						// Files
						else if (child.Name.Equals("folder"))
							ReadFolder(child);
						
						// Atlases
						else if (child.Name.Equals("atlas"))
							ReadAtlases(child);
						
						// Entity
						else if (child.Name.Equals("entity"))
							ReadEntity(child);
						
						// Character Map
						else if (child.Name.Equals("character_map"))
							ReadCharacterMap(child);
						
						// Document Info
						else if (child.Name.Equals("document_info"))
							ReadDocumentInfo(child);
					}
				}
			}
		}
		
		void ReadVersionInfo(XmlElement element)
		{
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// scml_version
				if (attribute.Name.Equals("scml_version"))
					m_Data.versionInfo.version = attribute.Value;
				
				// generator
				else if (attribute.Name.Equals("generator"))
					m_Data.versionInfo.generator = attribute.Value;
				
				// generator_version
				else if (attribute.Name.Equals("generator_version"))
					m_Data.versionInfo.generatorVersion = attribute.Value;
				
				// pixel_art_mode
				else if (attribute.Name.Equals("pixel_art_mode"))
					m_Data.versionInfo.pixelArtModeRaw = attribute.Value;
				
				// Try to parse non-string values
				bool pixelArtMode;
				
				if (bool.TryParse(m_Data.versionInfo.pixelArtModeRaw, out pixelArtMode))
					m_Data.versionInfo.pixelArtMode = pixelArtMode;
			}
		}
		
		// TODO: Make this similar to the other read methods
		void ReadMetaData(XmlElement element, List<SpriterMetaData> metaDataList)
		{
			foreach(XmlElement child in element)
			{
				// tag
				if (child.Name.Equals("tag"))
				{
					SpriterTagMetaData metaData = new SpriterTagMetaData();
					metaDataList.Add(metaData);
					metaData.name = child.Attributes.GetNamedItem("name").Value;
				}
				
				// variable
				else if (child.Name.Equals("variable"))
				{
					var attribute = child.Attributes.GetNamedItem("curve_type");
					
					// tweened variable
					if (attribute != null)
					{
						SpriterTweenedVariableMetaData metaData = new SpriterTweenedVariableMetaData();
						metaDataList.Add(metaData);
						metaData.name = child.Attributes.GetNamedItem("name").Value;
						metaData.variableTypeRaw = child.Attributes.GetNamedItem("type").Value;
						
						// string, int, or float
						if (metaData.variableTypeRaw.Equals("string"))
						{
							metaData.variableType = VariableType.String;
							metaData.value = child.Attributes.GetNamedItem("value").Value;
						}
						else if (metaData.variableTypeRaw.Equals("int"))
						{
							metaData.variableType = VariableType.Int;
							metaData.value = int.Parse(child.Attributes.GetNamedItem("value").Value);
						}
						else if (metaData.variableTypeRaw.Equals("float"))
						{
							metaData.variableType = VariableType.Float;
							metaData.value = float.Parse(child.Attributes.GetNamedItem("value").Value);
						}
						
						metaData.curveTypeRaw = attribute.Value;
						var curveType = Enum.Parse(typeof(CurveType), metaData.curveTypeRaw, true);
						
						metaData.curveType = (CurveType)curveType;
						
						switch(metaData.curveType)
						{
						case CurveType.Quadratic:
							metaData.curveTangents = new Vector2(float.Parse(child.Attributes.GetNamedItem("c1").Value), 0);
							break;
						case CurveType.Cubic:
							metaData.curveTangents = new Vector2(float.Parse(child.Attributes.GetNamedItem("c1").Value), float.Parse(child.Attributes.GetNamedItem("c2").Value));
							break;
						}
					}
					
					// normal variable
					else
					{
						SpriterVariableMetaData metaData = new SpriterVariableMetaData();
						metaDataList.Add(metaData);
						metaData.name = child.Attributes.GetNamedItem("name").Value;
						metaData.variableTypeRaw = child.Attributes.GetNamedItem("type").Value;
						
						// string, int, or float
						if (metaData.variableTypeRaw.Equals("string"))
						{
							metaData.variableType = VariableType.String;
							metaData.value = child.Attributes.GetNamedItem("value").Value;
						}
						else if (metaData.variableTypeRaw.Equals("int"))
						{
							metaData.variableType = VariableType.Int;
							metaData.value = int.Parse(child.Attributes.GetNamedItem("value").Value);
						}
						else if (metaData.variableTypeRaw.Equals("float"))
						{
							metaData.variableType = VariableType.Float;
							metaData.value = float.Parse(child.Attributes.GetNamedItem("value").Value);
						}
					}
				}
			}
		}

		void ReadFolder(XmlElement element)
		{
			int folderID = -1;
			string folderName = string.Empty;
			Vector2 pivot = Vector2.zero;
		
			foreach(XmlAttribute attribute in element.Attributes)
			{	
				// id
				if (attribute.Name.Equals("id"))
					folderID = int.Parse(attribute.Value);
				
				// name
				else if (attribute.Name.Equals("name"))
					folderName = attribute.Value;
			}
			
			foreach(XmlElement child in element)
			{
				if (!child.Name.Equals("file"))
					continue;
				
				SpriterFile file = new SpriterFile();
				m_Data.files.Add(file);
				
				file.folderID = folderID;
				file.folderName = folderName;
				
				foreach(XmlAttribute attribute in child.Attributes)
				{
					// type
					if (attribute.Name.Equals("type"))
					{
						// TODO: FileType
					}
					
					// id
					else if (attribute.Name.Equals("id"))
						file.ID = int.Parse(attribute.Value);
					
					// name
					else if (attribute.Name.Equals("name"))
						file.name = attribute.Value;
					
					// pivot
					else if (attribute.Name.Equals("pivot_x"))
						pivot.x = float.Parse(attribute.Value);
					else if (attribute.Name.Equals("pivot_y"))
						pivot.y = float.Parse(attribute.Value);
						
					// width, height
					else if (attribute.Name.Equals("width"))
						file.width = int.Parse(attribute.Value);
					else if (attribute.Name.Equals("height"))
						file.height = int.Parse(attribute.Value);
					
					// atlas_x, atlas_y
					else if (attribute.Name.Equals("atlas_x"))
						file.atlasX = int.Parse(attribute.Value);
					else if (attribute.Name.Equals("atlas_y"))
						file.atlasY = int.Parse(attribute.Value);
					
					// offset_x, offset_y
					else if (attribute.Name.Equals("offset_x"))
						file.offsetX = int.Parse(attribute.Value);
					else if (attribute.Name.Equals("offset_y"))
						file.offsetY = int.Parse(attribute.Value);
					
					// original_width, original_height
					else if (attribute.Name.Equals("original_width"))
						file.originalWidth = int.Parse(attribute.Value);
					else if (attribute.Name.Equals("original_height"))
						file.originalHeight = int.Parse(attribute.Value);
				
				}
				
				// Assign vector values
				file.pivot = pivot;
			}
		}
		
		// TODO
		void ReadAtlases(XmlElement element)
		{
			throw new NotImplementedException();
		}
		
		// TODO
		void ReadEntity(XmlElement element)
		{
			throw new NotImplementedException();
		}

		void ReadCharacterMap(XmlElement element)
		{
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// id
				if (attribute.Name.Equals("id"))
					m_Data.characterMap.ID = int.Parse(attribute.Value);
				
				// name
				else if (attribute.Name.Equals("name"))
					m_Data.characterMap.name = attribute.Value;
			}
			
			// Maps
			foreach(XmlElement child in element)
			{
				foreach(XmlAttribute attribute in child.Attributes)
				{
					SpriterMap map = new SpriterMap();
					m_Data.characterMap.maps.Add(map);
					
					// atlas
					if (attribute.Name.Equals("atlas"))
						map.atlas = int.Parse(attribute.Value);
					
					// folder
					else if (attribute.Name.Equals("folder"))
						map.folder = int.Parse(attribute.Value);
					
					// file
					else if (attribute.Name.Equals("file"))
						map.file = int.Parse(attribute.Value);
					
					// target_atlas
					else if (attribute.Name.Equals("target_atlas"))
						map.targetAtlas = int.Parse(attribute.Value);
					
					// target_folder
					else if (attribute.Name.Equals("target_folder"))
						map.targetFolder = int.Parse(attribute.Value);
					
					// target_file
					else if (attribute.Name.Equals("target_file"))
						map.targetFile = int.Parse(attribute.Value);
					
					// TODO: Object references
				}
			}
		}

		void ReadDocumentInfo(XmlElement element)
		{
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// author
				if (attribute.Name.Equals("author"))
					m_Data.documentInfo.author = attribute.Value;
				
				// copyright
				else if (attribute.Name.Equals("copyright"))
					m_Data.documentInfo.copyright = attribute.Value;
				
				// license
				else if (attribute.Name.Equals("license"))
					m_Data.documentInfo.license = attribute.Value;
				
				// version
				else if (attribute.Name.Equals("version"))
					m_Data.documentInfo.version = attribute.Value;
				
				// last_modified
				else if (attribute.Name.Equals("last_modified"))
					m_Data.documentInfo.lastModified = attribute.Value;
				
				// notes
				else if (attribute.Name.Equals("notes"))
					m_Data.documentInfo.notes = attribute.Value;
			}
		}
		
		#endregion
		
		#region Save
		/// <summary>
		/// Saves spriter information to SCML.
		/// </summary>
		public void SaveSCML(string path)
		{
			// Convert data back to xml
			throw new System.NotImplementedException();
			
			// Save document
			//scml.Save(path);
		}
		
		// TODO: Save methods
		#endregion
	}
}
#endif