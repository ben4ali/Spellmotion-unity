using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System;


public class InputMotionManager : MonoBehaviour
{
	[Header("WebSocket Settings")]
	public string wsUrl = "ws://localhost:8765";
	public bool debugLogging = true;

	private WebSocket ws;
	private SpellBook spellBook;

	private void Start()
	{

		_ = UnityMainThreadDispatcher.Instance();

		spellBook = FindObjectOfType<SpellBook>();
		if (spellBook == null)
		{
			Debug.LogError("[InputMotionManager] Aucun SpellBook trouvé !");
			return;
		}

		ws = new WebSocket(wsUrl);
		ws.OnOpen += (sender, e) => Debug.Log("[WebSocket] Connected to gesture server.");
		ws.OnMessage += OnWsMessage;
		ws.OnError += (sender, e) => Debug.LogError($"[WebSocket] Error: {e.Message}");
		ws.OnClose += (sender, e) => Debug.Log("[WebSocket] Connection closed.");

		ws.ConnectAsync();
	}

	private void OnWsMessage(object sender, MessageEventArgs e)
	{
		if (debugLogging) Debug.Log($"[WebSocket] Received: {e.Data}");

		try
		{
			JObject json = JObject.Parse(e.Data);
			string type = json["type"]?.ToString();

			if (type == "spell")
			{
				string spell = json["spell"]?.ToString();
				if (!string.IsNullOrEmpty(spell))
				{
					DispatchSpell(spell);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError($"[WebSocket] JSON parse error: {ex.Message}");
		}
	}

	private void DispatchSpell(string spell)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			switch (spell.ToUpper())
			{
				case "ARCANE BOLT":
					spellBook.CastArcaneBlast();
					break;
				case "FIREBALL":
					spellBook.CastFireball();
					break;
				case "LIGHTNING":
					spellBook.CastLightning();
					break;
				case "ICE SPIKE":
					spellBook.CastIceSpikes();
					break;
				case "HEAL":
					spellBook.CastHeal();
					break;
				default:
					Debug.LogWarning($"[InputMotionManager] Spell inconnu: {spell}");
					break;
			}
		});
	}

	private void OnDestroy()
	{
		if (ws != null && ws.IsAlive)
		{
			ws.Close();
		}
	}
}
