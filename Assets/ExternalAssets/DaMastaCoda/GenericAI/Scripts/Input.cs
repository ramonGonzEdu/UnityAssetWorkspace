using System.Collections.Generic;

namespace DaMastaCoda.GenericAI
{
	public interface IInput
	{
		bool GetKey(UnityEngine.KeyCode key);
		bool GetKeyDown(UnityEngine.KeyCode key);
		bool GetKeyUp(UnityEngine.KeyCode key);
		float GetAxis(string key);
		float GetAxisRaw(string key);
	}

	public interface InputHaving
	{
		void SetInput(IInput p_Input);
		IInput GetInput();
	}

	public class PlayerInput : IInput
	{
		static public PlayerInput Singleton = new PlayerInput();

		bool IInput.GetKey(UnityEngine.KeyCode key)
		{
			return UnityEngine.Input.GetKey(key);
		}
		bool IInput.GetKeyDown(UnityEngine.KeyCode key)
		{
			return UnityEngine.Input.GetKeyDown(key);
		}
		bool IInput.GetKeyUp(UnityEngine.KeyCode key)
		{
			return UnityEngine.Input.GetKeyUp(key);
		}
		float IInput.GetAxis(string key)
		{
			return UnityEngine.Input.GetAxis(key);
		}
		float IInput.GetAxisRaw(string key)
		{
			return UnityEngine.Input.GetAxisRaw(key);
		}
	}

	public class ControllableInput : IInput
	{

		Dictionary<UnityEngine.KeyCode, bool> keys = new Dictionary<UnityEngine.KeyCode, bool>();
		Dictionary<UnityEngine.KeyCode, bool> keysPrev = new Dictionary<UnityEngine.KeyCode, bool>();
		Dictionary<string, float> axes = new Dictionary<string, float>();

		public void SetKey(UnityEngine.KeyCode code, bool state)
		{
			keys[code] = state;
		}
		public void SetAxis(string axis, float state)
		{
			axes[axis] = state;
		}

		bool IInput.GetKey(UnityEngine.KeyCode key)
		{
			// return UnityEngine.Input.GetKey(key);
			bool test = false;
			keys.TryGetValue(key, out test);
			return test;
		}
		bool IInput.GetKeyDown(UnityEngine.KeyCode key)
		{
			bool test = false;
			bool testprev = false;
			keys.TryGetValue(key, out test);
			keysPrev.TryGetValue(key, out testprev);
			keysPrev[key] = test;
			return !testprev && test;
		}
		bool IInput.GetKeyUp(UnityEngine.KeyCode key)
		{
			bool test = false;
			bool testprev = false;
			keys.TryGetValue(key, out test);
			keysPrev.TryGetValue(key, out testprev);
			keysPrev[key] = test;
			return testprev && !test;
		}
		float IInput.GetAxis(string key)
		{
			float test = 0.0f;
			axes.TryGetValue(key, out test);
			return test;
		}
		float IInput.GetAxisRaw(string key)
		{
			float test = 0.0f;
			axes.TryGetValue(key, out test);
			return test;
		}
	}
}