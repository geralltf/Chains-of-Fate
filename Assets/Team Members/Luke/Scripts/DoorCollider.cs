using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using FMODUnity;
using Unity.Collections;
using UnityEngine;


public enum DoorType
{
	Metal,
	Wooden,
	TrapDoor
}

public class DoorCollider : MonoBehaviour
{
	public DoorType type = DoorType.Wooden;

	[SerializeField] private StudioEventEmitter _emitter;
	private InteractTriggerBox _interactBox;

	private void OnEnable()
	{
		_emitter = GetComponent<StudioEventEmitter>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		InteractTriggerBox interactBox = other.GetComponent<InteractTriggerBox>();
		if (interactBox != null)
		{
			_interactBox = interactBox;
			interactBox.InteractEvent += UseDoor;
		}
	}
	
	private void OnTriggerExit2D(Collider2D other)
	{
		InteractTriggerBox interactBox = other.GetComponent<InteractTriggerBox>();
		if (interactBox != null)
		{
			interactBox.InteractEvent -= UseDoor;
		}
	}

	private void OnDisable()
	{
		if (_interactBox != null) _interactBox.InteractEvent -= UseDoor;
	}
	
	private void OnDestroy()
	{
		if (_interactBox != null) _interactBox.InteractEvent -= UseDoor;
	}

	private void UseDoor()
	{
		_emitter.SetParameter("DoorType" , (int) type);
		_emitter.Play();
	}
}
