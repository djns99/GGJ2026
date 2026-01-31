using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MaskSwapper : MonoBehaviour
{
    //public List<Component> masks;
    public int maskId = -1;
    public GameObject player;

    public Canvas canvas;

    private Mask currentMask = null;
    private List<Mask> imasks = new List<Mask>();
    private List<UnityEngine.UI.Image> selectedImages;


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
        var maskUiItems = canvas.GetComponentsInChildren<UnityEngine.UI.Image>();
        for (int i = 0; i < imasks.Count; i++ )
        {
            foreach(var item in maskUiItems)
            {
                Debug.Log(item);
                Debug.Log(item.gameObject.name);
                if (item.gameObject.name == "Mask" + (i + 1))
                {
                    var maskSprite = imasks[i].GetSprite();
                    Debug.Log("Set mask " + item.gameObject.name + " mask " + imasks[i].GetSprite());
                    item.sprite = maskSprite;
                    item.color = Color.white;
                    break;
                }
            }
        }

        selectedImages = maskUiItems.ToList().FindAll(x => x.name.Contains("MaskSelected"));
        selectedImages.Sort((a, b) => a.name.CompareTo(b.name));
        selectedImages.ForEach(x => x.enabled = false);
        Debug.Log("Found " + selectedImages.Count + " selected element UI items");
    }

    void removeCurrentMask()
    {
        if (currentMask != null)
            currentMask.RemoveAbilities(player);
        if (maskId != -1)
            selectedImages[maskId].enabled = false;
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

    public void OnUIMaskElementClicked(GameObject maskClicked)
    {
        Debug.Log("Clicked");
        GetComponent<MaskSwapper>().ProcessMaskRequest(Int32.Parse(maskClicked.name.Last().ToString()) - 1, false);
    }

    public void ProcessMaskRequest(int i, bool enabled)
    {
        if (enabled && maskId != i && i < imasks.Count && imasks[i].CanApply(player))
        {
            removeCurrentMask();
            imasks[i].ApplyAbilities(player);
            maskId = i;
            currentMask = imasks[i];
            selectedImages[maskId].enabled = true;
        }
    }

    public void OnMask1(InputValue context)
    {
        ProcessMaskRequest(0, context.isPressed);
    }

    public void OnMask2(InputValue context)
    {
        ProcessMaskRequest(1, context.isPressed);
    }

    public void OnMask3(InputValue context)
    {
        ProcessMaskRequest(2, context.isPressed);
    }
    public void OnMask4(InputValue context)
    {
        ProcessMaskRequest(3, context.isPressed);
    }
}

