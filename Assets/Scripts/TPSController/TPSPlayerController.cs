﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace MMO
{
	public class TPSPlayerController : BasePlayerController
	{

		public float slopeLimit = 55;

		public float gravity = 20;
		public float jumpPower = 8;
		Vector3 _velocity;
		bool _grounded;


		public TPSCameraController tpsCameraController;
		CharacterController mCharacterController;
		float mInputX;
		float mInputY;
		Vector3 mVelocity;
		Transform mTrans;
		MMOUnitSkill mMMOUnitSkill;
		public float speed = 7;
		private UnitAnimator unitAnimator;

		public Canvas joystickCanvas;
		public ETCJoystick etcJoystick;


		void Awake ()
		{
			mCharacterController = GetComponent<CharacterController> ();
			unitAnimator = GetComponent<UnitAnimator> ();
			mTrans = transform;
			etcJoystick = MMO.PlatformController.Instance.etcJoystick.GetComponentInChildren<ETCJoystick> (true);
			etcJoystick.onMoveStart.AddListener (()=>{
				isPause = false;
			});
			Cursor.lockState = CursorLockMode.Locked;
		}

		bool mIsRuning;
		public bool isPause;
		float mNextShoot;
		void Update ()
		{

			if(Input.GetMouseButtonDown(0)){
				unitAnimator.StartFire ();
			}
			if(Input.GetMouseButton(0)){
				if (mNextShoot < Time.time) {
					PanelManager.Instance.mainInterfacePanel.Shoot ();
					Shoot ();
					mNextShoot = Time.time + 0.1f;
				}
			}
			if(Input.GetMouseButtonUp(0)){
				unitAnimator.StopFire ();
			}

			if(Input.GetKeyDown(KeyCode.Space)){
				Jump ();
			}

			if(Input.GetKeyDown(KeyCode.LeftCommand)){
				Squat ();
			}

			if(Input.GetKeyDown(KeyCode.Z)){
				Lying ();
			}

			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)){
				isPause = false;
			}
			if (isPause) {
				unitAnimator.SetMoveSpeed (0);
				return;
			}
			mInputX = Input.GetAxis ("Horizontal");
			mInputY = Input.GetAxis ("Vertical");

			if (etcJoystick != null && etcJoystick.gameObject.activeInHierarchy) {
				mInputX = etcJoystick.axisX.axisValue;
				mInputY = etcJoystick.axisY.axisValue;
			}

			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W)){
				unitAnimator.ResetAllAttackTriggers ();
			}

			Vector3 forward = tpsCameraController.transform.forward;
			forward = new Vector3 (forward.x, 0, forward.z).normalized;

			Vector3 plus = new Vector3 (mInputX, 0, mInputY).normalized;
			float angle = Vector3.Dot (plus, new Vector3 (1, 0, 0));
			int d = 1;
			if (mInputY < 0) {
				d = -1;
			}
			float angleY = Mathf.Acos (angle) * 180 / Mathf.PI * d - 90;
