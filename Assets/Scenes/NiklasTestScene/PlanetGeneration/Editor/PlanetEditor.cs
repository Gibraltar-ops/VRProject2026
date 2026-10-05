using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor; // cached editors 
    Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if(GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);

    }

    // fouldout has to be by reference so it acual changes to it are stored
    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings == null) // return early if no scriptable object is assigned
        {
            return;
        }

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        
        // check if editor has changed 
        using (var check = new EditorGUI.ChangeCheckScope()) 
        {
            if (foldout)
            {
                //Editor editor = CreateEditor(settings);
                CreateCachedEditor(settings,null, ref editor); // null is using the default editor type
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    if (onSettingsUpdated != null)
                    {
                        onSettingsUpdated(); // call the custom update function passed in
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
