using UnityEngine;

public class GroundPlane : MonoBehaviour
{
   [Header("Platformsreferens")] 
   [SerializeField] private PlatformDynamics platform;

   [Header("Rendererreferens")] 
   private Renderer[] allRenderers;
   private bool isVisible = true;

   [Header("Altitude visuals")] 
   [SerializeField] private float hideAltitude = 2000f;
   [SerializeField] private float moveMultiplier = 0.5f;
   [SerializeField] private float scaleMultiplier = 0.001f;

   private Vector3 initialScale;

   private void Start()
   {
      initialScale = this.transform.localScale;
      
      // Drar in alla groundplanes (+childrens) renderers i arrayen vid start 
      allRenderers = GetComponentsInChildren<Renderer>();
   }

   private void Update()
   {
      if (platform == null) return;

      float currentAltitude = platform.GetAltitude();

      // Dödar alla groundplane renderers när vi är tillräckligt högt upp
      if (currentAltitude >= hideAltitude)
      {
         if (isVisible) // När vi åker över threshold är marken osynling
         {
            SetRenderersState(false);
            isVisible = false;
         }
      }
      else
      {
         if (!isVisible) // När vi är under threshold ser vi marken
         {
            SetRenderersState(true);
            isVisible = true;
         }

         // Flyttar marken neråt
         transform.position = new Vector3(0, -currentAltitude * moveMultiplier, 0);

         // Skalar ner marken 
         float currentScale = currentAltitude * scaleMultiplier;
         Vector3 newScale = initialScale - (Vector3.one * currentScale);
         
         
         newScale.x = Mathf.Max(newScale.x, 0.01f);
         newScale.y = Mathf.Max(newScale.y, 0.01f);
         newScale.z = Mathf.Max(newScale.z, 0.01f);

         transform.localScale = newScale;
      }
   }
   
   // hjälpmetod för att slå på och av grafik
   private void SetRenderersState(bool state)
   {
      foreach (Renderer rend in allRenderers)
      {
         if (rend != null)
         {
            rend.enabled = state;
         }
      }
   }

}
