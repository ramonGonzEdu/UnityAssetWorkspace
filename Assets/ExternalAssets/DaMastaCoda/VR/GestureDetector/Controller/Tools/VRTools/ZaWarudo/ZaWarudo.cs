namespace DaMastaCoda.VR.GestureDetector.Controller.Tools.VRTools.ZaWarudo
{
	using UnityEngine;

	public class ZaWarudo : Tool
	{

		void Update()
		{
			OVRInput.Update();


			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, handInput))
			{
				Time.timeScale = 0.2f;
			}
			else
			{
				Time.timeScale = 1.0f;
			}
		}
	}
}
