﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MMO
{
	//TODO 臨時使用します。
	public class PanelManager : SingleMonoBehaviour<PanelManager>
	{

		public LoginPanel loginPanel;
		public MainInterfacePanel mainInterfacePanel;
		public ChatPanel chatPanel;
		public ServerListPanel serverListPanel;

		protected override void Awake ()
		{
			base.Awake ();
		}

		void Start(){
			loginPanel.gameObject.SetActive (true);
		}

		public void InitSkillIcons(MMOUnitSkill mmoUnitSkill){
			mainInterfacePanel.SetSkillDatas (mmoUnitSkill);
		}


	}
}
