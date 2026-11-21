using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WorkshopEditor
{
    [MenuItem("Практикум/Показать текущую сцену в редакторе")]
    public static void Preview()
    {
        var workshop=Object.FindAnyObjectByType<TwinWorkshop>();
        if (!workshop) { Debug.LogWarning("Откройте сцену из Assets/Scenes."); return; }
        if (workshop.transform.childCount==0) workshop.Build();
        const string folder="Assets/WorkshopMaterials";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets","WorkshopMaterials");
        foreach (Renderer renderer in workshop.GetComponentsInChildren<Renderer>()) {
            Material material=renderer.sharedMaterial;
            if(!material || EditorUtility.IsPersistent(material)) continue;
            string path=folder+"/"+material.name+".mat";
            Material existing=AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!existing) AssetDatabase.CreateAsset(material,path);
            else renderer.sharedMaterial=existing;
        }
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        if (SceneView.lastActiveSceneView) {
            var view=SceneView.lastActiveSceneView;
            view.LookAt(workshop.lesson<=3?Vector3.zero:new Vector3(0,0,3),
                        Quaternion.Euler(25,-20,0), workshop.lesson<=3?13:35);
            view.Repaint();
        }
    }
    [MenuItem("Практикум/Сохранить три вида камер")]
    public static void CaptureCameras()
    {
        Preview();
        var workshop=Object.FindAnyObjectByType<TwinWorkshop>();
        if(!workshop || workshop.lesson>3) return;
        string folder=EditorUtility.OpenFolderPanel("Папка для трёх видов камер","","");
        if(string.IsNullOrEmpty(folder)) return;
        foreach(var camera in workshop.GetComponentsInChildren<Camera>()) {
            var target=RenderTexture.GetTemporary(1440,900,24);
            var oldTarget=camera.targetTexture; var oldActive=RenderTexture.active;
            var image=new Texture2D(1440,900,TextureFormat.RGB24,false);
            try {
                camera.targetTexture=target; camera.Render(); RenderTexture.active=target;
                image.ReadPixels(new Rect(0,0,1440,900),0,0); image.Apply();
                File.WriteAllBytes(Path.Combine(folder,"Дисплей "+(camera.targetDisplay+1)+".png"),image.EncodeToPNG());
            } finally {
                camera.targetTexture=oldTarget; RenderTexture.active=oldActive;
                Object.DestroyImmediate(image); RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
