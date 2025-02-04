namespace DayenCreation
{
    using System.IO;
    using UnityEditor;

	public static class CreateScriptFromTemplates
	{
		private const string ROOT_PATH = "Assets/_GearedFate/Code/Editor/Templates/";
		private const string PLACEHOLDER_RNS = "#ROOTNAMESPACE#";
		private const string PLACEHOLDER_CR = "#COPYRIGHT#";
		private const string COPYRIGHT = "//\n// Copyright (c) " + PLACEHOLDER_RNS + ". All rights reserved.\n//";

		private static readonly bool[] filesUpdated = new bool[7];

		private static void GenerateFile(string path, string fileName, int fileIndex)
		{
			if (!filesUpdated[fileIndex])
			{
				string rootNamespace = EditorSettings.projectGenerationRootNamespace;
				if (string.IsNullOrEmpty(rootNamespace))
				{
					rootNamespace = "BTG";
				}

				var copyright = COPYRIGHT;
				copyright = copyright.Replace(PLACEHOLDER_RNS, rootNamespace);

				string templateContent = File.ReadAllText(path);
				templateContent = templateContent.Replace(PLACEHOLDER_CR, copyright);

				File.WriteAllText(path, templateContent);
				filesUpdated[fileIndex] = true;
			}

			ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, fileName);
		}

		[MenuItem("Assets/Create/Code/MonoBehaviour", priority = 1)]
		public static void CreateMonoBehaviourScript()
		{
			var path = ROOT_PATH + "MonoBehaviour.txt";
			var fileName = "NewBehaviour.cs";
			GenerateFile(path, fileName, 0);
		}

		[MenuItem("Assets/Create/Code/Class", priority = 2)]
		public static void CreateClassScript()
		{
			var path = ROOT_PATH + "Class.txt";
			var fileName = "NewClass.cs";
			GenerateFile(path, fileName, 1);
		}

		[MenuItem("Assets/Create/Code/Enum", priority = 3)]
		public static void CreateEnumScript()
		{
			var path = ROOT_PATH + "Enum.txt";
			var fileName = "NewType.cs";
			GenerateFile(path, fileName, 2);
		}

		[MenuItem("Assets/Create/Code/ScriptableObject", priority = 4)]
		public static void CreateScriptableObjectScript()
		{
			var path = ROOT_PATH + "ScriptableObject.txt";
			var fileName = "NewSO.cs";
			GenerateFile(path, fileName, 3);
		}

		[MenuItem("Assets/Create/Code/Interface", priority = 5)]
		public static void CreateInterfaceScript()
		{
			var path = ROOT_PATH + "Interface.txt";
			var fileName = "NewInterface.cs";
			GenerateFile(path, fileName, 4);
		}

		[MenuItem("Assets/Create/Code/Struct", priority = 6)]
		public static void CreateStructScript()
		{
			var path = ROOT_PATH + "Struct.txt";
			var fileName = "NewStruct.cs";
			GenerateFile(path, fileName, 5);
		}

		[MenuItem("Assets/Create/Code/FiniteState", priority = 7)]
		public static void CreateFiniteStateScript()
		{
			var path = ROOT_PATH + "FiniteState.txt";
			var fileName = "NewState.cs";
			GenerateFile(path, fileName, 6);
		}
	}
}
