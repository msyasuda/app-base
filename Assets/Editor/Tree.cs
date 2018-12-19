using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;

public class TreeViewTest : TreeView {
    public TreeViewTest(TreeViewState state) : base(state) {
    }

    protected override TreeViewItem BuildRoot ()
    {
        var root = new TreeViewItem (id:1, depth:-1, displayName:"ルート");
        var node1 = new TreeViewItem (id:2, depth:0, displayName:"node1");
        var node5 = new TreeViewItem (id:5, depth:0, displayName:"node5");

        //node1.icon = AssetDatabase.GetCachedIcon("");
        root.children = new List<TreeViewItem> {
            node1,
            node5,
            new TreeViewItem (id:3, depth:0, displayName:"node2")
        };

        node1.children = new List<TreeViewItem> {
            new TreeViewItem (id:4, depth:2, displayName:"node3")            
        };

        return root;
    }
}    

public class Tree : EditorWindow {
    private TreeViewState state;
    private TreeViewTest view;

    [MenuItem("Assets/treeview")]
    static void OnOpen() {
        GetWindow<Tree> ();
    }    

    private void OnGUI() {
        view.OnGUI(new Rect(0, 0, position.width, position.height));
    }
   
    private void OnEnable() {
        state = new TreeViewState ();
        view = new TreeViewTest (state);
        view.Reload ();
    }   
}

