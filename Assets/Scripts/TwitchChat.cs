﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class TwitchChat : MonoBehaviour {

    public hangmanGame hangmanScript;
    public string username, password, channelName; // Get the password from https://twitchapps.com/tmi
    public Text chatBox;
    public char letter;

    private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;

	// Use this for initialization
	void Start () {
		Connect ();
	}
	
	// Update is called once per frame
	void Update () {
		// if client is not connected...
		if (!twitchClient.Connected)
		{
			Debug.Log("Attempting to reconnect...");
			// call connect funciton to connect again
			Connect ();
		}

		ReadChat ();
	}

	private void Connect()
	{
		twitchClient = new TcpClient ("irc.chat.twitch.tv", 6667);
		reader = new StreamReader (twitchClient.GetStream ());
		writer = new StreamWriter (twitchClient.GetStream ());

		writer.WriteLine ("PASS " + password);
		writer.WriteLine ("NICK " + username);
		writer.WriteLine ("USER "  + username + " 8 * :" + username);
		writer.WriteLine ("JOIN #" + channelName);
		writer.Flush ();
		Debug.Log("Connected.");
	}

	private void ReadChat()
	{
		if (twitchClient.Available > 0)
		{
			// Read the current message
			var message = reader.ReadLine ();
			Debug.Log("Message read");

			if (message.Contains("PRIVMSG"))
			{
				Debug.Log("Message contains PRIVMSG");
				// Get the user's name by splitting it from the string
				var splitPoint = message.IndexOf("!", 1);
				var chatName = message.Substring(0, splitPoint);
				chatName = chatName.Substring(1);

				// Get the user's message by splitting it from the string
				splitPoint = message.IndexOf (":", 1);
				message = message.Substring(splitPoint + 1);
				print (String.Format("{0}: {1}", chatName, message));
				// Print message in chat box
				chatBox.text = chatBox.text + "\n" + String.Format ("{0}: {1}", chatName, message);

                if (message.StartsWith("!"))
                {
                    letter = message[1];
                    Debug.Log("Letter " + letter + " has been picked");
                    hangmanScript.CheckLetter();
                }
			}
		}
	}

	private void GameInputs(string ChatInputs)
	{
		
	}
}
