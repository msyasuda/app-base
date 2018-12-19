using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public class AssetReference : EditorWindow {
	private static GUIStyle searchBar;
	private static GUIStyle cancelButton;
	private static GUIStyle cancelButtonEmpty;
	private string text = "";
	private Vector2 scrollPosition = new Vector2(0, 0);
	private List<AssetInfo> assets = new List<AssetInfo> ();

	public class AssetInfo {
		public string Path { get; private set; }
		public string Guid { get; private set; }

		public AssetInfo(string guid, string path) {
			this.Path = path;
			this.Guid = guid;
		}	
	}

	[MenuItem("Assets/AssetReference")]
	static void Open() {
		GetWindow<AssetReference> ();
	}

	private void OnGUI() {
		searchBar 			= GetStyle ("ToolbarSeachTextField");
		cancelButton 		= GetStyle ("ToolbarSeachCancelButton");
		cancelButtonEmpty 	= GetStyle ("ToolbarSeachCancelButtonEmpty");

		GUILayoutOption[] layoutOptions = new GUILayoutOption[] { 
			GUILayout.Width (250f) 
		};

		var rect = GUILayoutUtility.GetRect (16f, 24f, 16f, 16f, layoutOptions);
		rect.x += 4f;
		rect.y += 4f;

		Rect position3 = rect;
		position3.x += rect.width;
		position3.width = 14f;

		var newText = EditorGUI.TextField(rect, this.text, searchBar);
		if (newText == "")
		{
			GUI.Button(position3, GUIContent.none, cancelButton);
		}
		else
		{
			if (GUI.Button(position3, GUIContent.none, cancelButtonEmpty))
			{
				newText = "";
				GUIUtility.keyboardControl = 0;
			}
		}

		if (text != newText) {
			OnTextChanged (newText);
		}

		SetScrollView ();
	}		

	static private GUIStyle GetStyle(string styleName)
	{
		GUIStyle gUIStyle = GUI.skin.FindStyle(styleName);
		if (gUIStyle == null)
		{
			gUIStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
		}
		if (gUIStyle == null)
		{
			Debug.LogError("Missing built-in guistyle " + styleName);
			gUIStyle = new GUIStyle();
		}
		return gUIStyle;
	}

	private void OnTextChanged(string changedText) {
		this.text = changedText;
	
		assets.Clear ();

		if (string.IsNullOrEmpty (changedText)) {
			return;
		}		

		var assetPaths = GetAssets (changedText);
		assets = assetPaths;
	}

	private List<AssetInfo> GetAssets(string searchString) {
        var guids = AssetDatabase.FindAssets (searchString);

		return guids.Select (guid => new AssetInfo(guid, AssetDatabase.GUIDToAssetPath (guid))).ToList();
	}		

	private void SetScrollView() {
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.BeginVertical ();
        string path = Application.dataPath;
		foreach (var asset in assets)
		{
            string label = string.Format ("{0} [GUID : {1}]", asset.Path, asset.Guid);
            Texture icon = AssetDatabase.GetCachedIcon(asset.Path);
			if (icon == null) {
				continue;
			}

            // Asset下の指定拡張子ファイルを取得
            var referenceList = new List<string> ();
            referenceList.AddRange(Grep(path, "*.unity", asset.Guid));
            referenceList.AddRange(Grep(path, "*.prefab", asset.Guid));
   

            GUILayout.BeginHorizontal ();
            GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
			GUILayout.Label(label);
            GUILayout.EndHorizontal ();


            foreach (var refer in referenceList) {
                var relativePath = "Assets" + refer.Substring (path.Length, refer.Length - path.Length);
                Texture icon2 = AssetDatabase.GetCachedIcon(relativePath);
                if (icon == null) {
                    continue;
                }
                GUILayout.BeginHorizontal ();
                GUILayout.Label(icon2, GUILayout.Width(20), GUILayout.Height(20));
                GUILayout.Label(relativePath);
                GUILayout.EndHorizontal ();
            } 


		}
        GUILayout.EndVertical();

		EditorGUILayout.EndScrollView();


	}

    public static List<string> Grep(string dirPath, string wildCard, string pattern) {
        DirectoryInfo dir = new DirectoryInfo (dirPath);
        var fileInfoList = dir.GetFiles (wildCard, System.IO.SearchOption.AllDirectories);
        var fileList = new List<string> ();
        foreach (var fileInfo in fileInfoList) {
            string fileText = GetFileContent (fileInfo.FullName);
            Regex regex = new Regex (pattern);
            if (regex.IsMatch (fileText)) {
                fileList.Add (fileInfo.FullName);
            }
        }

        return fileList;
    }    

    public static string GetFileContent(string filePath)
    {
        StreamReader reader = null;
        string content = "";
        try {
            reader = new StreamReader(filePath, System.Text.Encoding.Default, true);
            content = reader.ReadToEnd();
        } catch {
            Debug.Log ("ファイルの読み込みに失敗しました。path : " + filePath);
        }
        finally{
            reader.Close();
        }

        return content;
    }
}
