using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskSwapper : MonoBehaviour
{
    //public List<Component> masks;
    public int maskId = -1;
    public GameObject player;

    private Mask currentMask = null;
    private List<Mask> imasks = new List<Mask>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(currentMask != null && currentMask.CanApply(player))
        {
            currentMask.ApplyAbilities(player);
        }

        imasks = player.GetComponents<Mask>().ToList();
        imasks.Sort((a, b) => a.GetMaskId().CompareTo(b.GetMaskId()));
        Debug.LogWarning("Found masks " + imasks.Count);
        //foreach (var mask in masks)
        //{
        //    imasks.Add(mask.Cas);
        //}
    }

    void removeCurrentMask()
    {
        if (currentMask != null)
            currentMask.RemoveAbilities(player);
        maskId = -1;
        currentMask = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentMask != null && currentMask.ShouldRemove(player))
        {
            removeCurrentMask();
        }
    }

    public void OnRemoveMask(InputValue context)
    {
        if(context.isPressed)
        {
            removeCurrentMask();
        }
    }

    public void ProcessMaskRequest(int i, InputValue v)
    {
        if (v.isPressed && maskId != i && i < imasks.Count && imasks[i].CanApply(player))
        {
            removeCurrentMask();
            imasks[i].ApplyAbilities(player);
            maskId = i;
            currentMask = imasks[i];
        }
    }

    public void OnMask1(InputValue context)
    {
        ProcessMaskRequest(0, context);
    }

    public void OnMask2(InputValue context)
    {
        ProcessMaskRequest(1, context);
    }

    public void OnMask3(InputValue context)
    {
        ProcessMaskRequest(2, context);
    }
    public void OnMask4(InputValue context)
    {
        ProcessMaskRequest(3, context);
    }
}