//			mTrans.forward = Quaternion.AngleAxis (-angleY, new Vector3 (0, 1, 0)) * forward;
			Vector3 moveDirection = Quaternion.AngleAxis (-angleY, new Vector3 (0, 1, 0)) * forward;

			if (mInputX != 0 || mInputY != 0) {
				unitAnimator.SetMoveSpeed (mInputY);
				unitAnimator.SetRight (mInputX);
			} else {
				unitAnimator.SetMoveSpeed (0);
				unitAnimator.SetRight (0);
			}
			RaycastHit hit;
			if (_grounded && Physics.Raycast (mTrans.position + new Vector3(0,2,0), -Vector3.up, out hit, Mathf.Infinity,1<< LayerConstant.LAYER_GROUND | 1<<LayerConstant.LAYER_DEFAULT)) {
				mTrans.position = new Vector3 (mTrans.position.x, hit.point.y, mTrans.position.z);
			}
			if (unitAnimator.IsIdle () || unitAnimator.IsFire() || unitAnimator.IsJump() || !_grounded) {
				if(mInputY<0){
					mInputY = mInputY / 4f;
				}
				Vector3 direct = moveDirection * Time.deltaTime * speed * new Vector3 (mInputX * 0.5f, 0, mInputY).magnitude ;
				if(_grounded){
					mCharacterController.Move (direct );
				}else{
					mCharacterController.Move (direct + new Vector3(0,_velocity.y * Time.deltaTime,0));
				}
				//TODO 暂时只能同步move和idle，attack无法同步。
				if (!mIsRuning) {
					mIsRuning = true;
					MMOController.Instance.SendPlayerAction (BattleConst.UnitMachineStatus.MOVE, -1,IntVector3.zero);
				}
			} else {
				if (mIsRuning) {
					mIsRuning = false;
					MMOController.Instance.SendPlayerAction (BattleConst.UnitMachineStatus.STANDBY, -1,IntVector3.zero);
				}
			}
			_velocity.y -= gravity * Time.deltaTime;

		}

		void OnControllerColliderHit(ControllerColliderHit col)
		{
			// This keeps the player from sticking to walls
			float angle = col.normal.y * 90;

			if(angle < slopeLimit)
			{
//				if(_grounded)
//				{
					_velocity = Vector3.zero;
//				}

//				if(_velocity.y > 0)
//				{
//					_velocity.y = 0;
//				}
//				else
//				{
//					_velocity += new Vector3(col.normal.x, 0, col.normal.z).normalized;
//				}
//
//				_grounded = false;
			}
			else
			{
				// Player is grounded here
				// If player falls too far, trigger falling damage
//				if(_t.position.y < _fall_start - fallThreshold)
//				{
//					FallingDamage(_fall_start - fallThreshold - _t.position.y);
//				}
//
//				_fall_start = _t.position.y;
//
				_grounded = true;
				_velocity.y = 0;
			}
		}

		void LateUpdate(){
			Vector3 forward = tpsCameraController.transform.forward;
			forward = new Vector3 (forward.x, 0, forward.z).normalized;
			mTrans.forward = forward;
		}

		void UpdateETCJoystickPos(){
			if(etcJoystick.activated){
				Vector2 touchPos = RectTransformUtility.PixelAdjustPoint(Input.GetTouch(etcJoystick.pointId).position,this.joystickCanvas.transform,this.joystickCanvas);
				//				etcJoystick.pointId

			}
		}

		bool CheckShootAble(){
			return true;
		}

		void Shoot(){
			if (CheckShootAble ()) {
				if(mMMOUnitSkill==null){
					mMMOUnitSkill = MMOController.Instance.player.GetComponent<MMOUnitSkill> ();
				}
				mMMOUnitSkill.skillList [0].targetPos = IntVector3.ToIntVector3(tpsCameraController.transform.forward);
				if (mMMOUnitSkill.skillList [0].Play ()) {
//					mMMOUnitSkill.mmoUnit.unitAnimator.SetTrigger (mMMOUnitSkill.skillList [0].mUnitSkill.anim_name);
				}
			}
		}

		Coroutine mToggleOffsetCoroutine;

		float mToggleDuration = 1f;

		IEnumerator _ToggleOffset(Vector3 target){
			float t = 0;
			Vector3 startOffset = tpsCameraController.targetOffset;
			Debug.Log (JsonUtility.ToJson(target));
			Debug.Log (JsonUtility.ToJson(startOffset));
			while(t<1){
				t += Time.deltaTime / mToggleDuration;
				tpsCameraController.targetOffset = Vector3.Lerp (startOffset,target,t);
				yield return null;
			}
		}

		void ToggleOffset(Vector3 target){
			if (mToggleOffsetCoroutine != null)
				StopCoroutine (mToggleOffsetCoroutine);
			mToggleOffsetCoroutine = StartCoroutine (_ToggleOffset(target));
		}

		void Squat(){
			if (unitAnimator.Squat ()) {
				ToggleOffset (new Vector3 (0, 1.8f, 0));
			} else {
				ToggleOffset (new Vector3 (0, 2.3f, 0));
			}
		}

		void Lying(){
			if (unitAnimator.Lying ()) {
				ToggleOffset (new Vector3 (0, 1.2f, 0)); 
			} else {
				ToggleOffset (new Vector3 (0, 2.3f, 0)); 
			}
		}

		void Reload(){
			unitAnimator.Reload ();
		}

		void Throw(){
		
		}

		void Melee(){
		
		}

		//Jump 正式一点跳跃应该是3个动作
		void Jump(){
//			StartCoroutine (_Jump());
//			unitAnimator.GetComponent<Rigidbody> ().AddForce (0,force,0);
			unitAnimator.SetTrigger (AnimationConstant.UNIT_ANIMATION_PARAMETER_JUMP);
			_velocity.y = jumpPower;
			_grounded = false;
		}
		public float force = 10;
		IEnumerator _Jump(){
			unitAnimator.GetComponent<Rigidbody> ().velocity = new Vector3 (0,100,0);// AddForce (0,force,0);
			unitAnimator.SetTrigger (AnimationConstant.UNIT_ANIMATION_PARAMETER_JUMP);
			while(true){
				yield return null;
			}
		}

	}
}