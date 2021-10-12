using UnityEngine;
using UnityEngine.SceneManagement;
namespace DaMastaCoda.Scenes
{
	public class SceneManagerBehavior : MonoBehaviour
	{
		public void Reload()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}