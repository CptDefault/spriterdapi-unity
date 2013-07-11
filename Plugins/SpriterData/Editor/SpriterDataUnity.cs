// NGUI import/export plugin
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

// Some notes:
//
// The SCML file and all images must be imported and within the project's Assets folder.
// This limitation may be removed in a future version.
//
// The default implementation of ImportData works in two phases:
//
// Phase 1 creates a sprite atlas, with all textures. This is handled by extending SpriterDataUnity
// Phase 2 creates a character prefab and animation data. This is handled by SpriterDataUnity itself
//
// Please see SpriterDataNGUI for an example of how to extend this class for your 2D Library
//
// Overrides are planned to allow support for updating an existing atlas and character prefab.
// 
// Due to some strange behaviour in the Unity Editor, animations are not very editable once imported.


using System;
using System.Collections.Generic;
using System.Linq;
using BrashMonkey.Spriter.Data;
using BrashMonkey.Spriter.Data.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace BrashMonkey.Spriter.DataPlugins
{
	public abstract class SpriterDataUnity : SpriterData
	{
		#region Internal types
		protected class SpriteEntry
		{
			public Texture2D Tex; // Sprite texture -- original texture or a temporary texture
			public Rect Rect; // Sprite's outer rectangle within the generated texture atlas
			public int MinX; // Padding, if any (set if the sprite is trimmed)
			public int MaxX;
			public int MinY;
			public int MaxY;
			public bool TemporaryTexture; // Whether the texture is temporary and should be deleted
		}
		/*[Flags]
		enum TangentMode
		{
			Editable = 0,
			//Smooth = 1,
			//Linear = 2,
			Stepped = 3,
		}*/
		#endregion
		
		#region Private fields
		const int KLeftTangentMask = 1 << 1 | 1 << 2;
		const int KRightTangentMask = 1 << 3 | 1 << 4;	
		#endregion
		
		

		protected abstract void CreateSpriteAtlas();
		protected abstract string GetSaveFolder();
		protected abstract void GetSpriteInfo(SpriterNGUIColorHelper helper, out Vector2 paddingTL, out Vector2 paddingBR, out Vector2 size);
		protected abstract void AddSprite(ISpriterTimelineObject obj, GameObject go);

		#region Character prefab creation

		private void CreateCharacterPrefab()
		{
			// All character creation is done in the hierarchy, then saved to a prefab
			
			// Create root object
			Transform characterRoot = new GameObject(entity.name).transform;
			characterRoot.gameObject.AddComponent<Animation>();
			
			if (Selection.activeTransform)
			{
				characterRoot.parent = Selection.activeTransform;
				characterRoot.localScale = Vector3.one;
			}

			//Do an inital pass to ensure all sprites are created
			foreach (SpriterAnimation anim in entity.animations)
			{
				// Create an animation clip
				var tempClip = new AnimationClip();
				var curves = new Dictionary<string, AnimationCurve>[14];
				for (int i = 0; i < 14; i++)
				{
					curves[i] = new Dictionary<string, AnimationCurve>();
				}

				// Process each keyframe
				foreach (SpriterMainlineKey t in anim.mainline.keys)
				{
					RecordFrame(anim, characterRoot, t, null, t.time/1000f, tempClip.frameRate, curves);
				}
			}

			// Go through each animation
			foreach(SpriterAnimation anim in entity.animations)
			{
				// Create an animation clip
				var clip = new AnimationClip();
				clip.EnsureQuaternionContinuity();
				clip.name = anim.name;
				clip.wrapMode = WrapMode.Loop;

				bool last = anim == entity.animations.LastOrDefault();

				// Create animation curves
				var curves = new Dictionary<string, AnimationCurve>[14];
				
				for(int i = 0; i < 14; i++)
				{
					curves[i] = new Dictionary<string, AnimationCurve>();
				}
				
				// Go through each keyframe
				foreach (SpriterMainlineKey t in anim.mainline.keys)
				{
					// Record the frame
					RecordFrame(anim, characterRoot, t, null, t.time/1000f, clip.frameRate, curves);
				}
				//if there's no keyframe at the end, give an end target to set the animations length
				if (anim.mainline.keys.Last().time < anim.length)
				{
					if (anim.playbackType == PlaybackType.Loop || anim.playbackType == PlaybackType.Unknown)
						RecordFrame(anim, characterRoot, anim.mainline.keys[0], last ? anim.mainline.keys[0] : null, anim.length / 1000f, clip.frameRate,
						            curves);
					else
						RecordFrame(anim, characterRoot, anim.mainline.keys.Last(), last ? anim.mainline.keys.Last() : null, anim.length / 1000f, clip.frameRate,
						            curves);
				}

				switch (anim.playbackType)
				{
					case PlaybackType.Unknown:
						clip.wrapMode = WrapMode.Loop;
						break;
					case PlaybackType.PlayOnce:
						clip.wrapMode = WrapMode.Once;
						break;
					case PlaybackType.Loop:
						clip.wrapMode = WrapMode.Loop;
						break;
					case PlaybackType.PingPong:
						clip.wrapMode = WrapMode.PingPong;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				RemoveTangents(curves);

				// Bind the animation
				BindAnimation(clip, curves);
				
				// Save the animation
				string p = GetSaveFolder() + characterRoot.name + "@" + clip.name + ".anim";
				if (AssetDatabase.LoadAssetAtPath(p, typeof(AnimationClip)))
				{
					AssetDatabase.DeleteAsset(p);
					AssetDatabase.CreateAsset(clip, p);
				}
				else
				{
					AssetDatabase.CreateAsset(clip, p);
				}
				
				// Update the reference
				clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(p, typeof(AnimationClip));
				
				// Add the clip to the root object
				characterRoot.animation.AddClip(clip, clip.name);
				
				if (!characterRoot.animation.clip)
				{
					characterRoot.animation.clip = characterRoot.animation.GetClip(clip.name);
				}
			}
							
			SaveAssets(characterRoot.gameObject);
		}

		private static void RemoveTangents(IEnumerable<Dictionary<string, AnimationCurve>> curves)
		{
			//TODO: Remove duplicate keys?
			foreach (var animationCurve in curves)
			{
				foreach (var curve in animationCurve.Values)
				{
					Keyframe last = curve.keys[0];
					for (int i = 1; i < curve.keys.Length; i++)
					{
						Keyframe keyframe = curve.keys[i];
						var diff = keyframe.value - last.value;
						var time = keyframe.time - last.time;

						last.outTangent = diff/time;
						keyframe.inTangent = diff/time;

						curve.MoveKey(i - 1, last);
						curve.MoveKey(i, keyframe);

						last = keyframe;
					}
				}
			}
		}

		private void RecordFrame(SpriterAnimation anim, Transform characterRoot, SpriterMainlineKey keyframe,
		                         SpriterMainlineKey endKey, float currentTime, float frameRate,
		                         Dictionary<string, AnimationCurve>[] curves)
		{
			// Go through each sprite, keeping track of what is processed
			var processedSprites = new List<Component>();

			var bones = new Dictionary<int, Transform>();
			var scales = new Dictionary<int, Vector2>();

			foreach (SpriterMainlineBoneBase obj in keyframe.hierarchy.bones)
			{
				var obj1 = (obj as ISpriterTimelineBone);
				if (obj1 == null && obj is SpriterMainlineBoneRef)
					obj1 = ((SpriterMainlineBoneRef)obj).target;
				if (obj1 == null)
				{
					Debug.LogException(new Exception("Unknown type"));
					continue;
				}

				SetObject(anim, characterRoot, keyframe == endKey, currentTime, frameRate, curves, processedSprites, obj1, bones, scales, obj);
			}

			//foreach(ISpriterSprite sprite in frame.sprites)
			foreach(SpriterMainlineObjectBase obj in keyframe.objects)
			{
				
				var obj1 = (obj as ISpriterTimelineObject);
				if (obj1 == null && obj is SpriterMainlineObjectRef)
					obj1 = ((SpriterMainlineObjectRef)obj).target;
				if (obj1 == null)
				{
					Debug.LogException(new Exception("Unknown type"));
					continue;
				}


				SetObject(anim, characterRoot, keyframe == endKey, currentTime, frameRate, curves, processedSprites, obj1, bones, scales, null, obj);
			}

			//turn off all unused sprites
			foreach (var colorHelper in characterRoot.GetComponentsInChildren<SpriterNGUIColorHelper>())
			{
				//check if it's been used this keyframe
				if (!processedSprites.Contains(colorHelper))
				{
					//grab the curve. If it doesn't exist, create it.
					AnimationCurve val;
					if (!curves[10].TryGetValue(GetRelativeName(colorHelper, characterRoot), out val))
					{
						val = new AnimationCurve();
						curves[10].Add(GetRelativeName(colorHelper, characterRoot), val);
					}

					//If this is not the first keyframe, and the previous key was visible,
					//make sure we step the value
					int pos = val.AddKey(SteppedKeyframe(currentTime, 0));
					if (pos > 0)
					{
						if (val.keys[pos - 1].value > 0)
						{
							val.AddKey(SteppedKeyframe(currentTime - (.001f / frameRate), val.keys[pos - 1].value));
						}
					}
				}
			}
		}

		private void SetObject(SpriterAnimation anim, Transform characterRoot, bool lastFrame, float currentTime,
		                       float frameRate, Dictionary<string, AnimationCurve>[] curves, List<Component> processedSprites,
							   ISpriterTimelineBone obj1, Dictionary<int, Transform> bones, Dictionary<int, Vector2> scales, SpriterMainlineBoneBase boneBase = null,
		                       SpriterMainlineObjectBase obj = null)
		{
			// Find the sprite object
			SpriterTimelineKey spriterTimelineKey = null;
			
			bool isTransient = false;

			string timelineName = null;
			int id = -1;
			

			var mainlineObjectRef = obj as SpriterMainlineObjectRef;
			var mainlineObject = obj as SpriterMainlineObject;
			if (mainlineObjectRef != null)
			{
				SpriterTimeline spriterTimeline = anim.timelines.Find(timeline => mainlineObjectRef.timeline == timeline.ID);
				spriterTimelineKey = spriterTimeline.keys.Find(key => key.ID == mainlineObjectRef.key);
				timelineName = spriterTimeline.name;
				id = mainlineObjectRef.ID;
			}
			if (mainlineObject != null)
			{
				timelineName = mainlineObject.name;
				id = mainlineObject.ID;
				isTransient = true;
			}
			var boneRef = boneBase as SpriterMainlineBoneRef;
			var bone = boneBase as SpriterMainlineBone;
			if (boneRef != null)
			{
				SpriterTimeline spriterTimeline = anim.timelines.Find(timeline => boneRef.timeline == timeline.ID);
				spriterTimelineKey = spriterTimeline.keys.Find(key => key.ID == boneRef.key);
				timelineName = spriterTimeline.name;
				id = boneRef.ID;
				isTransient = false;
			}
			if (bone != null)
			{
				timelineName = "trans_bone_";
				id = bone.ID;
				isTransient = true;
			}
			//UISprite uiSprite = FindSpriteObject(characterRoot, obj1, timelineName, id);
			//var uiSprite = colorHelper.GetComponent<UISprite>();
			int parent = -1;
			if (obj != null) parent = obj.parent;
			else if (boneRef != null) parent = boneRef.parent;
			else if (bone != null) parent = bone.parent;

			Transform parentT = characterRoot;
			Vector2 localScale = Vector2.one;

			if (parent != -1)
			{
				if (!bones.TryGetValue(parent, out parentT))
					Debug.LogError("Parent " + parent + " does not exist");
				scales.TryGetValue(parent, out localScale);
			}

			Transform animObj = FindObject(parentT, timelineName, id);
			Transform animSpriteObj = null;
			SpriterNGUIColorHelper colorHelper = null;

			animObj.localPosition = new Vector2(obj1.position.x * localScale.x, obj1.position.y * localScale.y);
			localScale.x *= obj1.scale.x;
			localScale.y *= obj1.scale.y;

			if (boneBase != null)
			{
				bones.Add(boneBase.ID, animObj);
				scales.Add(boneBase.ID, localScale);
			}

			var animKeyName = GetRelativeName(animObj, characterRoot);
			string animSpriteKeyName = null;

			//animObj.localScale = obj1.scale;

			var sprObj = obj1 as ISpriterTimelineObject;
			if (sprObj != null)
			{
				System.Diagnostics.Debug.Assert(obj != null, "obj != null");

				colorHelper = FindChildSpriteObject(animObj, (ISpriterTimelineObject)obj1);

				animSpriteObj = colorHelper.transform;
				animSpriteKeyName = GetRelativeName(colorHelper, characterRoot);

				//set this here so that bones don't get screwed with
				animObj.localScale = localScale;

				Vector2 paddingTL;
				Vector2 paddingBR;
				Vector2 size;
				GetSpriteInfo(colorHelper, out paddingTL, out paddingBR, out size);

				float pW = paddingTL.x + paddingBR.x;
				float pH = paddingTL.y + paddingBR.y;

				float w = sprObj.targetFile.width - pW;
				float h = sprObj.targetFile.height - pH;
				colorHelper.transform.localRotation = Quaternion.identity;
				colorHelper.transform.localPosition = new Vector2(-sprObj.pivot.x*sprObj.targetFile.width,
				                                                  (1 - sprObj.pivot.y)*sprObj.targetFile.height)
				                                      + new Vector2(paddingTL.x, -paddingTL.y);

				colorHelper.transform.localScale = new Vector3(w, h, 1);

				colorHelper.color = obj1.color;

				
				colorHelper.depth = obj.zIndex;
			}

			if (spriterTimelineKey != null)
			{
				float lastRot = animObj.localEulerAngles.z;
				float targetRot = ((obj1.angle + 180)%360) - 180;

				//TODO: fix rotation

				if (targetRot - 180 > lastRot)
					targetRot -= 360;
				if (targetRot + 180 < lastRot)
					targetRot += 360;


				/*if (spriterTimelineKey.spin != -1)
					{
						//clockwise
						while (targetRot < lastRot - 5)
							targetRot += 360;
					}
					else
					{
						//counterclockwise
						while (targetRot > lastRot + 5)
							targetRot -= 360;
					}*/

				animObj.localRotation = Quaternion.Euler(0, 0, targetRot);
			}
			else
				animObj.localRotation = Quaternion.Euler(0, 0, ((obj1.angle + 180)%360) - 180);

			

			AnimationCurve currentCurveXPos;
			AnimationCurve currentCurveYPos;
			//AnimationCurve currentCurveZPos;
			AnimationCurve currentCurveXScale;
			AnimationCurve currentCurveYScale;
			//AnimationCurve currentCurveZScale;
			AnimationCurve currentCurveXRot;
			AnimationCurve currentCurveYRot;
			AnimationCurve currentCurveZRot;
			AnimationCurve currentCurveWRot;
			AnimationCurve currentCurveColorR = null;
			AnimationCurve currentCurveColorG = null;
			AnimationCurve currentCurveColorB = null;

			AnimationCurve currentCurveSpriteXPos = null;
			AnimationCurve currentCurveSpriteYPos = null;

			if (!curves[0].ContainsKey(animKeyName))
			{
				// New object, create new animation curves
				currentCurveXPos = new AnimationCurve();
				currentCurveYPos = new AnimationCurve();
				//currentCurveZPos = new AnimationCurve();
				currentCurveXScale = new AnimationCurve();
				currentCurveYScale = new AnimationCurve();
				//currentCurveZScale = new AnimationCurve();
				currentCurveXRot = new AnimationCurve();
				currentCurveYRot = new AnimationCurve();
				currentCurveZRot = new AnimationCurve();
				currentCurveWRot = new AnimationCurve();


				curves[0].Add(animKeyName, currentCurveXPos);
				curves[1].Add(animKeyName, currentCurveYPos);
				//curves[2].Add(animKeyName, currentCurveZPos);
				curves[3].Add(animKeyName, currentCurveXScale);
				curves[4].Add(animKeyName, currentCurveYScale);
				//curves[5].Add(animKeyName, currentCurveZScale);
				curves[6].Add(animKeyName, currentCurveXRot);
				curves[7].Add(animKeyName, currentCurveYRot);
				curves[8].Add(animKeyName, currentCurveZRot);
				curves[9].Add(animKeyName, currentCurveWRot);
			}
			else
			{
				// Get the current animation curves
				currentCurveXPos = curves[0][animKeyName];
				currentCurveYPos = curves[1][animKeyName];
				//currentCurveZPos = curves[2][animKeyName];
				currentCurveXScale = curves[3][animKeyName];
				currentCurveYScale = curves[4][animKeyName];
				//currentCurveZScale = curves[5][animKeyName];
				currentCurveXRot = curves[6][animKeyName];
				currentCurveYRot = curves[7][animKeyName];
				currentCurveZRot = curves[8][animKeyName];
				currentCurveWRot = curves[9][animKeyName];
			}

			if (animSpriteKeyName != null)
			{
				if (!curves[0].ContainsKey(animSpriteKeyName))
				{
					currentCurveSpriteXPos = new AnimationCurve();
					currentCurveSpriteYPos = new AnimationCurve();

					currentCurveColorR = new AnimationCurve();
					currentCurveColorG = new AnimationCurve();
					currentCurveColorB = new AnimationCurve();

					curves[0].Add(animSpriteKeyName, currentCurveSpriteXPos);
					curves[1].Add(animSpriteKeyName, currentCurveSpriteYPos);

					curves[11].Add(animSpriteKeyName, currentCurveColorR);
					curves[12].Add(animSpriteKeyName, currentCurveColorG);
					curves[13].Add(animSpriteKeyName, currentCurveColorB);
				}
				else
				{
					currentCurveSpriteXPos = curves[0][animSpriteKeyName];
					currentCurveSpriteYPos = curves[1][animSpriteKeyName];

					currentCurveColorR = curves[11][animSpriteKeyName];
					currentCurveColorG = curves[12][animSpriteKeyName];
					currentCurveColorB = curves[13][animSpriteKeyName];
				}


				AnimationCurve currentCurveToggle;
				if (!curves[10].ContainsKey(animSpriteKeyName))
				{
					currentCurveToggle = new AnimationCurve();
					curves[10].Add(animSpriteKeyName, currentCurveToggle);

					if (currentTime > 0)
						currentCurveToggle.AddKey(SteppedKeyframe(0, 0));
				}
				else
				{
					currentCurveToggle = curves[10][animSpriteKeyName];
				}
			

				int keyPos = currentCurveToggle.AddKey(SteppedKeyframe(currentTime, colorHelper.color.a));

				if (keyPos > 0)
				{
					// Check if sprite was previously turned off
					if (currentCurveToggle.keys[keyPos - 1].value < 0.01f)
					{
						currentCurveToggle.AddKey(SteppedKeyframe(currentTime - (.001f/frameRate), 0));
					}
				}
			}
			if (isTransient)
			{
				DuplicateLast(currentTime, frameRate, currentCurveXPos);
				DuplicateLast(currentTime, frameRate, currentCurveYPos);

				DuplicateLast(currentTime, frameRate, currentCurveXScale);
				DuplicateLast(currentTime, frameRate, currentCurveYScale);

				DuplicateLast(currentTime, frameRate, currentCurveXRot);
				DuplicateLast(currentTime, frameRate, currentCurveYRot);
				DuplicateLast(currentTime, frameRate, currentCurveZRot);
				DuplicateLast(currentTime, frameRate, currentCurveWRot);
			}

			currentCurveXPos.AddKey(LinearKeyframe(currentTime, animObj.localPosition.x));
			currentCurveYPos.AddKey(LinearKeyframe(currentTime, animObj.localPosition.y));

			currentCurveXScale.AddKey(LinearKeyframe(currentTime, animObj.localScale.x));
			currentCurveYScale.AddKey(LinearKeyframe(currentTime, animObj.localScale.y));

			currentCurveXRot.AddKey(LinearKeyframe(currentTime, animObj.localRotation.x));
			currentCurveYRot.AddKey(LinearKeyframe(currentTime, animObj.localRotation.y));
			currentCurveZRot.AddKey(LinearKeyframe(currentTime, animObj.localRotation.z));
			currentCurveWRot.AddKey(LinearKeyframe(currentTime, animObj.localRotation.w));


			if (animSpriteKeyName != null)
			{
				if (isTransient)
				{
					DuplicateLast(currentTime, frameRate, currentCurveColorR);
					DuplicateLast(currentTime, frameRate, currentCurveColorG);
					DuplicateLast(currentTime, frameRate, currentCurveColorB);

					DuplicateLast(currentTime, frameRate, currentCurveSpriteXPos);
					DuplicateLast(currentTime, frameRate, currentCurveSpriteYPos);
				}
				currentCurveColorR.AddKey(LinearKeyframe(currentTime, colorHelper.color.r));
				currentCurveColorG.AddKey(LinearKeyframe(currentTime, colorHelper.color.g));
				currentCurveColorB.AddKey(LinearKeyframe(currentTime, colorHelper.color.b));

				currentCurveSpriteXPos.AddKey(LinearKeyframe(currentTime, animSpriteObj.localPosition.x));
				currentCurveSpriteYPos.AddKey(LinearKeyframe(currentTime, animSpriteObj.localPosition.y));
			}

			if (colorHelper != null)
			{
				colorHelper.color = lastFrame ? obj1.color : new Color(obj1.color.r, obj1.color.g, obj1.color.b, 0);

				// Mark as processed
				processedSprites.Add(colorHelper);
			}
		}

		private static void DuplicateLast(float currentTime, float frameRate, AnimationCurve curve)
		{
			if(curve.keys.Length < 1)
				return;

			int pos = curve.keys.Length - 1;
			int addKey = curve.AddKey(currentTime - (.001f/frameRate), curve.keys[pos].value);
			if(addKey == -1)
				Debug.LogError("Something went wrong with adding duplicate keys!");
			
		}

		private static string GetRelativeName(Component component, Transform root)
		{
			string name = component.transform.name;

			Transform t = component.transform.parent;

			while (t != root)
			{
				name = t.name + "/" + name;
				t = t.parent;
			}

			return name;
		}

/*
		static void SetKeyTangentMode (ref Keyframe key, int leftRight, TangentMode mode)
		{
			if (leftRight == 0)
			{
				key.tangentMode &= ~KLeftTangentMask;
				key.tangentMode |= (int)mode << 1;
			}
			else
			{
				key.tangentMode &= ~KRightTangentMask;
				key.tangentMode |= (int)mode << 3;
			}
			
			if (GetKeyTangentMode (key, leftRight) != mode)
				Debug.Log("problem");
		}
*/

/*
		static TangentMode GetKeyTangentMode (Keyframe key, int leftRight)
		{
			if (leftRight == 0)
			{
				return (TangentMode)((key.tangentMode & KLeftTangentMask) >> 1);
			}
			return (TangentMode)((key.tangentMode & KRightTangentMask) >> 3);
		}
*/

		static Keyframe LinearKeyframe(float time, float value)
		{
			return SteppedKeyframe(time, value);
/*
			var result = new Keyframe {value = value, time = time};
			//SetKeyTangentMode(ref result, 0, TangentMode.Editable | TangentMode.Linear);
			//SetKeyTangentMode(ref result, 1, TangentMode.Editable | TangentMode.Linear);
			
			return result;
*/
		}

		static Keyframe SteppedKeyframe(float time, float value)
		{
			var result = new Keyframe {value = value, time = time};
			//SetKeyTangentMode(ref result, 0, TangentMode.Editable | TangentMode.Stepped);
			//SetKeyTangentMode(ref result, 1, TangentMode.Editable | TangentMode.Stepped);
			
			

			return result;
		}

		static void BindAnimation(AnimationClip clip, Dictionary<string, AnimationCurve>[] curves)
		{
			// Bind the curves to the animation clip
			BindAnimation(clip, curves[0], "localPosition.x", typeof (Transform));
			BindAnimation(clip, curves[1], "localPosition.y", typeof (Transform));
			BindAnimation(clip, curves[2], "localPosition.z", typeof (Transform));

			BindAnimation(clip, curves[3], "localScale.x", typeof (Transform));
			BindAnimation(clip, curves[4], "localScale.y", typeof (Transform));
			BindAnimation(clip, curves[5], "localScale.z", typeof (Transform));

			BindAnimation(clip, curves[6], "localRotation.x", typeof (Transform));
			BindAnimation(clip, curves[7], "localRotation.y", typeof (Transform));
			BindAnimation(clip, curves[8], "localRotation.z", typeof (Transform));
			BindAnimation(clip, curves[9], "localRotation.w", typeof (Transform));

			BindAnimation(clip, curves[10], "color.a", typeof(SpriterNGUIColorHelper));
			BindAnimation(clip, curves[11], "color.r", typeof(SpriterNGUIColorHelper));
			BindAnimation(clip, curves[12], "color.g", typeof(SpriterNGUIColorHelper));
			BindAnimation(clip, curves[13], "color.b", typeof(SpriterNGUIColorHelper));
		}

		private static void BindAnimation(AnimationClip clip, Dictionary<string, AnimationCurve> animationCurves, string animProperty, Type type)
		{
			foreach (var kvp in animationCurves)
			{
				if (kvp.Value.length > 0)
				{
					clip.SetCurve(kvp.Key, type, animProperty, kvp.Value);
				}
			}
		}

		Transform FindObject(Transform parent, string name, int id = -1)
		{
			string baseName = (name) + (id == -1 ? "" : "_" + id);

			//find base object (the animation node). Create if it doesn't exist
			var baseGO = parent.Find(baseName);
			if (baseGO == null)
			{
				baseGO = new GameObject(baseName).transform;
				baseGO.parent = parent;
				baseGO.localPosition = Vector3.zero;
			}

			return baseGO;
		}

		SpriterNGUIColorHelper FindChildSpriteObject(Transform baseGO, ISpriterTimelineObject obj)
		{
			string spriteName = GetSpriteName(obj.targetFile.name);

			//create the UISprite object
			var t = baseGO.Find(spriteName);
			if (t != null)
				return t.GetComponent<SpriterNGUIColorHelper>();

			var go = new GameObject(spriteName);
			go.transform.parent = baseGO;
			go.transform.localPosition = Vector3.zero;

			AddSprite(obj, go);

			var result = go.AddComponent<SpriterNGUIColorHelper>();

			return result;
			
		}

		

		void SaveAssets(GameObject root)
		{	
#if UNITY_3_5
			Transform parent = root.transform.parent;
			var prefab = PrefabUtility.CreatePrefab(NGUIEditorTools.GetSelectionFolder() + root.name + ".prefab", root);
			UnityEngine.Object.DestroyImmediate(root);
			var newObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			newObj.transform.parent = parent;
			newObj.transform.localScale = Vector3.one;
#else
			Transform parent = root.transform.parent;
			var prefab = PrefabUtility.CreateEmptyPrefab(GetSaveFolder() + root.name + "_.prefab");
			prefab = PrefabUtility.ReplacePrefab(root, prefab);
			UnityEngine.Object.DestroyImmediate(root);
			var newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			newObj.transform.parent = parent;
			newObj.transform.localScale = Vector3.one;
#endif		
			AssetDatabase.Refresh();
		}
		protected string GetSpriteName(string imagePath)
		{
			int index = imagePath.LastIndexOf('/') + 1;
			string file = index == -1 ? imagePath : imagePath.Substring(index);
			return file.Substring(0, file.LastIndexOf('.'));
		}
		#endregion
		
		#region ISpriterData implementation

		protected override void FromImplementation()
		{
			throw new NotImplementedException();
		}

		protected override void ToImplementation()
		{			
			CreateSpriteAtlas();
			CreateCharacterPrefab();
		}
		#endregion
	}
}