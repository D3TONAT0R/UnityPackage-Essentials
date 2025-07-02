using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor
{
	/// <summary>
	/// Struct representing an asset reference in the asset database.
	/// </summary>
	public struct AssetReference
	{
		/// <summary>
		/// The GUID of this asset.
		/// </summary>
		public readonly string guid;

		/// <summary>
		/// The asset path of this asset.
		/// </summary>
		public string AssetPath => AssetDatabase.GUIDToAssetPath(guid);

		private AssetReference(string guid)
		{
			this.guid = guid;
		}

		public static AssetReference FromGUID(string guid)
		{
			if(string.IsNullOrEmpty(guid))
			{
				throw new ArgumentException("GUID cannot be null or empty.");
			}
			return new AssetReference(guid);
		}

		public static AssetReference FromAssetPath(string assetPath)
		{
			if(string.IsNullOrEmpty(assetPath))
				throw new ArgumentException("Asset path cannot be null or empty.", nameof(assetPath));

			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			if(string.IsNullOrEmpty(guid))
				throw new ArgumentException($"No asset found at path: {assetPath}", nameof(assetPath));

			return new AssetReference(guid);
		}

		/// <summary>
		/// Returns the object represented by this asset reference, or null if the asset does not exist.
		/// </summary>
		public Object LoadAsset(Type type)
		{
			return AssetDatabase.LoadAssetAtPath(AssetPath, type);
		}

		/// <summary>
		/// Returns the object represented by this asset reference, or null if the asset does not exist.
		/// </summary>
		public T LoadAsset<T>() where T : Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(AssetPath);
		}

		/// <summary>
		/// Deletes the asset from the asset database.
		/// </summary>
		public void DeleteAsset()
		{
			AssetDatabase.DeleteAsset(AssetPath);
		}

		public override string ToString()
		{
			var path = AssetPath;
			if(string.IsNullOrEmpty(AssetPath)) return $"((invalid asset) [GUID:{guid}])";
			return $"({Path.GetFileName(path)} [GUID:{guid}])";
		}

		/// <summary>
		/// Searches the asset database for assets matching the given search filter.
		/// </summary>
		/// <param name="searchFilter">Search filter to apply.</param>
		/// <returns>Array of matching asset references.</returns>
		public static AssetReference[] FindAll(string searchFilter) => AssetDatabaseUtility.FindAssets(searchFilter);

		/// <summary>
		/// Searches the asset database for all assets of the specified type.
		/// </summary>
		/// <param name="type">The type of asset to search, includes classes inherited from this type.</param>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching asset references.</returns>
		public static AssetReference[] FindAllOfType(Type type, string searchFilter = null) => AssetDatabaseUtility.FindAllAssetsOfType(type, searchFilter);

		/// <summary>
		/// Searches the asset database for all assets of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of asset to search, includes classes inherited from this type.</typeparam>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching asset references.</returns>
		public static AssetReference[] FindAllOfType<T>(string searchFilter = null) where T : Object
		{
			return AssetDatabaseUtility.FindAllAssetsOfType<T>(searchFilter);
		}
	}

	public static class AssetDatabaseUtility
	{
		internal class CreateFromTemplateAssetAction : EndNameEditAction
		{
			private string templateFile;
			private Action<Object> callback;

			public static CreateFromTemplateAssetAction CreateAction(string templateFile, Action<Object> callback)
			{
				var instance = CreateInstance<CreateFromTemplateAssetAction>();
				instance.templateFile = templateFile;
				instance.callback = callback;
				return instance;
			}

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				string finalPath = AssetDatabase.GenerateUniqueAssetPath(pathName + Path.GetExtension(templateFile));
				AssetDatabase.CopyAsset(templateFile, finalPath);
				var newAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(finalPath);
				ProjectWindowUtil.ShowCreatedAsset(newAsset);
				callback?.Invoke(newAsset);
			}
		}

		/// <summary>
		/// Prompts the user to type a name for a new asset that will be created from the given template file.
		/// </summary>
		public static void BeginAssetCreationFromTemplateFile(string templateAssetPath, string defaultName, Texture2D icon, Action<Object> callback = null)
		{
			var action = CreateFromTemplateAssetAction.CreateAction(templateAssetPath, callback);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, defaultName, icon, templateAssetPath);
		}

		/// <summary>
		/// Searches the asset database for assets matching the given search filter.
		/// </summary>
		/// <param name="searchFilter">Search filter to apply.</param>
		/// <returns>Array of matching asset references.</returns>
		public static AssetReference[] FindAssets(string searchFilter)
		{
			return AssetDatabase.FindAssets(searchFilter).Select(guid => AssetReference.FromGUID(guid)).ToArray();
		}

		/// <summary>
		/// Searches the asset database for all assets of the specified type and returns their GUIDs.
		/// </summary>
		/// <param name="type">The type of asset to search, includes classes inherited from this type.</param>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching asset references.</returns>
		public static AssetReference[] FindAllAssetsOfType(Type type, string searchFilter = null)
		{
			searchFilter = string.IsNullOrEmpty(searchFilter) ? $"t:{type.Name}" : $"{searchFilter} t:{type.Name}";
			return AssetDatabase.FindAssets(searchFilter).Select(guid => AssetReference.FromGUID(guid)).ToArray();
		}

		/// <summary>
		/// Searches the asset database for all assets of the specified type and returns their asset paths.
		/// </summary>
		/// <param name="type">The type of asset to search, includes classes inherited from this type.</param>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching assets as asset paths.</returns>
		public static string[] FindAllAssetPathsOfType(Type type, string searchFilter = null)
		{
			return FindAllAssetsOfType(type, searchFilter)
				.Select(assetRef => assetRef.AssetPath)
				.ToArray();
		}

		/// <summary>
		/// Searches the asset database for all assets of the specified type and loads them.
		/// </summary>
		/// <param name="type">The type of asset to search, includes classes inherited from this type.</param>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching assets.</returns>
		public static Object[] LoadAllAssetsOfType(Type type, string searchFilter = null)
		{
			return FindAllAssetsOfType(type, searchFilter)
				.Select(assetRef => assetRef.LoadAsset(type))
				.ToArray();
		}



		/// <summary>
		/// Searches the asset database for all assets of the specified type and returns their GUIDs.
		/// </summary>
		/// <typeparam name="T">The type of asset to search, including classes inherited from this type.</typeparam>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching asset references.</returns>
		public static AssetReference[] FindAllAssetsOfType<T>(string searchFilter = null) where T : Object
		{
			return FindAllAssetsOfType(typeof(T), searchFilter);
		}

		/// <summary>
		/// Searches the asset database for all assets of the specified type and returns their asset paths.
		/// </summary>
		/// <typeparam name="T">The type of asset to search, including classes inherited from this type.</typeparam>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching assets as asset paths.</returns>
		public static string[] FindAllAssetPathsOfType<T>(string searchFilter = null) where T : Object
		{
			return FindAllAssetsOfType<T>(searchFilter)
				.Select(assetRef => assetRef.AssetPath)
				.ToArray();
		}

		/// <summary>
		/// Searches the asset database for all assets of the specified type and loads them.
		/// </summary>
		/// <typeparam name="T">The type of asset to search, includes classes inherited from this type.</typeparam>
		/// <param name="searchFilter">Optional search filter to apply.</param>
		/// <returns>An array of matching assets.</returns>
		public static T[] LoadAllAssetsOfType<T>(string searchFilter = null) where T : Object
		{
			return FindAllAssetsOfType<T>(searchFilter)
				.Select(assetRef => assetRef.LoadAsset<T>())
				.ToArray();
		}
	}
}
