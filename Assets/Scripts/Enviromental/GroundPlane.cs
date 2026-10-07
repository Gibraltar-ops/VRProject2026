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
   
   // Agerar nu enbart som en utplattnings-faktor för terrängen.
   [SerializeField] private float scaleMultiplier = 0.001f;

   private Vector3 initialScale;

   private void Start()
   {
      initialScale = this.transform.localScale;
      
      // Drar in alla groundplanes (+childrens) renderers i arrayen vid start 
      allRenderers = GetComponentsInChildren<Renderer>();
   }

   private void FixedUpdate()
   {
      if (platform == null) return;

      float currentAltitude = platform.GetAltitude();

      // Dödar alla groundplane renderers när vi är tillräckligt högt upp
      if (currentAltitude >= hideAltitude)
      {
         if (isVisible) // När vi åker över threshold är marken osynlig
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

         // Flyttar marken neråt för att skapa parallax och avstånd
         transform.position = new Vector3(0, -currentAltitude * moveMultiplier, 0);

         // Beräknar hur mycket vi ska platta ut (skala ner Y) marken
         float currentScale = currentAltitude * scaleMultiplier;
         
         // VIKTIGT: Vi behåller initialScale för X och Z så att marken inte krymper 
         // och blottar sina ytterkanter mot skyboxen. Vi subtraherar endast på Y-axeln.
         Vector3 newScale = initialScale;
         newScale.y = initialScale.y - currentScale;
         
         // Säkerställer att Y-axeln aldrig inverteras eller blir 0
         newScale.y = Mathf.Max(newScale.y, 0.01f);

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