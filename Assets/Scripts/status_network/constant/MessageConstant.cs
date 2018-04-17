﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
	public static class MessageConstant
	{
		public const short SERVER_TO_CLIENT_MSG = 8001;
		public const short PLYAER_INIT_INFO = 8002;
		public const short SERVER_TO_CLIENT_MONSTER_INFO = 8003;
		public const short SERVER_TO_CLIENT_SKILL = 8004;
		//public const short SERVER_TO_CLIENT_HIT = 8005;
		public const short CLIENT_TO_SERVER_MSG = 9001;
		public const short CLIENT_TO_SERVER_PLAYER_INFO = 9002;
		public const short PLAYER_RESPAWN = 9003;
		public const short PLAYER_ACTION = 10001;
		public const short PLAYER_VOICE = 10003;

		public const short PLAYER_SHOOT = 10004;
	}
}
