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
							ReadAtlas(child);
						
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
		
		void ReadMetaData(XmlElement element, List<SpriterMetaData> metaDataList)
		{
			foreach(XmlElement child in element)
			{
				// tag
				if (child.Name.Equals("tag"))
				{
					SpriterTagMetaData metaData = new SpriterTagMetaData();
					metaDataList.Add(metaData);
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// name
						if (attribute.Name.Equals("name"))
							metaData.name = attribute.Value;
					}
				}
				
				// variable
				else if (child.Name.Equals("variable"))
				{
					bool isTweenedVariable = child.Attributes.GetNamedItem("curve_type") != null;
					
					// tweened variable
					if (isTweenedVariable)
					{
						SpriterTweenedVariableMetaData metaData = new SpriterTweenedVariableMetaData();
						metaDataList.Add(metaData);
						
						Vector2 curveTangents = Vector2.zero;
						
						foreach(XmlAttribute attribute in child.Attributes)
						{
							// name
							if (attribute.Name.Equals("name"))
								metaData.name = attribute.Value;
							
							// type
							else if (attribute.Name.Equals("type"))
							{
								metaData.variableTypeRaw = attribute.Value;
								metaData.variableType = SpriterDataHelpers.ParseSpriterEnum<VariableType>(metaData.variableTypeRaw);
							}
							
							// value
							else if (attribute.Name.Equals("value"))
							{
								switch (metaData.variableType)
								{
								case VariableType.String:
									metaData.value = attribute.Value;
									break;
								case VariableType.Int:
									metaData.value = int.Parse(attribute.Value);
									break;
								case VariableType.Float:
									metaData.value = float.Parse(attribute.Value);
									break;
								}
							}
							
							// curve_type
							else if (attribute.Name.Equals("curve_type"))
							{
								metaData.curveTypeRaw = attribute.Value;
								metaData.curveType = SpriterDataHelpers.ParseSpriterEnum<CurveType>(metaData.curveTypeRaw);
							}
							
							// c1, c2
							else if (attribute.Name.Equals("c1"))
								curveTangents.x = float.Parse(attribute.Value);
							else if (attribute.Name.Equals("c2"))
								curveTangents.y = float.Parse(attribute.Value);
						}
						
						// Assign vector values
						metaData.curveTangents = curveTangents;
					}
					
					// normal variable
					else
					{
						SpriterVariableMetaData metaData = new SpriterVariableMetaData();
						metaDataList.Add(metaData);
						
						foreach(XmlAttribute attribute in child.Attributes)
						{
							// name
							if (attribute.Name.Equals("name"))
								metaData.name = attribute.Value;
							
							// type
							else if (attribute.Name.Equals("type"))
							{
								metaData.variableTypeRaw = attribute.Value;
								metaData.variableType = SpriterDataHelpers.ParseSpriterEnum<VariableType>(metaData.variableTypeRaw);
							}
							
							// value
							else if (attribute.Name.Equals("value"))
							{
								switch (metaData.variableType)
								{
								case VariableType.String:
									metaData.value = attribute.Value;
									break;
								case VariableType.Int:
									metaData.value = int.Parse(attribute.Value);
									break;
								case VariableType.Float:
									metaData.value = float.Parse(attribute.Value);
									break;
								}
							}
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
						file.typeRaw = attribute.Value;
						file.type = SpriterDataHelpers.ParseSpriterEnum<FileType>(file.typeRaw);
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
		
		void ReadAtlas(XmlElement element)
		{
			SpriterAtlas atlas = new SpriterAtlas();
			m_Data.atlases.Add(atlas);
			int folderID = 0;
			string folderName = string.Empty;
			
			foreach(XmlAttribute attribute in element.Attributes)
			{	
				// id
				if (attribute.Name.Equals("id"))
					atlas.ID = int.Parse(attribute.Value);
				
				// data_path
				else if (attribute.Name.Equals("data_path"))
					atlas.dataPath = attribute.Value;
			
				// image_path
				else if (attribute.Name.Equals("image_path"))
					atlas.imagePath = attribute.Value;
			}
			
			foreach(XmlElement child in element)
			{	
				if (child.Name.Equals("folder"))
				{
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// folder id
						if (attribute.Name.Equals("id"))
							folderID = int.Parse(attribute.Value);
						
						// folder name
						else if (attribute.Name.Equals("name"))
							folderName = attribute.Value;
					}
					
					foreach(XmlElement child2 in child)
					{
						SpriterAtlasImage image = new SpriterAtlasImage();
						atlas.images.Add(image);
						
						foreach(XmlAttribute attribute in child2.Attributes)
						{
							// image id
							if (attribute.Name.Equals("id"))
								image.ID = int.Parse(attribute.Value);
							
							// image full_path
							else if (attribute.Name.Equals("full_path"))
								image.fullPath = attribute.Value;
						}
						
						// other properties
						image.folderID = folderID;
						image.folderName = folderName;
					}
				}
			}
		}
		
		void ReadEntity(XmlElement element)
		{
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// id
				if (attribute.Name.Equals("id"))
					m_Data.entity.ID = int.Parse(attribute.Value);
				
				// name
				else if (attribute.Name.Equals("name"))
					m_Data.entity.name = attribute.Value;
			}
			
			foreach(XmlElement child in element)
			{
				// meta_data
				if (child.Name.Equals("meta_data"))
					ReadMetaData(child, m_Data.entity.metaData);
				
				// animation
				else if (child.Name.Equals("animation"))
				{
					SpriterAnimation animation = new SpriterAnimation();
					m_Data.entity.animations.Add(animation);
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// id
						if (attribute.Name.Equals("id"))
							animation.ID = int.Parse(attribute.Value);
						
						// name
						else if (attribute.Name.Equals("name"))
							animation.name = attribute.Value;
					
						// length
						else if (attribute.Name.Equals("length"))
							animation.length = int.Parse(attribute.Value);
						
						// looping
						else if (attribute.Name.Equals("looping"))
						{
							animation.playbackTypeRaw = attribute.Value;
							animation.playbackType = SpriterDataHelpers.ParseSpriterEnum<PlaybackType>(animation.playbackTypeRaw);
						}
						
						// loop_to
						else if (attribute.Name.Equals("loop_to"))
							animation.loopTo = int.Parse(attribute.Value);
					}
					
					foreach(XmlElement child2 in child)
					{
						// meta_data
						if (child2.Name.Equals("meta_data"))
							ReadMetaData(child2, animation.metaData);
						
						// mainline
						else if (child2.Name.Equals("mainline"))
							ReadMainline(child2, animation);
						
						// timeline
						else if (child2.Name.Equals("timeline"))
							ReadTimeline(child2, animation);
					}
				}
			}
		}
		
		void ReadMainline(XmlElement element, SpriterAnimation animation)
		{
			foreach(XmlElement child in element)
			{
				// key
				if (child.Name.Equals("key"))
				{
					SpriterMainlineKey key = new SpriterMainlineKey();
					animation.mainline.keys.Add(key);
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// id
						if (attribute.Name.Equals("id"))
							key.ID = int.Parse(attribute.Value);
						
						// time
						else if (attribute.Name.Equals("time"))
							key.time = int.Parse(attribute.Value);
					}
					
					foreach(XmlElement child2 in child)
					{
						// meta_data
						if (child2.Name.Equals("meta_data"))
							ReadMetaData(child2, key.metaData);
						
						// hierarchy
						else if (child2.Name.Equals("hierarchy"))
							ReadHierarchy(child2, key.hierarchy);
						
						// object
						else if (child2.Name.Equals("object"))
							ReadMainlineObject(child2, key);
						
						// object_ref
						else if (child2.Name.Equals("object_ref"))
							ReadMainlineObjectRef(child2, key);
					}
				}
			}
		}
		
		void ReadHierarchy(XmlElement element, SpriterHierarchy hierarchy)
		{
			foreach(XmlElement child in element)
			{
				// bone
				if (child.Name.Equals("bone"))
				{
					SpriterMainlineBone bone = new SpriterMainlineBone();
					hierarchy.bones.Add(bone);
					
					Vector2 position = Vector2.zero, scale = Vector2.zero;
					Color color = Color.white;
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// id
						if (attribute.Name.Equals("id"))
							bone.ID = int.Parse(attribute.Value);
						
						// parent
						else if (attribute.Name.Equals("parent"))
							bone.parent = int.Parse(attribute.Value);
						
						// x, y
						else if (attribute.Name.Equals("x"))
							position.x = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("y"))
							position.y = float.Parse(attribute.Value);
						
						// angle
						else if (attribute.Name.Equals("angle"))
							bone.angle = float.Parse(attribute.Value);
						
						// scale_x, scale_y
						else if (attribute.Name.Equals("scale_x"))
							scale.x = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("scale_y"))
							scale.y = float.Parse(attribute.Value);
						
						// r, g, b, a
						else if (attribute.Name.Equals("r"))
							color.r = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("g"))
							color.g = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("b"))
							color.b = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("a"))
							color.a = float.Parse(attribute.Value);
					}
					
					// Assign vector values
					bone.position = position;
					bone.color = color;
					
					foreach(XmlElement child2 in child)
					{
						// meta_data
						if (child2.Name.Equals("meta_data"))
							ReadMetaData(child2, bone.metaData);
					}
				}
				
				// bone_ref
				else if (child.Name.Equals("bone_ref"))
				{
					SpriterMainlineBoneRef boneRef = new SpriterMainlineBoneRef();
					hierarchy.bones.Add(boneRef);
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// id
						if (attribute.Name.Equals("id"))
							boneRef.ID = int.Parse(attribute.Value);
						
						// parent
						else if (attribute.Name.Equals("parent"))
							boneRef.parent = int.Parse(attribute.Value);
						
						// timeline
						else if (attribute.Name.Equals("timeline"))
							boneRef.timeline = int.Parse(attribute.Value);
						
						// key
						else if (attribute.Name.Equals("key"))
							boneRef.key = int.Parse(attribute.Value);
						
						// TODO: Object reference - not in this method
					}
				}
			}
		}
		
		void ReadMainlineObject(XmlElement element, SpriterMainlineKey key)
		{
			SpriterMainlineObject obj = new SpriterMainlineObject();
			key.objects.Add(obj);
			
			Vector2 position = Vector2.zero, pivot = Vector2.zero, scale = Vector2.zero;
			Color color = Color.white;
			
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// id
				if (attribute.Name.Equals("id"))
					obj.ID = int.Parse(attribute.Value);
				
				// parent
				else if (attribute.Name.Equals("parent"))
					obj.parent = int.Parse(attribute.Value);
				
				// object_type
				else if (attribute.Name.Equals("object_type"))
				{
					obj.objectTypeRaw = attribute.Value;
					obj.objectType = SpriterDataHelpers.ParseSpriterEnum<ObjectType>(obj.objectTypeRaw);
				}
				
				// atlas
				else if (attribute.Name.Equals("atlas"))
					obj.atlas = int.Parse(attribute.Value);
				
				// folder
				else if (attribute.Name.Equals("folder"))
					obj.folder = int.Parse(attribute.Value);

				// file
				else if (attribute.Name.Equals("file"))
					obj.file = int.Parse(attribute.Value);

				
				// TODO: Object references
			
				// usage
				else if (attribute.Name.Equals("usage"))
				{
					obj.usageRaw = attribute.Value;
					obj.usage = SpriterDataHelpers.ParseSpriterEnum<UsageType>(obj.usageRaw);
				}
				
				// blend_mode
				else if (attribute.Name.Equals("blend_mode"))
				{
					obj.blendModeRaw = attribute.Value;
					obj.blendMode = SpriterDataHelpers.ParseSpriterEnum<BlendMode>(obj.blendModeRaw);
				}
				
				// name
				else if (attribute.Name.Equals("name"))
					obj.name = attribute.Value;
				
				// x, y
				else if (attribute.Name.Equals("x"))
					position.x = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("y"))
					position.y = float.Parse(attribute.Value);
				
				// pivot_x, pivot_y
				else if (attribute.Name.Equals("pivot_x"))
					pivot.x = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("pivot_y"))
					pivot.y = float.Parse(attribute.Value);
				
				// angle
				else if (attribute.Name.Equals("angle"))
					obj.angle = float.Parse(attribute.Value);
				
				// w, h
				else if (attribute.Name.Equals("w"))
					obj.pixelWidth = int.Parse(attribute.Value);
				else if (attribute.Name.Equals("h"))
					obj.pixelHeight = int.Parse(attribute.Value);
				
				// scale_x, scale_y
				else if (attribute.Name.Equals("scale_x"))
					scale.x = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("scale_y"))
					scale.y = float.Parse(attribute.Value);
				
				// r, g, b, a
				else if (attribute.Name.Equals("r"))
					color.r = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("g"))
					color.g = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("b"))
					color.b = float.Parse(attribute.Value);
				else if (attribute.Name.Equals("a"))
					color.a = float.Parse(attribute.Value);
				
				// variable_type
				else if (attribute.Name.Equals("variable_type"))
				{
					obj.variableTypeRaw = attribute.Value;
					obj.variableType = SpriterDataHelpers.ParseSpriterEnum<VariableType>(obj.variableTypeRaw);
				}
				
				// value
				else if (attribute.Name.Equals("value"))
				{
					switch (obj.variableType)
					{
					case VariableType.String:
						obj.value = attribute.Value;
						break;
					case VariableType.Int:
						obj.value = int.Parse(attribute.Value);
						break;
					case VariableType.Float:
						obj.value = float.Parse(attribute.Value);
						break;
					}
				}
				
				// min, max
				else if (attribute.Name.Equals("min"))
				{
					switch (obj.variableType)
					{
					case VariableType.Int:
						obj.value = int.Parse(attribute.Value);
						break;
					case VariableType.Float:
						obj.value = float.Parse(attribute.Value);
						break;
					}
				}
				else if (attribute.Name.Equals("max"))
				{
					switch (obj.variableType)
					{
					case VariableType.Int:
						obj.value = int.Parse(attribute.Value);
						break;
					case VariableType.Float:
						obj.value = float.Parse(attribute.Value);
						break;
					}
				}
				
				// animation
				else if (attribute.Name.Equals("animation"))
					obj.entityAnimation = int.Parse(attribute.Value);
				
				// t
				else if (attribute.Name.Equals("t"))
					obj.entityT = float.Parse(attribute.Value);
				
				// z_index
				else if (attribute.Value.Equals("z_index"))
					obj.zIndex = int.Parse(attribute.Value);
				
				// volume
				else if (attribute.Name.Equals("volume"))
					obj.volume = float.Parse(attribute.Value);
				
				// panning
				else if (attribute.Name.Equals("panning"))
					obj.panning = float.Parse(attribute.Value);
			}
			
			// Assign vector values
			obj.position = position;
			obj.pivot = pivot;
			obj.scale = scale;
			obj.color = color;
			
			foreach(XmlElement child in element)
			{
				// meta_data
				if (child.Name.Equals("meta_data"))
					ReadMetaData(child, obj.metaData);
			}
		}
		
		void ReadMainlineObjectRef(XmlElement element, SpriterMainlineKey key)
		{
			SpriterMainlineObjectRef obj = new SpriterMainlineObjectRef();
			key.objects.Add(obj);
		
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// id
				if (attribute.Name.Equals("id"))
					obj.ID = int.Parse(attribute.Value);
				
				// parent
				else if (attribute.Name.Equals("parent"))
					obj.parent = int.Parse(attribute.Value);
				
				// timeline
				else if (attribute.Name.Equals("timeline"))
					obj.timeline = int.Parse(attribute.Value);
				
				// key
				else if (attribute.Name.Equals("key"))
					obj.key = int.Parse(attribute.Value);
				
				// TODO: Object reference - not in this method
		
				// z_index
				else if (attribute.Value.Equals("z_index"))
					obj.zIndex = int.Parse(attribute.Value);
			}
		}
		
		void ReadTimeline(XmlElement element, SpriterAnimation animation)
		{
			SpriterTimeline timeline = new SpriterTimeline();
			animation.timelines.Add(timeline);
			
			foreach(XmlAttribute attribute in element.Attributes)
			{
				// id
				if (attribute.Name.Equals("id"))
					timeline.ID = int.Parse(attribute.Value);
				
				// name
				else if (attribute.Name.Equals("name"))
					timeline.name = attribute.Value;
				
				// object_type
				else if (attribute.Name.Equals("object_type"))
				{
					timeline.objectTypeRaw = attribute.Value;
					timeline.objectType = SpriterDataHelpers.ParseSpriterEnum<ObjectType>(timeline.objectTypeRaw);
				}
				
				// variable_type
				else if (attribute.Name.Equals("variable_type"))
				{
					timeline.variableTypeRaw = attribute.Value;
					timeline.variableType = SpriterDataHelpers.ParseSpriterEnum<VariableType>(timeline.variableTypeRaw);
				}
				
				// usage
				else if (attribute.Name.Equals("usage"))
				{
					timeline.usageRaw = attribute.Value;
					timeline.usage = SpriterDataHelpers.ParseSpriterEnum<UsageType>(timeline.usageRaw);
				}
			}
			
			foreach(XmlElement child in element)
			{
				// meta_data
				if (child.Name.Equals("meta_data"))
					ReadMetaData(child, timeline.metaData);
				
				// key
				else if (child.Name.Equals("key"))
				{
					SpriterTimelineKey key = new SpriterTimelineKey();
					timeline.keys.Add(key);
					
					Vector2 tangents = Vector2.zero;
					
					foreach(XmlAttribute attribute in child.Attributes)
					{
						// id
						if (attribute.Name.Equals("id"))
							key.ID = int.Parse(attribute.Value);
						
						// time
						else if (attribute.Name.Equals("time"))
							key.time = int.Parse(attribute.Value);
						
						// curve_type
						else if (attribute.Name.Equals("curve_type"))
						{
							key.curveTypeRaw = attribute.Value;
							key.curveType = SpriterDataHelpers.ParseSpriterEnum<CurveType>(key.curveTypeRaw);
						}
						
						// c1, c2
						else if (attribute.Name.Equals("c1"))
							tangents.x = float.Parse(attribute.Value);
						else if (attribute.Name.Equals("c2"))
							tangents.y = float.Parse(attribute.Value);
						
						// spin
						else if (attribute.Name.Equals("spin"))
							key.spin = int.Parse(attribute.Value);
					}
					
					// Assign vector values
					key.curveTangents = tangents;
					
					foreach(XmlElement child2 in child)
					{
						// meta_data
						if (child2.Name.Equals("meta_data"))
							ReadMetaData(child2, key.metaData);
						
						// bone
						else if (child2.Name.Equals("bone"))
							ReadTimelineBone(child2, key);
						
						// object
						else if (child2.Name.Equals("object"))
							ReadTimelineObject(child2, key);
					}
				}
			}
		}
		
		void ReadTimelineBone(XmlElement element, SpriterTimelineKey key)
		{
			throw new NotImplementedException();
		}
		
		void ReadTimelineObject(XmlElement element, SpriterTimelineKey key)
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