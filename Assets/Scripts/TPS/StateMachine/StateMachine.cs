using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum EnumState { PATROL, TRACKING, ATTACKING, DEATH,HITED,IDLE,THIRDAIM, FIRSTAIM };
    Dictionary<EnumState, IState> stateDictionary = new Dictionary<EnumState, IState>();
    IState nowState;
    EnumState nowEnumState;

    public void insertState(EnumState stateName, IState state)
    {
        stateDictionary[stateName] = state;
    }
    public void initState(EnumState stateName)
    {
        var state = stateDictionary[stateName];
        if (state != null)
        {
            nowEnumState = stateName;
            nowState = state;
            nowState.enter();
        }
    }
    public void changeState(EnumState stateName)
    {
        var state = stateDictionary[stateName];
        if (state != null)
        {
            if (nowState != null)
            {
                nowState.exit();
            }
            nowState = state;
            nowEnumState = stateName;
            nowState.enter();
        }
    }

    public EnumState getCurrentState()
    {
        return nowEnumState;
    }

    // Update is called once per frame
    void Update()
    {
        if (nowState != null)
        {
            nowState.update();
        }
    }

    private void LateUpdate()
    {
        if (nowState != null)
        {
            nowState.lateUpdate();
        }
    }
}
