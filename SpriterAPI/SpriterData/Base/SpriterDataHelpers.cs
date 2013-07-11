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
using System;

namespace BrashMonkey.Spriter.Data
{
	/// <summary>
	/// Helper functions.
	/// </summary>
	internal static class SpriterDataHelpers
	{
		/// <summary>
		/// Converts a UNIX timestamp to a DateTime object, in UTC
		/// </summary>
		/// <remarks>
		/// UNIX timestamp is in seconds past epoch (64-bit)
		/// </remarks>
		public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
		{
		    DateTime epoch = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
		    epoch = epoch.AddSeconds((double)unixTimeStamp);
		    return epoch;
		}
		
		/// <summary>
		/// Converts a UTC DateTime object to a UNIX timestamp
		/// </summary>
		/// <remarks>
		/// UNIX timestamp is in seconds past epoch (64-bit)
		/// </remarks>
		public static long DataTimeToUnixTimeStamp(DateTime dateTime)
		{
			DateTime epoch = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
			return (long)((dateTime - epoch).TotalSeconds);
		}
		
		/// <summary>
		/// Parses a Spriter enumeration based on input.
		/// </summary>
		public static T ParseSpriterEnum<T>(string input) where T: struct
		{
			// C# enum boxing/unboxing makes me sadfaced. :(
			
			Type enumType = typeof(T);
			
			if (enumType == typeof(FileType))
			{
				switch(input)
				{
				case "image":
					return (T)((object)FileType.Image);
				case "sound_effect":
					return (T)((object)FileType.SoundEffect);
				case "atlas_image":
					return (T)((object)FileType.AtlasImage);
				case "entity":
					return (T)((object)FileType.Entity);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(PlaybackType))
			{
				switch(input)
				{
				case "true":
					return (T)((object)PlaybackType.Loop);
				case "false":
					return (T)((object)PlaybackType.PlayOnce);
				case "ping_pong":
					return (T)((object)PlaybackType.PingPong);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(ObjectType))
			{
				switch(input)
				{
				case "point":
					return (T)((object)ObjectType.Point);
				case "box":
					return (T)((object)ObjectType.Box);
				case "sprite":
					return (T)((object)ObjectType.Sprite);
				case "sound":
					return (T)((object)ObjectType.Sound);
				case "entity":
					return (T)((object)ObjectType.Entity);
				case "variable":
					return (T)((object)ObjectType.Variable);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(VariableType))
			{
				switch(input)
				{
				case "string":
					return (T)((object)VariableType.String);
				case "int":
					return (T)((object)VariableType.Int);
				case "float":
					return (T)((object)VariableType.Float);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(UsageType))
			{
				switch(input)
				{
				case "display":
					return (T)((object)UsageType.Display);
				case "collision":
					return (T)((object)UsageType.Collision);
				case "both":
					return (T)((object)UsageType.Both);
				case "neither":
					return (T)((object)UsageType.Neither);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(BlendMode))
			{
				switch(input)
				{
				case "alpha":
					return (T)((object)BlendMode.Alpha);
				case "additive":
					return (T)((object)BlendMode.Additive);
				case "subtractive":
					return (T)((object)BlendMode.Subtractive);
				default:
					return default(T);
				}
			}
			else if (enumType == typeof(CurveType))
			{
				switch(input)
				{
				case "instant":
					return (T)((object)CurveType.Instant);
				case "linear":
					return (T)((object)CurveType.Linear);
				case "quadratic":
					return (T)((object)CurveType.Quadratic);
				case "cubic":
					return (T)((object)CurveType.Cubic);
				default:
					return default(T);
				}
			}
			
			// Unknown enumeration
			return default(T);
		}
	}
}