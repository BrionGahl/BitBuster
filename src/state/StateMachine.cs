using System;
using Godot;
using Godot.Collections;

namespace BitBuster.state;

public partial class StateMachine : Node2D
{

	[Export]
	public State InitialState { get; set; }
	
	public State CurrentState { get; private set; }

	private Dictionary<string, State> _states;

	public override void _Ready()
	{
		_states = new Dictionary<string, State>();
		
		foreach (Node child in GetChildren())
		{
			if (child is State state)
			{
				_states.Add(state.Name.ToString().ToLower(), state);
				state.Init();
				state.StateTransition += OnStateTransition;
			}
		}

		if (InitialState == null)
			InitialState = _states[GetChild(0).Name.ToString().ToLower()];
			
		InitialState.Enter();
		CurrentState = InitialState;
	
	}

	public override void _Process(double delta)
	{
		if (CurrentState != null)
			CurrentState.StateUpdate(delta);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (CurrentState != null)
			CurrentState.StatePhysicsUpdate(delta);
	}

	private void OnStateTransition(State state, string newStateName)
	{
		if (state != CurrentState)
			return;

		State newState = _states[newStateName.ToLower()];
		if (newState == null)
			return;

		if (CurrentState != null)
			CurrentState.Exit();
		
		newState.Enter();
		CurrentState = newState;
	}

}
