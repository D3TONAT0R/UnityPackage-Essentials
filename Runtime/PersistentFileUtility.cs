using System;
using System.IO;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Contains helper classes for getting common file paths to save persistent files in.
	/// </summary>
	public static class PersistentFileUtility
	{
		/// <summary>
		/// The possible file location to store files in.
		/// </summary>
		public enum FileLocation
		{
			/// <summary>
			/// The "_Data" folder next to the game's executable or the project's root folder when in the editor.
			/// </summary>
			DataPath = 0,
			/// <summary>
			/// Unity Editor: Always returns the project's root folder.
			/// Windows: Usually points to %userprofile%\AppData\LocalLow\(companyname)\(productname).
			/// Mac: Points to the user Library folder. (This folder is often hidden)
			/// </summary>
			PersistentDataPath = 1,
			/// <summary>
			/// The current user's documents folder defined by the operating system. Always returns the project's root folder when in the editor.
			/// </summary>
			Documents = 2
		}

		/// <summary>
		/// The default file location to use in this project.
		/// </summary>
		public static FileLocation DefaultFileLocation { get; set; } = FileLocation.PersistentDataPath;

		/// <summary>
		/// The actual path to the default file location.
		/// </summary>
		public static string DefaultRootPath => GetRootPathForLocation(DefaultFileLocation);

		/// <summary>
		/// Returns the actual path to the given file location.
		/// </summary>
		public static string GetRootPathForLocation(FileLocation location)
		{
			if(Application.isEditor)
			{
				return Directory.GetParent(Application.dataPath).ToString();
			}
			if((int)location >= 10) location -= 10;

			if(location == FileLocation.PersistentDataPath) return Application.persistentDataPath;
			else if(location == FileLocation.Documents) return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.productName);
			else if(location == FileLocation.DataPath) return Application.dataPath;
			else throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the full path for the given location and subfolder / file name.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public static string GetFullPath(FileLocation location, string relativePath)
		{
			return Path.Combine(GetRootPathForLocation(location), relativePath);
		}

		/// <summary>
		/// Returns the full path for the default file path and the given subfolder / file name.
		/// </summary>
		public static string GetDefaultFullPath(string relativePath)
		{
			return GetFullPath(DefaultFileLocation, relativePath);
		}
	}
}
