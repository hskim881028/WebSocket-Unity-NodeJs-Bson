using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using WebSocketSharp;

public class Drag : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;
	
	private WebSocket ws;
	
	private float screenWidth;
	private float screenHeight;
	
	Vector3 ReCurScreenPoint;
	
	void Start() {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		ws = new WebSocket("ws://localhost:1337", "echo-protocol");
		ws.OnMessage += OnMessage;
		ws.Connect ();
	}
	
	void Update() {
		if (ReCurScreenPoint.x != 0) {
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(ReCurScreenPoint);
			transform.position = curPosition;
		}
	}
	
	void OnMessage(object sender, MessageEventArgs e){

		JsonSerializer serializer = new JsonSerializer();
		Stream stream = new MemoryStream(e.RawData);
		BsonReader reader = new BsonReader(stream);
		JPosition po = serializer.Deserialize<JPosition>(reader);
		Debug.Log("po X : " + po.X + "   po Y : " + po.Y);
		ReCurScreenPoint = new Vector3(po.X * screenWidth, po.Y * screenHeight, 10f);

		// JPosition po = JsonConvert.DeserializeObject<JPosition>(e.Data);
	}

	void OnMouseDrag() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);		
		float x = (float)Math.Round(curScreenPoint.x / Screen.width,3);
		float y = (float)Math.Round(curScreenPoint.y / Screen.height,3);
		
		var positon = new { X = x, Y = y };        
		Debug.Log("positon : " + positon);
		
		MemoryStream ms = new MemoryStream();
		JsonSerializer serializer = new JsonSerializer();
		BsonWriter writer = new BsonWriter(ms);
		serializer.Serialize(writer, positon);
		ms.Seek(0, SeekOrigin.Begin);
		byte[] byteBSON = ms.ToArray();
		ws.Send(byteBSON);

		// int byteLength = 0;
		// foreach (byte a in byteBSON) {
		// 	byteLength++;
		// 	Debug.Log(a + "! "); 
		// }

		BsonReader reader = new BsonReader(ms);
		JPosition dp = serializer.Deserialize<JPosition>(reader);

		Debug.Log("XXX: " + dp.X);
		Debug.Log("YYY : " + dp.Y);
		// ws.Send(JsonConvert.SerializeObject(positon));
	}

	class JPosition {
		public float X { get; set; }
		public float Y { get; set; }
	}
}
