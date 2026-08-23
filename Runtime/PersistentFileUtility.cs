using System;
using System.IO;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Contains helper classes for getting common file paths to save persistent files in.
	/// </summary>
	public static class PersistentFileUtility
	{
		/// <summary>
		/// The file location to store files in.
		/// </summary>
		public enum FileLocation
		{
			/// <summary>
			/// The data directory next to the game's executable.
			/// </summary>
			DataPath = 0,
			/// <summary>
			/// Unity Editor: Always returns the project's root folder.
			/// Windows: Usually points to %userprofile%\AppData\LocalLow\(companyname)\(productname).
			/// Mac: Points to the user Library folder. (This folder is often hidden)
			/// </summary>
			PersistentDataPath = 1,
			/// <summary>
			/// The current user's documents folder defined by the operating system.
			/// </summary>
			Documents = 2,
			/// <summary>
			/// Windows: Usually points to %userprofile%\AppData\LocalLow\(companyname)\(productname).
			/// Mac: Points to the user Library folder. (This folder is often hidden)
			/// </summary>
			PersistentDataPathOrProjectRoot = 3,
			/// <summary>
			/// The current user's documents folder defined by the operating system. Returns the project's root folder when in the editor.
			/// </summary>
			DocumentsOrProjectRoot = 4,
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
			switch (location)
			{
				case FileLocation.DataPath:
					return Application.dataPath;
				case FileLocation.PersistentDataPath:
					return Application.persistentDataPath;
				case FileLocation.Documents:
					return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.productName);
				case FileLocation.PersistentDataPathOrProjectRoot:
					return Application.isEditor ? GetProjectRootPath() : Application.persistentDataPath;
				case FileLocation.DocumentsOrProjectRoot:
					return Application.isEditor
						? GetProjectRootPath()
						: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.productName);
				default:
					throw new NotImplementedException();
			}
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
		
		private static string GetProjectRootPath()
		{
			return Directory.GetParent(Application.dataPath).ToString();
		}
	}
}
